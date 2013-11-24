using UnityEngine;
using System.Collections;
using System;
using Bronk;

[RequireComponent(typeof(Animator))]
public class CharacterAnimationController : MonoBehaviour 
{
	Animator _Animator;
	Transform _Transform;

	[NonSerialized]
	public Vector3 Velocity;

	void Awake () 
	{
		_Animator = GetComponent<Animator>();
		_Transform = GetComponent<Transform>();
	}

	void Update () 
	{
		if (Velocity != Vector3.zero)
			_Transform.rotation = Quaternion.LookRotation(Velocity.normalized);

		// Animations are timed for scale = 1. // If we divide the speed by the scale we'll play run animations slower/faster if they are bigger/smaller.
		float sizeScaleFactor = _Transform.localScale.x * _Animator.humanScale;
		float velocityMagnitude = Velocity.magnitude / sizeScaleFactor;
		_Animator.SetFloat("Speed", velocityMagnitude);
	}

	public void PlayAnimation(AnimationEnum animationEnum)
	{
		_Animator.Play(Animations.GetAnimationHash(animationEnum));
		//Debug.Log("Play animation: " + animation.ToString() + "  " + Time.time);
	}

    internal void updateState(Bronk.Ant ant)
    {
        var timelines = ant.getActiveTimelines();

        foreach (var item in timelines)
        {
            if (item is WalkTimeline)
            {
                var walktimeline = item as WalkTimeline;
                Velocity = transform.position - walktimeline.getPosition(Time.time);
                transform.position = walktimeline.getPosition(Time.time);
            }
            else if (item is MiningTimeline)
            {
                var miningtimeline = item as MiningTimeline;
                PlayAnimation(AnimationEnum.Laugh);
            }
            else
            {
                PlayAnimation(AnimationEnum.Death);
            }
        }
    }
}
