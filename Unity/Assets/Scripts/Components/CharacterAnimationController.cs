using UnityEngine;
using System.Collections;
using Bronk;

[RequireComponent(typeof(Animator))]
public class CharacterAnimationController : MonoBehaviour 
{
	Animator _Animator;
	Transform _Transform;

	public ITimelinedEntity LogicCharacter;

	private Vector3 Velocity;

	private AntStateTimeline _StateTimeline;
	private AnimatorStateInfo _CurrentStateInfo;
	private AnimatorStateInfo _NextStateInfo;

	void Awake () 
	{
		_Animator = GetComponent<Animator>();
		_Transform = GetComponent<Transform>();
	}

	void Update () 
	{
		_CurrentStateInfo = _Animator.GetCurrentAnimatorStateInfo(0);
		_NextStateInfo = _Animator.GetNextAnimatorStateInfo(0);

		if (LogicCharacter != null)
		{
			_StateTimeline = LogicCharacter.GetStateTimeline();
			GameEntity.States state = _StateTimeline.GetValue(Time.time);

			switch (state) 
			{
			case GameEntity.States.Idle: 
				DoIdle();
				break;
			case GameEntity.States.Mine:
				DoMining();
				break;
			case GameEntity.States.Move:
				DoMovement();
				break;
			case GameEntity.States.Sleep:
				DoSleep();
				break;
			}

			PositionTimeline posTimeline = LogicCharacter.GetPositionTimeline();
			float oldTime = Time.time - Time.deltaTime;
			Vector3 oldPosition = posTimeline.GetValue(oldTime);
			Vector3 newPosition = posTimeline.GetValue(Time.time);
			transform.position = newPosition;
			Velocity = (newPosition - oldPosition); 
			if (Velocity != Vector3.zero)
			{
				Velocity /= Time.deltaTime;
				_Transform.rotation = Quaternion.LookRotation(Velocity.normalized);
			}
			
			// Animations are timed for scale = 1. // If we divide the speed by the scale we'll play run animations slower/faster if they are bigger/smaller.
			float sizeScaleFactor = _Transform.localScale.x * _Animator.humanScale;
			float velocityMagnitude = Velocity.magnitude / sizeScaleFactor;
			_Animator.SetFloat("Speed", velocityMagnitude);
		}
	}

	public void PlayAnimation(AnimationEnum animationEnum, float stateTime, float fadeTime = 0.1f)
	{
		float normalizedTime = (stateTime / Animations.Get(animationEnum).Lenght) % 1f;
		_Animator.CrossFade(Animations.Get(animationEnum).Hash, Mathf.Max(0f, fadeTime - normalizedTime), 0, normalizedTime);
        //Debug.Log("startTime: " +stateTime + " normalized: " + normalizedTime + " fade: " + fadeTime + "  " + Time.time);
	}

	void DoIdle()
	{
		
	}

	void DoSleep()
	{
		AnimationEnum animEnum = AnimationEnum.Death;
		if (_CurrentStateInfo.nameHash != Animations.Get(animEnum).Hash &&
		    _NextStateInfo.nameHash != Animations.Get(animEnum).Hash)
		{
			PlayAnimation(animEnum, _StateTimeline.GetCurrentKeyframeTime(Time.time));
		}
	}

	void DoMining()
	{
		AnimationEnum animEnum = AnimationEnum.Mine;
		if (_CurrentStateInfo.nameHash != Animations.Get(animEnum).Hash &&
		    _NextStateInfo.nameHash != Animations.Get(animEnum).Hash)
		{
			PlayAnimation(animEnum, _StateTimeline.GetCurrentKeyframeTime(Time.time));
		}
	}

	void DoMovement()
	{

	}
}









