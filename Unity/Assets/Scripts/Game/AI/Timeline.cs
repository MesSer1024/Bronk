using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct KeyframeItem<T>
{
	public T Value;
	public float Time;
}

public class Timeline<Keyframe, Value>
{
	private List<KeyframeItem<Keyframe>> _Keyframes = new List<KeyframeItem<Keyframe>> ();
	protected Func<Keyframe, Keyframe, float, Value> _InterpolationFunc;


	protected static T Create<T, Keyframe,Value>(Func<Keyframe, Keyframe, float, Value> func) where T : Timeline<Keyframe, Value>, new()
	{
		T timeline = new T ();
		timeline._InterpolationFunc = func;
		return timeline;
	}
	public Value GetValue (float time)
	{
		if (_Keyframes.Count == 0)
			return default(Value);
		for (int i = 0; i < _Keyframes.Count; i++) {
			if (time < _Keyframes [i].Time) {
				float t;
				Keyframe value1;
				if (i > 0) {
					value1 = _Keyframes [i - 1].Value;
					t = (time - _Keyframes [i - 1].Time) / (_Keyframes [i].Time - _Keyframes [i - 1].Time);
				} else {
					value1 = default(Keyframe);
					t = 1;
				}
				Keyframe value2 = _Keyframes [i].Value;

				return _InterpolationFunc (value1, value2, t);
			}
		}
		return _InterpolationFunc (_Keyframes [0].Value, default(Keyframe), 0);
	}

	public float GetNextKeyTime (float time)
	{
		if (_Keyframes.Count == 0)
			return time;
		for (int i = 0; i < _Keyframes.Count; i++) {
			if (time < _Keyframes [i].Time) {
				return _Keyframes [i].Time;
			}
		}
		return _Keyframes [_Keyframes.Count - 1].Time;
	}

	public Value GetNextValue (float time)
	{
		if (_Keyframes.Count == 0)
			return default(Value);
		for (int i = 0; i < _Keyframes.Count; i++) {
			if (time < _Keyframes [i].Time) {
				return _InterpolationFunc (default(Keyframe), _Keyframes [i].Value, 1);
			}
		}
		return _InterpolationFunc (default(Keyframe), _Keyframes [_Keyframes.Count - 1].Value, 1);
	}

	public float GetCurrentKeyframeTime(float time)
	{
		if (_Keyframes.Count == 0)
			return 0;
		for (int i = 0; i < _Keyframes.Count; i++) {
			if (time < _Keyframes [i].Time && i > 0) {
				return time - _Keyframes [i - 1].Time;
			}
		}
		return Mathf.Max(0, time - _Keyframes [_Keyframes.Count - 1].Time);
	}
}
