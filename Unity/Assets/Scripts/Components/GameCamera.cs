using UnityEngine;
using System.Collections;
using Bronk;

public class GameCamera : MonoBehaviour
{
	public Vector3 Offset = new Vector3 (0, 15, -5);
	public Vector2 Position2D;
	public float StopTimer = 0.5f;
	public float Sensitivity = 0.01f;
	public float FingerDeltaThreshold = 600;
	private CubeLogic _SemiHighlightEntity;
	private Vector2 _Velocity;
	private float _TapStartTime;
	private float _LastPanTime;
	private float _LastPanTimePCX;

	void Awake ()
	{
		Application.targetFrameRate = 60;
		UpdatePosition ();
	}

	void LateUpdate ()
	{
		#if UNITY_IPHONE && !UNITY_EDITOR
		UpdatePadInput ();
		#else
		UpdatePCInput ();
		#endif
		if (_Velocity != Vector2.zero) {
			Position2D += _Velocity * Sensitivity * Time.deltaTime;
			UpdatePosition ();		
		}
	}

	void UpdatePCInput ()
	{
		if (Input.GetKey (KeyCode.LeftArrow) || Input.GetKey (KeyCode.A)) {
			_LastPanTimePCX = Time.time;
			_Velocity.x = -Screen.width / Sensitivity / 30;
		}
		if (Input.GetKey (KeyCode.RightArrow) || Input.GetKey (KeyCode.D)) {
			_LastPanTimePCX = Time.time;
			_Velocity.x = Screen.width / Sensitivity / 30;
		} else if (_Velocity.y != 0) {
			_Velocity.x = Mathf.Lerp (_Velocity.x, 0, Mathf.Clamp01 ((Time.time - _LastPanTimePCX) / StopTimer));
		}

		if (Input.GetKey (KeyCode.UpArrow) || Input.GetKey (KeyCode.W)) {
			_LastPanTime = Time.time;
			_Velocity.y = Screen.height / Sensitivity / 30;
		} else if (Input.GetKey (KeyCode.DownArrow) || Input.GetKey (KeyCode.S)) {
			_LastPanTime = Time.time;
			_Velocity.y = -Screen.height / Sensitivity / 30;
		} else if (_Velocity.y != 0) {
			_Velocity.y = Mathf.Lerp (_Velocity.y, 0, Mathf.Clamp01 ((Time.time - _LastPanTime) / StopTimer));
		}

		if (Input.GetMouseButtonDown (0)) {
			Ray ray = camera.ScreenPointToRay (Input.mousePosition);

			RaycastHit[] newHits = Physics.SphereCastAll (ray, 0.5f);
			float closestDistance = float.MaxValue;
			int closestTarget = -1;
			for (int i = 0; i < newHits.Length; i++) {
				float distance = GetDistPointToLine (ray.origin, ray.direction, newHits [i].transform.position); 
				if (newHits [i].collider.gameObject.GetComponent<CubeLogic> () != null) {
					if (distance < closestDistance) {
						closestTarget = i;
						closestDistance = distance;
					}
				}
			}

			if (closestTarget != -1) {

				CubeLogic cube = newHits [closestTarget].collider.gameObject.GetComponent<CubeLogic> ();
				MessageManager.ExecuteMessage (new CubeClickedMessage ("cube", cube));
			}
		}
	}

