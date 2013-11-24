using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public struct AnimationData
{
	public int Hash;
	public float Lenght;
	
	public AnimationData(string animation, float animationLenght)
	{
		Hash = Animator.StringToHash(animation);
		Lenght = animationLenght;
	}
}

public enum AnimationEnum
{
	Laugh,
	Enraged,
	Death,
	Mine,
}

public static class Animations
{
	private static Dictionary<AnimationEnum, AnimationData> _AnimationDict;

	private static void Initialize()
	{
		_AnimationDict = new Dictionary<AnimationEnum, AnimationData>();		
		_AnimationDict.Add(AnimationEnum.Laugh, new AnimationData("Animations.Laugh", 2.667f));
		_AnimationDict.Add(AnimationEnum.Enraged, new AnimationData("Animations.Enraged", 2.667f));
		_AnimationDict.Add(AnimationEnum.Death, new AnimationData("Animations.Death", 0.867f));
		_AnimationDict.Add(AnimationEnum.Mine, new AnimationData("Animations.Mine", 1f));
	}
	public static AnimationData Get(AnimationEnum animEnum)
	{
		if (_AnimationDict == null)
			Initialize();
		return _AnimationDict[animEnum];
	}
}

