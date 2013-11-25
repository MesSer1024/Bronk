using UnityEngine;
using System.Collections;
using Bronk;

public class GameCamera : MonoBehaviour
{
	public Vector3 Offset = new Vector3 (-3, 7.5f, -3);
	public Vector2 Position2D;
	public float StopTimer = 0.5f;
	public float Sensitivity = 0.005f;
	public float FingerDeltaThreshold = 600;
	private IInteractable _SemiHighlightEntity;
	private Vector2 _Velocity;
	private float _TapStartTime;
	private float _LastPanTime;
	private float _LastPanTimePCX;
	private bool _IsTapping = false;

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

			Vector2 delta2D = _Velocity * Sensitivity * Time.deltaTime;
			Vector3 transformedDelta = Quaternion.LookRotation (-new Vector3 (Offset.x, 0, Offset.z)) * new Vector3 (delta2D.x, 0, delta2D.y);
			Position2D += new Vector2 (transformedDelta.x, transformedDelta.z);
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
		} else if (_Velocity.x != 0) {
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
				if (newHits [i].collider.gameObject.GetComponent (typeof(IInteractable)) != null) {
					if (distance < closestDistance) {
						closestTarget = i;
						closestDistance = distance;
					}
				}
			}

			if (closestTarget != -1) {

				IInteractable interactable = newHits [closestTarget].collider.gameObject.GetComponent (typeof(IInteractable)) as IInteractable;
				interactable.Interact ();
			}
		}
	}

	void UpdatePadInput ()
	{
		if (Input.touchCount == 2) {
			_IsTapping = false;
			if (_SemiHighlightEntity != null) {
				_SemiHighlightEntity.SemiSelect (false);
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
			if (finger.phase == TouchPhase.Began)
				_IsTapping = true;
			if (_IsTapping && (finger.phase == TouchPhase.Began || finger.phase == TouchPhase.Moved)) {
				_Velocity = Vector2.zero;
				Ray ray = camera.ScreenPointToRay (finger.position);

				RaycastHit[] hits = Physics.RaycastAll (ray);

				float closestDistance = float.MaxValue;
				int closestTarget = -1;
				for (int i = 0; i < hits.Length; i++) {
					if (hits [i].collider.gameObject.GetComponent (typeof(IInteractable)) != null) {
						
						float distance = hits [i].distance;
						if (distance < closestDistance) {
							closestTarget = i;
							closestDistance = distance;
						}
					}
				}

				if (closestTarget != -1) {
					if (_SemiHighlightEntity != null) {
						_SemiHighlightEntity.SemiSelect (false);
					} else {
						_TapStartTime = Time.time;
					}
					_SemiHighlightEntity = hits [closestTarget].collider.gameObject.GetComponent (typeof(IInteractable)) as IInteractable;
					_SemiHighlightEntity.SemiSelect (true);

				}

			} else if (_IsTapping && finger.phase == TouchPhase.Ended) {
				if (_SemiHighlightEntity != null) {
					_SemiHighlightEntity.SemiSelect (false);
					_SemiHighlightEntity = null;
					if (Time.time - _TapStartTime > 0.003f) {
						Ray ray = camera.ScreenPointToRay (finger.position);

						RaycastHit[] hits = Physics.RaycastAll (ray);
						float closestDistance = float.MaxValue;
						int closestTarget = -1;
						for (int i = 0; i < hits.Length; i++) {

							if (hits [i].collider.gameObject.GetComponent (typeof(IInteractable)) != null) {

								float distance = hits [i].distance;
								if (distance < closestDistance) {
									closestTarget = i;
									closestDistance = distance;
								}
							}
						}

						if (closestTarget != -1) {
							IInteractable interactable = hits [closestTarget].collider.gameObject.GetComponent (typeof(IInteractable)) as IInteractable;
							interactable.Interact ();
						}
					}
				}
			}
		} else if (Input.touchCount == 0) {
			_IsTapping = false;
			if (_SemiHighlightEntity != null) {
				_SemiHighlightEntity.SemiSelect (false);
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

	public void SetPosition (Vector2 position)
	{
		Position2D = position;
		UpdatePosition ();
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
