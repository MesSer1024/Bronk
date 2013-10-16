using UnityEngine;
using System.Collections.Generic;

public class GameWorld : MonoBehaviour {
	public GameObject cubePrefab;
	
	private List<GameObject> _cubes;
	
	// Use this for initialization
	void Start ()
	{		
		_cubes = new List<GameObject>();
		int sizeX = 10;
		int sizeY = 10;
		int sizeZ = 4;
		for (int x=0; x < sizeX; ++x) {
			for (int y=0; y < sizeY; ++y) {
				for (int z=0; z < sizeZ; ++z) {
					_cubes.Add(Instantiate (cubePrefab, new Vector3 (x-sizeX/2, y-sizeY/2, z-sizeZ/2), Quaternion.identity) as GameObject);
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	}
	
	public List<GameObject> getCubes() {
		return _cubes;
	}
}
