using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterAnimationController))]
public class DummyPlayer : MonoBehaviour 
{
	float _ActionTimer;
	Vector3 _TargetPos;

	public float MoveSpeed = 10;
	private CharacterAnimationController _Controller;
	private Vector3 _LastPosition;

	public float CircleSpeed = 1;
	public float CircleSize = 5;

	public bool RandomActions;
	private bool _ReachedTarget;
	private bool _PlayedAnimation;
	private int _Animation;

	void Awake()
	{
		_Controller = GetComponent<CharacterAnimationController>();
		_LastPosition = transform.position;
	}

	// Update is called once per frame
	void Update () 
	{
		if (RandomActions)
		{
			_ActionTimer -= Time.deltaTime;
			if (_ActionTimer < 0)
			{
				_TargetPos = new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
				_ActionTimer = Random.Range(2f, 5f);
				_ReachedTarget = false;
				_PlayedAnimation = false;
			}
			if (_ReachedTarget == false)
			{				
				Vector3 newPosition = Vector3.MoveTowards(transform.position, _TargetPos, Time.deltaTime * MoveSpeed);
				if (newPosition == _LastPosition)
					_ReachedTarget = true;
				_Controller.Velocity = (newPosition - _LastPosition) / Time.deltaTime;
				_LastPosition = newPosition;
				transform.position = newPosition;


			}
			else
			{
				if (_PlayedAnimation == false)
				{
					_PlayedAnimation = true;
					_Controller.PlayAnimation(AnimationEnum.Laugh);
				}
			}

		}
		else
		{
			float x = Mathf.Cos(Time.time * CircleSpeed) * CircleSize;
			float z = Mathf.Sin(Time.time * CircleSpeed) * CircleSize;

			_TargetPos = new Vector3(x, 0, z);
			_LastPosition = transform.position;
			transform.position = _TargetPos;
			_Controller.Velocity = (_TargetPos - _LastPosition) / Time.deltaTime;

		}

	}
}
