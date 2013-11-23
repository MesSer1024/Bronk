using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public struct AnimationData
{
	public int AnimationHash;
	public float AnimationLenght;
	
	public AnimationData(string animation, float animationLenght)
	{
		AnimationHash = Animator.StringToHash(animation);
		AnimationLenght = animationLenght;
	}
}

public enum AnimationEnum
{
	Laugh,
	Enraged,
	Death,
	AttackSlash,
}

public static class Animations
{
	private static Dictionary<AnimationEnum, AnimationData> _AnimationDict;
	
	public static AnimationData GetAnimationData(AnimationEnum animationEnum)
	{
		if (_AnimationDict == null)
			Initialize();
		return _AnimationDict[animationEnum];
	}
	
	public static int GetAnimationHash(AnimationEnum animationEnum)
	{
		if (_AnimationDict == null)
			Initialize();
		return _AnimationDict[animationEnum].AnimationHash;
	}
	
	private static void Initialize()
	{
		_AnimationDict = new Dictionary<AnimationEnum, AnimationData>();		
		_AnimationDict.Add(AnimationEnum.Laugh, new AnimationData("Animations.Laugh", 2.667f));
		_AnimationDict.Add(AnimationEnum.Enraged, new AnimationData("Animations.Enraged", 2.667f));
		_AnimationDict.Add(AnimationEnum.Death, new AnimationData("Animations.Death", 0.867f));
		_AnimationDict.Add(AnimationEnum.AttackSlash, new AnimationData("Animations.AttackSlash", 1f));
	}
}

