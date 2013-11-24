using UnityEngine;
using System.Collections;
using System;

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

		float velocityMagnitude = Velocity.magnitude;
		_Animator.SetFloat("Speed", velocityMagnitude);
	}

	public void PlayAnimation(AnimationEnum animation)
	{


		_Animator.Play(Animator.StringToHash(GetStateName(animation)));
		Debug.Log("Play animation: " + animation.ToString() + "  " + Time.time);
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
}
