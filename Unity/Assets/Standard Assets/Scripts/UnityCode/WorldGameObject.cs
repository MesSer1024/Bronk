using UnityEngine;
using System.Collections.Generic;
using Bronk;

public class WorldGameObject : MonoBehaviour {
	public GameObject cubePrefab;
	
	private List<GameObject> _cubes;

    private const int VISIBLE_X = 20;
    private const int VISIBLE_Y = 4;
    private const int VISIBLE_Z = 20;

	void Start ()
	{
		_cubes = new List<GameObject>();

        //draw a 20x4x20 grid of items
        for (var i = 0; i < System.Math.Min(Game.World.Cubes.Count, VISIBLE_X*VISIBLE_Y*VISIBLE_Z); ++i) {
            //#TODO: fix some kind of way of getting the real data depending on camera position and what not...
            var block = Game.World.Cubes[i];
            var cube = Instantiate(cubePrefab, new Vector3(i % VISIBLE_X, (i % (VISIBLE_X * VISIBLE_Y)) / VISIBLE_Z, i / (VISIBLE_X * VISIBLE_Y)), Quaternion.identity) as GameObject;
            _cubes.Add(cube);
            var view = cube.GetComponent<CubeLogic>();
            view.setData(block);
            view.Index = i;
        }
	}
	
	// Update is called once per frame
	void Update ()
	{
	}
	
	public List<GameObject> getCubes() {
		return _cubes;
	}

    public object getCubesBetween(int _item1Index, int _item2Index) {
        return null;
    }
}
