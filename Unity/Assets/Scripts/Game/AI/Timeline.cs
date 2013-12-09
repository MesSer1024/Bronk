using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct KeyframeItem<T>
{
	public T Value;
	public float Time;

	public KeyframeItem (T value, float time)
	{
		Value = value;
		Time = time;
	}
}

public abstract class Timeline<Keyframe, Value> : Bronk.ITimeline
{
	private List<KeyframeItem<Keyframe>> _Keyframes = new List<KeyframeItem<Keyframe>> ();
	protected Func<Keyframe, Keyframe, float, Value> _InterpolationFunc;

	public abstract TimelineType Type { get; }

	protected static T Create<T, K,V> (Func<K, K, float, V> func) where T : Timeline<K, V>, new()
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
		return _InterpolationFunc (default(Keyframe), _Keyframes [_Keyframes.Count - 1].Value, 1);
	}

	public void AddKeyframe (float time, Keyframe frame)
	{
		if (_Keyframes.Count > 0 && _Keyframes [_Keyframes.Count - 1].Time > time)
			throw new ArgumentException ("time needs to be later than last keyframe: " + time + " < " + _Keyframes[_Keyframes.Count - 1].Time);
		_Keyframes.Add (new KeyframeItem<Keyframe> (frame, time));
	}

	public void removeKeyframesInFuture (float currentTime)
	{
		int lastIndex = _Keyframes.Count;
		for (int i = 0; i < _Keyframes.Count; i++) {
			if (currentTime > _Keyframes [i].Time) {
				lastIndex = i + 1;
			}
		}

		if (lastIndex < _Keyframes.Count) {
			_Keyframes.RemoveRange (lastIndex, _Keyframes.Count - lastIndex);
		}
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

	public float GetCurrentKeyframeTime (float time)
	{
		if (_Keyframes.Count == 0)
			return 0;
		for (int i = 0; i < _Keyframes.Count; i++) {
			if (time < _Keyframes [i].Time && i > 0) {
				return time - _Keyframes [i - 1].Time;
			}
		}
		return Mathf.Max (0, time - _Keyframes [_Keyframes.Count - 1].Time);
	}

	public bool HasFuture(float time)
	{
		if (_Keyframes.Count == 0)
			return false;
		return _Keyframes [_Keyframes.Count - 1].Time >= time;
	}

	public bool HasNewValue (float prevTime, float currentTime)
	{
		if (_Keyframes.Count == 0)
			return false;
		for (int i = 0; i < _Keyframes.Count; i++) {
			if (prevTime < _Keyframes [i].Time && _Keyframes [i].Time <= currentTime) {
				return true;
			}
		}
		return false;
	}

	public void CopyFrom (Bronk.ITimeline timeline)
	{
		if (timeline.Type == this.Type)
			CopyFrom (timeline as Timeline<Keyframe,Value>);
	}

	public void CopyFrom (Timeline<Keyframe, Value> timeline)
	{
		_Keyframes.Clear ();
		_Keyframes.AddRange (timeline._Keyframes);
	}
}
