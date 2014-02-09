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
	private bool _IsPanning = false;
	private bool _TouchInProgress = false;
	private Vector2[] _VelocityBuffer;
	private int _VelocityBufferIndex;
	private Vector2 _FingerMoveVector;

	void Awake ()
	{
		_VelocityBuffer = new Vector2[4];
		Application.targetFrameRate = 60;
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

			RaycastHit[] newHits = Physics.RaycastAll (ray);
			float closestDistance = float.MaxValue;
			int closestTarget = -1;
			for (int i = 0; i < newHits.Length; i++) {
				float distance = newHits [i].distance; 
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
		if (Input.touchCount == 1) {
			Touch finger = Input.touches [0];
			if (finger.deltaTime > 0.2f)
				return;
			if (finger.phase == TouchPhase.Began && !_TouchInProgress)
				_IsTapping = true;
			if (_IsTapping && (finger.phase == TouchPhase.Moved || finger.phase == TouchPhase.Stationary)) {
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
				_TouchInProgress = true;
			} else if (_IsTapping && finger.phase == TouchPhase.Ended) {
				if (_SemiHighlightEntity != null) {
					_SemiHighlightEntity.SemiSelect (false);
					_SemiHighlightEntity = null;
				}
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
			Vector2 deltaPos = finger.deltaPosition;
			_VelocityBuffer [_VelocityBufferIndex] = -deltaPos / finger.deltaTime;
			_VelocityBufferIndex++;
			if (_VelocityBufferIndex >= _VelocityBuffer.Length)
				_VelocityBufferIndex = 0;

			if (finger.phase == TouchPhase.Moved || finger.phase == TouchPhase.Began) {
				if (!_IsPanning) {
					_FingerMoveVector += deltaPos;
					if (_FingerMoveVector.magnitude > 20) {
						Vector2 movedExceptThisFrame = _FingerMoveVector - deltaPos;
						Vector3 transformedDir = transform.TransformDirection (-new Vector3 (movedExceptThisFrame.x, 0, movedExceptThisFrame.y) * Sensitivity);
						Position2D += new Vector2 (transformedDir.x, transformedDir.z);
						_IsPanning = true;
					}
				}
				if (_IsPanning) {

					_LastPanTime = Time.time;
					if (_SemiHighlightEntity != null) {
						_SemiHighlightEntity.SemiSelect (false);
						_SemiHighlightEntity = null; 
					}
					_IsPanning = true;
					Vector3 transformedDir = transform.TransformDirection (-new Vector3 (deltaPos.x, 0, deltaPos.y) * Sensitivity);
					Position2D += new Vector2 (transformedDir.x, transformedDir.z);
					UpdatePosition ();
					_IsTapping = false;
				} else {
					_Velocity = Vector2.zero;
				}
			} else if (finger.phase == TouchPhase.Stationary) {
				_Velocity = Vector2.zero;
			} else if (_IsPanning && finger.phase == TouchPhase.Ended) {
				Vector2 velocity = Vector2.zero;
				for (int i = 0; i < _VelocityBuffer.Length; i++) {
					velocity += _VelocityBuffer [i];
				}
				velocity /= _VelocityBuffer.Length;
				_Velocity = velocity; 
			}
		} else {
			_FingerMoveVector = Vector2.zero;
			_IsPanning = false;
			_TouchInProgress = false;
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

	Vector2 GetConstrainedPos (Vector2 pos)
	{
		IntRect boundingBox = Game.World.Blocks.DiscoveredBoundingBox;
		Vector2 min = Game.World.Blocks.getBlockPosition (boundingBox.xMin + boundingBox.yMin * GameWorld.SIZE_X);
		Vector2 max = Game.World.Blocks.getBlockPosition (boundingBox.xMax + boundingBox.yMax * GameWorld.SIZE_X);
		Vector2 constrainedPos = Vector2.Max (min, pos);
		constrainedPos = Vector2.Min (max, constrainedPos);
		return constrainedPos;
	}

	void UpdatePosition ()
	{
		Vector2 constrainedPos = GetConstrainedPos (Position2D);
		Position2D = constrainedPos;
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

	void OnPreRender ()
	{
		BlockDecorators.Update ();
	}

	void GetTileIndices (Ray ray, out int endXIndex, out int endYIndex)
	{
		endXIndex = 0;
		endYIndex = 0;
	}
}
