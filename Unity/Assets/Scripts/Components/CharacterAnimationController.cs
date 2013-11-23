using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Animator))]
public class CharacterAnimationController : MonoBehaviour 
{
	Animator _Animator;
	Transform _Transform;

	public DummyPlayer LogicCharacter;

	private Vector3 Velocity;

	private DummyTimeline _StateTimeline;
	private AnimatorStateInfo _CurrentStateInfo;
	private AnimatorStateInfo _NextStateInfo;

	void Awake () 
	{
		_Animator = GetComponent<Animator>();
		_Transform = GetComponent<Transform>();
	}

	void Update () 
	{
		if (LogicCharacter != null)
		{
			_StateTimeline = LogicCharacter.GetStateTimeline();
			switch (_StateTimeline.DummyState)
			{
			case DummyState.Idle: 
				DoIdle();
				break;
			case DummyState.Mining:
				DoMining();
				break;
			case DummyState.Movement:
				DoMovement();
				break;
			}
		}

		_CurrentStateInfo = _Animator.GetCurrentAnimatorStateInfo(0);
		_NextStateInfo = _Animator.GetNextAnimatorStateInfo(0);

		/*
		if (Velocity != Vector3.zero)
			_Transform.rotation = Quaternion.LookRotation(Velocity.normalized);

		// Animations are timed for scale = 1. // If we divide the speed by the scale we'll play run animations slower/faster if they are bigger/smaller.
		float sizeScaleFactor = _Transform.localScale.x * _Animator.humanScale;
		float velocityMagnitude = Velocity.magnitude / sizeScaleFactor;
		_Animator.SetFloat("Speed", velocityMagnitude);
		*/
	}

	public void PlayAnimation(AnimationEnum animationEnum)
	{
		_Animator.Play(Animations.GetAnimationHash(animationEnum));
		//Debug.Log("Play animation: " + animation.ToString() + "  " + Time.time);
	}

	void DoIdle()
	{
		
	}

	void DoMining()
	{
		if (_CurrentStateInfo.nameHash != Animations.GetAnimationHash(AnimationEnum.Laugh) ||
		    _NextStateInfo.nameHash != Animations.GetAnimationHash(AnimationEnum.Laugh))
			PlayAnimation(AnimationEnum.Laugh);
	}

	void DoMovement()
	{
		Debug.Log(_StateTimeline);
		float lerp = (Time.time - _StateTimeline.StartTime) / (_StateTimeline.EndTime - _StateTimeline.StartTime);
		Vector3 position = Vector3.Lerp(_StateTimeline.StartPos, _StateTimeline.EndPos, lerp);
		_Transform.position = position;
	}
}









