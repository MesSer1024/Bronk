using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour
{
	public Vector3 Offset = new Vector3 (0, 15, 5);
	public Vector2 Position2D;
	public float StopTimer = 0.5f;
	public float Sensitivity = 1;
	public float FingerDeltaThreshold = 600;
	private Vector2 _Velocity;
	private float _LastPanTime;
	private RaycastHit[] _TapTargets;

	void Awake ()
	{
		Application.targetFrameRate = 60;
	}

	void LateUpdate ()
	{
		Vector3 lookatPos;
		Vector3 cameraPos = GetCameraPos (Position2D, out lookatPos);
		transform.position = cameraPos;
		transform.LookAt (lookatPos);
	

		if (Input.touchCount == 2) {
			Touch finger1 = Input.touches [0];
			Touch finger2 = Input.touches [1];
			float distance = Vector2.Distance (finger1.position, finger2.position);
			if (distance < FingerDeltaThreshold) {
				_LastPanTime = Time.time;
				if (finger1.phase == TouchPhase.Moved || finger2.phase == TouchPhase.Moved || finger1.phase == TouchPhase.Began || finger2.phase == TouchPhase.Began) {
					Vector2 deltaPos = (finger1.deltaPosition + finger2.deltaPosition) / 2;
					if (deltaPos.magnitude > 10)
						_Velocity = deltaPos * (finger1.deltaTime + finger2.deltaTime) / 2;
					else
						_Velocity = Vector2.zero;
				} else if (finger1.phase == TouchPhase.Stationary && finger2.phase == TouchPhase.Stationary) {
					_Velocity = Vector2.zero;
				}
			}
		} else if (Input.touchCount == 1) {
			Touch finger = Input.touches [0];
			if (finger.phase == TouchPhase.Began) {
				_Velocity = Vector2.zero;
				Ray ray = camera.ScreenPointToRay (finger.position);
				
				_TapTargets = Physics.SphereCastAll (ray, 0.5f);

			} else if (finger.phase == TouchPhase.Ended) {
				Ray ray = camera.ScreenPointToRay (finger.position);

				if (_TapTargets != null && _TapTargets.Length > 0) {
					RaycastHit[] newHits = Physics.SphereCastAll (ray, 0.5f);
					float closestDistance = float.MaxValue;
					int closestTarget = -1;
					for (int i = 0; i < newHits.Length; i++) {
						bool hitFirst = false;
						for (int j = 0; j < _TapTargets.Length; j++) {
							if (_TapTargets [j].collider == newHits [j].collider) {
								hitFirst = true;
								break;
							}
						}
						if (hitFirst) {
							float distance = GetDistPointToLine (ray.origin, ray.direction, newHits [i].point);
							if (distance < closestDistance) {
								closestTarget = i;
								closestDistance = distance;
							}
						}
					}

					if (closestTarget != -1) {
						Debug.Log ("hit " + newHits [closestTarget].collider.gameObject, newHits [closestTarget].collider.gameObject);
					}
				}
				_TapTargets = null;
			}
		} else if (Input.touchCount == 0) {
			if (_Velocity != Vector2.zero) {
				_Velocity = Vector2.Lerp (_Velocity, Vector2.zero, Mathf.Clamp01 ((Time.time - _LastPanTime) / StopTimer));
			}
			_TapTargets = null;
		}

		Position2D += _Velocity * Sensitivity * Time.deltaTime;
	}

	static public float GetDistPointToLine (Vector3 origin, Vector3 direction, Vector3 point)
	{

		Vector3 point2origin = origin - point;

		Vector3 point2closestPointOnLine = point2origin - Vector3.Dot (point2origin, direction) * direction;

		return point2closestPointOnLine.magnitude;

	}

	Vector3 GetCameraPos (Vector2 pos2D, out Vector3 lookatPos)
	{
		lookatPos = new Vector3 (pos2D.x, GetCameraHeight (pos2D), pos2D.y);
		return lookatPos + Offset;
	}

	float GetCameraHeight (Vector2 pos2D)
	{
		return 0f;
	}
}
