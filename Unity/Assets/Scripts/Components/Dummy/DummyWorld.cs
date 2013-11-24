using UnityEngine;
using System.Collections;

public class DummyWorld : MonoBehaviour 
{
	public static DummyWorld Instance;

	// Use this for initialization
	void Awake () 
	{
		Instance = this;
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public DummyBed GetBed()
	{
		var beds = Object.FindObjectsOfType<DummyBed>();
		if (beds.Length > 0)
		{
			int randomIndex = Random.Range(0, beds.Length);
			return beds[randomIndex];
		}
		else
			return null;
	}

	public Vector3 GetGoldPos()
	{
		var golds = Object.FindObjectsOfType<DummyGold>();
		if (golds.Length > 0)
		{
			int randomIndex = Random.Range(0, golds.Length);
			return golds[randomIndex].transform.position;
		}
		else
			return Vector3.zero;
	}
}
