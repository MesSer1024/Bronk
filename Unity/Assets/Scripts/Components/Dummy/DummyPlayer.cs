using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum DummyState
{
	Idle,
	Movement,
	Mining
}

public class DummyTimeline
{
	public DummyState DummyState;
	public float StartTime;
	public float EndTime;
	public Vector3 StartPos;
	public Vector3 EndPos;
}

public class DummyPlayer : MonoBehaviour 
{
	float _ActionTimer;
	Vector3 _TargetPos;

	public float MoveSpeed = 10;
	private Vector3 _LastPosition;

	public float CircleSpeed = 1;
	public float CircleSize = 5;

	public bool UseTimeline;
	private bool _ReachedTarget;
	private bool _PlayedAnimation;

	public DummyState DummyState;

	DummyTimeline _Timeline;

	void Awake()
	{
		_LastPosition = transform.position;
		_Timeline = new DummyTimeline();
		SetTimeline(DummyState.Idle);
	}

	void Start()
	{		
		CharacterAnimationController controller = GetComponent<CharacterAnimationController>();
		controller.LogicCharacter = this;
	}

	public DummyTimeline GetStateTimeline()
	{
		return _Timeline;
	}

	void SetTimeline(DummyState state)
	{
		_Timeline.DummyState = state;
		_Timeline.StartTime = Time.time;

		if (state == DummyState.Movement)
		{
			_Timeline.StartPos = transform.position;
			_Timeline.EndPos = new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
			_Timeline.EndTime = Time.time + Vector3.Distance(_Timeline.StartPos, _Timeline.EndPos) * MoveSpeed;
		}
		if (state == DummyState.Idle)
		{
			_Timeline.EndTime = Time.time + 5f;
		}
		if (state == DummyState.Mining)
		{
			_Timeline.EndTime = Time.time + 3f;
		}
	}

	DummyState GetRandomState()
	{
		int random = Random.Range(0, 3);
		if (random == 0)
			return DummyState.Idle;
		else if (random == 1)
			return DummyState.Movement;
		else 
			return DummyState.Mining;
	}

	// Update is called once per frame
	void Update () 
	{
		if (UseTimeline)
		{
			// New timeline
			if (Time.time > _Timeline.EndTime)
				SetTimeline(GetRandomState());

			DummyState = _Timeline.DummyState;
		}
		else
		{
			float x = Mathf.Cos(Time.time * CircleSpeed) * CircleSize;
			float z = Mathf.Sin(Time.time * CircleSpeed) * CircleSize;

			_TargetPos = new Vector3(x, 0, z);
			_LastPosition = transform.position;
			transform.position = _TargetPos;
		}
	}
}
