using UnityEngine;
using System.Collections;
using System;
using Bronk;

public enum AnimationEnum
{
	Laugh,
	Enraged,
	Death,
}

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

		//_Animator.humanScale

		//Vector3 scale = Vector3.one * (1f / _Animator.humanScale);
		//_Transform.localScale = scale;

		// Animations are timed for scale = 1. 
		// If we divide the speed by the scale we'll play run animations slower/faster if they are bigger/smaller.
		float sizeScaleFactor = _Transform.localScale.x * _Animator.humanScale;
		float velocityMagnitude = Velocity.magnitude / sizeScaleFactor;
		_Animator.SetFloat("Speed", velocityMagnitude);
	}

	public void PlayAnimation(AnimationEnum animation)
	{


		_Animator.Play(Animator.StringToHash(GetStateName(animation)));
		//Debug.Log("Play animation: " + animation.ToString() + "  " + Time.time);
	}

	public static string GetStateName(AnimationEnum animationEnum)
	{
		switch (animationEnum)
		{
			case AnimationEnum.Death:
				return "Animations.Death";
			case AnimationEnum.Enraged:
				return "Animations.Enraged";
			case AnimationEnum.Laugh:
				return "Animations.Laugh";
			default:
				return string.Empty;
		}
	}

    internal void updateState(Bronk.Ant ant)
    {
        var timelines = ant.getActiveTimelines();
        foreach (var item in timelines)
        {
            if (item is WalkTimeline)
            {
                var walktimeline = item as WalkTimeline;
                transform.position = walktimeline.getPosition(Time.time);
            }
        }
    }
}
