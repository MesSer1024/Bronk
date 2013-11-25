using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Bronk;


public class DummyPlayer : MonoBehaviour 
{
	float _ActionTimer;
	Vector3 _TargetPos;

	public float MoveSpeed = 10;
	private Vector3 _LastPosition;

	public float CircleSpeed = 1;
	public float CircleSize = 5;

	public bool UseTimeline;

	AntStateTimeline _AntStateTimeline;
	PositionTimeline _PositionTimeline;
	public GameEntity.States CurrentState;

	void Awake()
	{
		_LastPosition = transform.position;
		_AntStateTimeline = AntStateTimeline.Create();
		_PositionTimeline = PositionTimeline.Create();
		_PositionTimeline.AddKeyframe(Time.time, transform.position);

		AddKeyframe(GameEntity.States.Idle);
	}

	void Start()
	{		
		CharacterAnimationController controller = GetComponent<CharacterAnimationController>();
		controller.LogicCharacter = this;
	}
	
	public AntStateTimeline GetStateTimeline()
	{
		return _AntStateTimeline;
	}
	
	public PositionTimeline GetPositionTimeline()
	{
		return _PositionTimeline;
	}

	void AddKeyframe(GameEntity.States state)
	{
		float startTime = Time.time + 2f;
		_AntStateTimeline.AddKeyframe(startTime, state);
		if (state == GameEntity.States.Move)
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

	[ContextMenu("prnt")]
	void PrintStateTimeline()
	{
		Debug.Log("Current: " + _AntStateTimeline.GetValue(Time.time) + " Next: " + _AntStateTimeline.GetNextValue(Time.time) + " Time: " + Time.time);
	}
	

	GameEntity.States GetRandomState()
	{
		int random = Random.Range(0, 3);
		if (random == 0)
			return GameEntity.States.Idle;
		else if (random == 1)
			return GameEntity.States.Move;
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
				AddKeyframe(GetRandomState());

			CurrentState = _AntStateTimeline.GetValue(Time.time);
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
