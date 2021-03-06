﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Bronk;


public class DummyPlayer : MonoBehaviour
{
	float _ActionTimer;
	Vector3 _TargetPos;

	public float MoveSpeed = 10;

	public float CircleSpeed = 1;
	public float CircleSize = 5;

	public bool UseTimeline;

	AntStateTimeline _AntStateTimeline;
	PositionTimeline _PositionTimeline;
	private CharacterAnimationController _View;
	public GameEntity.States CurrentState;

	void Awake()
	{
		_AntStateTimeline = AntStateTimeline.Create();
		_PositionTimeline = PositionTimeline.Create();
		_PositionTimeline.AddKeyframe(Time.time, transform.position);

		AddKeyframe (new StateData (GameEntity.States.Idle));
	}

	void Start()
	{		
		_View = GetComponent<CharacterAnimationController>();
	}


	void AddKeyframe(StateData state, float startTime = float.NegativeInfinity)
	{
		if (startTime == float.NegativeInfinity)
			startTime = Time.time + 2f;

		_AntStateTimeline.AddKeyframe(startTime, state);
		if (state.State == GameEntity.States.Move)
		{
			Vector3 currentPos = _PositionTimeline.GetValue(startTime);
			Vector3 targetPos = new Vector3(Random.Range(-10, 10),0,Random.Range(-10, 10));
			float duration = (targetPos - currentPos).magnitude / MoveSpeed;
			float endTime = startTime + duration;
			
			_PositionTimeline.AddKeyframe(startTime, currentPos);

			_PositionTimeline.AddKeyframe(endTime, targetPos);
			_AntStateTimeline.AddKeyframe(endTime, state);
		}
	}

	float AddMoveKeyframe(float startTime, Vector3 targetPos)
	{
		Vector3 currentPos = _PositionTimeline.GetValue(startTime);
		float duration = (targetPos - currentPos).magnitude / MoveSpeed;
		float endTime = startTime + duration;
		
		_PositionTimeline.AddKeyframe(startTime, currentPos);		
		_PositionTimeline.AddKeyframe(endTime, targetPos);
		_AntStateTimeline.AddKeyframe(startTime, new StateData( GameEntity.States.Move));

		return endTime;
	}

	[ContextMenu("prnt")]
	void PrintStateTimeline()
	{
		Debug.Log("Current: " + _AntStateTimeline.GetValue(Time.time) + " Next: " + _AntStateTimeline.GetNextValue(Time.time) + " Time: " + Time.time);
	}
	

	GameEntity.States GetRandomState()
	{
		int random = Random.Range(0, 4);
		if (random == 0)
			return GameEntity.States.Idle;
		else if (random == 1)
			return GameEntity.States.Move;
		else if (random == 2)
			return GameEntity.States.Sleep;
		else 
			return GameEntity.States.Mine;
	}

	// Update is called once per frame
	void Update () 
	{
		if (UseTimeline)
		{
			// New timeline
			if (_AntStateTimeline.GetNextKeyTime(Time.time) < Time.time)
			{
				var newState = GetRandomState();
				if (newState == GameEntity.States.Mine)
				{
					float endTime = AddMoveKeyframe(Time.time, DummyWorld.Instance.GetGoldPos());
					AddKeyframe(new StateData(GameEntity.States.Mine), endTime);
				}
				else if (newState == GameEntity.States.Sleep)
				{
					DummyBed dummyBed = DummyWorld.Instance.GetBed();
					float endTime = AddMoveKeyframe(Time.time, dummyBed.transform.position);
					AddKeyframe(new StateData(GameEntity.States.Sleep), endTime);
				}
				else
					AddKeyframe(new StateData(newState));

				UpdateViewTimelines ();			
			}

			CurrentState = _AntStateTimeline.GetValue(Time.time).State;
		}
		else
		{
			float x = Mathf.Cos(Time.time * CircleSpeed) * CircleSize;
			float z = Mathf.Sin(Time.time * CircleSpeed) * CircleSize;

			_TargetPos = new Vector3(x, 0, z);
			transform.position = _TargetPos;
		}
	}

	void UpdateViewTimelines ()
	{
		_View.GetTimeline (TimelineType.AntState).CopyFrom (_AntStateTimeline);
		_View.GetTimeline (TimelineType.Position).CopyFrom (_PositionTimeline);
	}
}