	void UpdatePadInput ()
	{
		if (Input.touchCount == 2) {
			if (_SemiHighlightEntity != null) {
				MessageManager.ExecuteMessage (new CubeSemiSelectedMessage ("cube", _SemiHighlightEntity, false));
				_SemiHighlightEntity = null;
			}
			Touch finger1 = Input.touches [0];
			Touch finger2 = Input.touches [1];
			float distance = Vector2.Distance (finger1.position, finger2.position);
			if (distance < FingerDeltaThreshold) {
				_LastPanTime = Time.time;
				if (finger1.phase == TouchPhase.Moved || finger2.phase == TouchPhase.Moved || finger1.phase == TouchPhase.Began || finger2.phase == TouchPhase.Began) {
					Vector2 deltaPos = (finger1.deltaPosition + finger2.deltaPosition) / 2;
					if (deltaPos.magnitude > 10)
						_Velocity = -deltaPos / ((finger1.deltaTime + finger2.deltaTime) / 2);
					else
						_Velocity = Vector2.zero;
				} else if (finger1.phase == TouchPhase.Stationary && finger2.phase == TouchPhase.Stationary) {
					_Velocity = Vector2.zero;
				}
			}
		} else if (Input.touchCount == 1) {
			Touch finger = Input.touches [0];
			if (finger.phase == TouchPhase.Began || finger.phase == TouchPhase.Moved) {
				_Velocity = Vector2.zero;
				Ray ray = camera.ScreenPointToRay (finger.position);

				RaycastHit[] hits = Physics.SphereCastAll (ray, 0.5f);

				float closestDistance = float.MaxValue;
				int closestTarget = -1;
				for (int i = 0; i < hits.Length; i++) {
					if (hits [i].collider.gameObject.GetComponent<CubeLogic> () != null) {
						
						float distance = GetDistPointToLine (ray.origin, ray.direction, hits [i].point);
						if (distance < closestDistance) {
							closestTarget = i;
							closestDistance = distance;
						}
					}
				}

				if (closestTarget != -1) {
					if (_SemiHighlightEntity != null) {
						MessageManager.ExecuteMessage (new CubeSemiSelectedMessage ("cube", _SemiHighlightEntity, false));
					} else {
						_TapStartTime = Time.time;
					}
					_SemiHighlightEntity = hits [closestTarget].collider.gameObject.GetComponent<CubeLogic> ();
					MessageManager.ExecuteMessage (new CubeSemiSelectedMessage ("cube", _SemiHighlightEntity, true));
				}

			} else if (finger.phase == TouchPhase.Ended) {
				if (_SemiHighlightEntity != null) {
					MessageManager.ExecuteMessage (new CubeSemiSelectedMessage ("cube", _SemiHighlightEntity, false));
					_SemiHighlightEntity = null;
					if (Time.time - _TapStartTime > 0.07f) {
						Ray ray = camera.ScreenPointToRay (finger.position);

						RaycastHit[] hits = Physics.SphereCastAll (ray, 0.5f);
						float closestDistance = float.MaxValue;
						int closestTarget = -1;
						for (int i = 0; i < hits.Length; i++) {

							if (hits [i].collider.gameObject.GetComponent<CubeLogic> () != null) {

								float distance = GetDistPointToLine (ray.origin, ray.direction, hits [i].point);
								if (distance < closestDistance) {
									closestTarget = i;
									closestDistance = distance;
								}
							}
						}

						if (closestTarget != -1) {
							CubeLogic cube = hits [closestTarget].collider.gameObject.GetComponent<CubeLogic> ();
							MessageManager.ExecuteMessage (new CubeClickedMessage ("cube", cube));
						}
					}
				}
			}
		} else if (Input.touchCount == 0) {
			if (_SemiHighlightEntity != null) {
				MessageManager.ExecuteMessage (new CubeSemiSelectedMessage ("cube", _SemiHighlightEntity, false));
				_SemiHighlightEntity = null;
			}
			if (_Velocity != Vector2.zero) {
				_Velocity = Vector2.Lerp (_Velocity, Vector2.zero, Mathf.Clamp01 ((Time.time - _LastPanTime) / StopTimer));
			}
		}
	}

	void UpdatePosition ()
	{
		Vector3 lookatPos;
		Vector3 cameraPos = GetCameraPos (Position2D, out lookatPos);
		transform.position = cameraPos;
		transform.LookAt (lookatPos);
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

	void GetTileIndices (Ray ray, out int endXIndex, out int endYIndex)
	{
		endXIndex = 0;
		endYIndex = 0;
	}
}
