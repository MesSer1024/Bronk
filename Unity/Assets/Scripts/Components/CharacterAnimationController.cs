using UnityEngine;
using System.Collections;
using Bronk;

[RequireComponent (typeof(Animator))]
public class CharacterAnimationController : MonoBehaviour, ITimelineObject
{
	Animator _Animator;
	Transform _Transform;
	private PositionTimeline _PositionTimeline;
	private AntStateTimeline _StateTimeline;
	private Vector3 Velocity;
	private AnimatorStateInfo _CurrentStateInfo;
	private AnimatorStateInfo _NextStateInfo;
	private static ParticleSystem _MiningParticleSystem;

	void Awake ()
	{
		_Animator = GetComponent<Animator> ();
		_Transform = GetComponent<Transform> ();
		_StateTimeline = AntStateTimeline.Create ();
		_PositionTimeline = PositionTimeline.Create ();
	}

	void Update ()
	{
		_CurrentStateInfo = _Animator.GetCurrentAnimatorStateInfo (0);
		_NextStateInfo = _Animator.GetNextAnimatorStateInfo (0);

		GameEntity.States state = _StateTimeline.GetValue (Time.time);

		switch (state) {
		case GameEntity.States.Idle: 
			DoIdle ();
			break;
		case GameEntity.States.Mine:
			DoMining ();
			break;
		case GameEntity.States.Move:
			DoMovement ();
			break;
		case GameEntity.States.Sleep:
			DoSleep ();
			break;
		}


		float oldTime = Time.time - Time.deltaTime;
		Vector2 oldPosition2D = _PositionTimeline.GetValue (oldTime);
		Vector2 newPosition2D = _PositionTimeline.GetValue (Time.time);
		Vector3 oldPosition = new Vector3 (oldPosition2D.x, 0, oldPosition2D.y);
		Vector3 newPosition = new Vector3 (newPosition2D.x, 0, newPosition2D.y);
		transform.position = newPosition;
		Velocity = (newPosition - oldPosition); 
		if (Velocity != Vector3.zero) {
			Velocity /= Time.deltaTime;
			_Transform.rotation = Quaternion.LookRotation (Velocity.normalized);
		}
			
		// Animations are timed for scale = 1. // If we divide the speed by the scale we'll play run animations slower/faster if they are bigger/smaller.
		float sizeScaleFactor = _Transform.localScale.x * _Animator.humanScale;
		float velocityMagnitude = Velocity.magnitude / sizeScaleFactor;
		_Animator.SetFloat ("Speed", velocityMagnitude);
	}

	public ITimeline GetTimeline (TimelineType type)
	{
		switch (type) {
		case TimelineType.AntState:
			return _StateTimeline;
		case TimelineType.Position:
			return _PositionTimeline;
		}
		return null;
	}

	public void PlayAnimation (AnimationEnum animationEnum, float stateTime, float fadeTime = 0.1f)
	{
		float normalizedTime = (stateTime / Animations.Get (animationEnum).Lenght) % 1f;
		_Animator.CrossFade (Animations.Get (animationEnum).Hash, Mathf.Max (0f, fadeTime - normalizedTime), 0, normalizedTime);
		//Debug.Log("startTime: " +stateTime + " normalized: " + normalizedTime + " fade: " + fadeTime + "  " + Time.time);
	}

	void DoEffect (int id)
	{
		if (_MiningParticleSystem == null)
			_MiningParticleSystem = Instantiate (Resources.Load<ParticleSystem> ("ParticleEffects/MineImpact")) as ParticleSystem;

		_MiningParticleSystem.transform.position = transform.position + Vector3.up * 2;
		_MiningParticleSystem.Emit (15);
		Debug.Log ("Do Effect! " + id);
		
	}

	void DoIdle ()
	{
		
	}

	void DoSleep ()
	{
		AnimationEnum animEnum = AnimationEnum.Death;
		if (_CurrentStateInfo.nameHash != Animations.Get (animEnum).Hash &&
		    _NextStateInfo.nameHash != Animations.Get (animEnum).Hash) {
			PlayAnimation (animEnum, _StateTimeline.GetCurrentKeyframeTime (Time.time));
		}
	}

	void DoMining ()
	{
		AnimationEnum animEnum = AnimationEnum.Mine;
		if (_CurrentStateInfo.nameHash != Animations.Get (animEnum).Hash &&
		    _NextStateInfo.nameHash != Animations.Get (animEnum).Hash) {
			PlayAnimation (animEnum, _StateTimeline.GetCurrentKeyframeTime (Time.time));
		}
	}

	void DoMovement ()
	{

	}
}









