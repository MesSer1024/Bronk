using UnityEngine;
using System.Collections.Generic;

public class GameWorld : MonoBehaviour {
	public GameObject cubePrefab;
	
	private List<GameObject> _cubes;
    private List<BlockType> _data;

    private const int SIZE_X = 128;
    private const int SIZE_Y = 64;
    private const int SIZE_Z = 128;

    private const int VISIBLE_X = 20;
    private const int VISIBLE_Y = 4;
    private const int VISIBLE_Z = 20;

    public enum BlockType : sbyte {
        None,
        Unknown,
        Dirt,
        Stone
    }

	void Start ()
	{
		_cubes = new List<GameObject>();
        _data = new List<BlockType>(SIZE_X * SIZE_Y * SIZE_Z);

		for (int x=0; x < SIZE_X; ++x) {
			for (int y=0; y < SIZE_Y; ++y) {
				for (int z=0; z < SIZE_Z; ++z) {
                    var rnd = Random.value;
                    BlockType t;
                    if (rnd < 0.075) {
                        t = BlockType.None;
                    } else if (rnd < 0.75) {
                        t = BlockType.Dirt;
                    } else if (rnd < 0.96) {
                        t = BlockType.Stone;
                    } else {
                        t = BlockType.Unknown;
                    }
                    _data.Add(t);
				}
			}
		}

        //visible cubes
        for (var i = 0; i < System.Math.Min(_data.Count, VISIBLE_X*VISIBLE_Y*VISIBLE_Z); ++i) {
            //#TODO: fix some kind of way of getting the real data depending on camera position and what not...
            var block = _data[i];
            var cube = Instantiate(cubePrefab, new Vector3(i % VISIBLE_X, (i % (VISIBLE_X * VISIBLE_Y)) / VISIBLE_Z, i / (VISIBLE_X * VISIBLE_Y)), Quaternion.identity) as GameObject;
            _cubes.Add(cube);
            cube.GetComponent<CubeLogic>().setData(block);
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
