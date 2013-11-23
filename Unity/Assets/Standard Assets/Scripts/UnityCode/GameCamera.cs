using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour
{
	enum CameraState
	{
		Neutral,
		Select,
	}

	struct SelectStateData
	{
		public int StartXIndex;
		public int StartYIndex;
		public int EndXIndex;
		public int EndYIndex;
	}

	struct NeutralStateData
	{
		public Vector2 Velocity;
		public float LastPanTime;
		public RaycastHit[] TapTargets;
	}

	public Vector3 Offset = new Vector3 (0, 15, 5);
	public Vector2 Position2D;
	public float StopTimer = 0.5f;
	public float Sensitivity = 0.01f;
	public float FingerDeltaThreshold = 600;
	private CameraState _State;
	private NeutralStateData _NeutralData;
	private SelectStateData _SelectData;

	void Awake ()
	{
		Application.targetFrameRate = 60;
		UpdatePosition ();
	}

	void LateUpdate ()
	{
		switch (_State) {
		case CameraState.Neutral:
			UpdateNeutralState (ref _NeutralData);
			break;
		case CameraState.Select:
			UpdateSelectState (ref _SelectData);
			break;
		}
	}

	void UpdateSelectState (ref SelectStateData data)
	{
		if (Input.touchCount == 1) {
			Touch finger = Input.touches [0];

			Ray ray = camera.ScreenPointToRay (finger.position);

			GetTileIndices (ray, out data.EndXIndex, out data.EndYIndex);
		}
	}

	void EnterSelectState (int xIndexStart, int yIndexStart)
	{
		_SelectData = default(SelectStateData);
		_SelectData.StartXIndex = xIndexStart;
		_SelectData.StartYIndex = yIndexStart;
		_SelectData.EndXIndex = xIndexStart;
		_SelectData.EndYIndex = yIndexStart;
		_State = CameraState.Select;
	}

	void EnterNeutralState ()
	{
		_NeutralData = default(NeutralStateData);
		_State = CameraState.Neutral;
	}

	void UpdateNeutralState (ref NeutralStateData data)
	{
		if (Input.touchCount == 2) {
			Touch finger1 = Input.touches [0];
			Touch finger2 = Input.touches [1];
			float distance = Vector2.Distance (finger1.position, finger2.position);
			if (distance < FingerDeltaThreshold) {
				data.LastPanTime = Time.time;
				if (finger1.phase == TouchPhase.Moved || finger2.phase == TouchPhase.Moved || finger1.phase == TouchPhase.Began || finger2.phase == TouchPhase.Began) {
					Vector2 deltaPos = (finger1.deltaPosition + finger2.deltaPosition) / 2;
					if (deltaPos.magnitude > 10)
						data.Velocity = deltaPos / ((finger1.deltaTime + finger2.deltaTime) / 2);
					else
						data.Velocity = Vector2.zero;
				} else if (finger1.phase == TouchPhase.Stationary && finger2.phase == TouchPhase.Stationary) {
					data.Velocity = Vector2.zero;
				}
			}
		} else if (Input.touchCount == 1) {
			Touch finger = Input.touches [0];
			if (finger.phase == TouchPhase.Began) {
				data.Velocity = Vector2.zero;
				Ray ray = camera.ScreenPointToRay (finger.position);

				data.TapTargets = Physics.SphereCastAll (ray, 0.5f);

			} else if (finger.phase == TouchPhase.Ended) {
				Ray ray = camera.ScreenPointToRay (finger.position);

				if (data.TapTargets != null && data.TapTargets.Length > 0) {
					RaycastHit[] newHits = Physics.SphereCastAll (ray, 0.5f);
					float closestDistance = float.MaxValue;
					int closestTarget = -1;
					for (int i = 0; i < newHits.Length; i++) {
						bool hitFirst = false;
						for (int j = 0; j < data.TapTargets.Length; j++) {
							if (data.TapTargets [j].collider == newHits [j].collider) {
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
				data.TapTargets = null;
			}
		} else if (Input.touchCount == 0) {
			if (data.Velocity != Vector2.zero) {
				data.Velocity = Vector2.Lerp (data.Velocity, Vector2.zero, Mathf.Clamp01 ((Time.time - data.LastPanTime) / StopTimer));
			}
			data.TapTargets = null;
		}
		if (Input.GetKey (KeyCode.LeftArrow)) {
			data.LastPanTime = Time.time;
			data.Velocity.x = -Screen.width / Sensitivity / 30;
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			data.LastPanTime = Time.time;
			data.Velocity.x = Screen.width / Sensitivity / 30;
		}
		if (Input.GetKey (KeyCode.UpArrow)) {
			data.LastPanTime = Time.time;
			data.Velocity.y = Screen.height / Sensitivity / 30;
		}
		if (Input.GetKey (KeyCode.DownArrow)) {
			data.LastPanTime = Time.time;
			data.Velocity.y = -Screen.height / Sensitivity / 30;
		}
		if (data.Velocity != Vector2.zero) {
			Position2D += data.Velocity * Sensitivity * Time.deltaTime;
			UpdatePosition ();		
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
