using UnityEngine;
[ExecuteInEditMode]
public class FPSCounter : MonoBehaviour
{
	private float _LastTime;
	private int _FpsCounter;
	private int _Fps;

	// Use this for initialization
	void Start()
	{

	}

	void OnGUI()
	{
		GUI.Label(new Rect(10, 10, 200, 30), "Fps: " + _Fps.ToString());
	}

	// Update is called once per frame
	void Update()
	{
		if (Time.time > _LastTime + 1f)
		{
			_Fps = _FpsCounter;
			_FpsCounter = 0;
			_LastTime = Time.time;
		}

		_FpsCounter++;
	}
}