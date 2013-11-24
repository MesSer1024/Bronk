using UnityEngine;
using System.Collections.Generic;
using Bronk;
using System;

public class WorldGameObject : MonoBehaviour {
	public GameObject cubePrefab;
	
	private List<GameObject> _cubes;
    private List<CharacterAnimationController> _ants;

	private GameObject _DirtFloorPrefab;
	private GameObject _FoodWallPrefab;
	private GameObject _DirtWallPrefab;
	private GameObject _GoldWallPrefab;

    private const int VISIBLE_X = 100;
    private const int VISIBLE_Z = 100;

	void Start ()
	{
        _cubes = new List<GameObject>();
        _ants = new List<CharacterAnimationController>();
		LoadTilePrefabs ();
		CubeLogic.InitializeMaterials (cubePrefab.renderer.sharedMaterial);
        //draw a 20x4x20 grid of items
        for (var i = 0; i < System.Math.Min(Game.World.Cubes.Count, VISIBLE_X*VISIBLE_Z); ++i) {
            //#TODO: fix some kind of way of getting the real data depending on camera position and what not...
            var block = Game.World.Cubes[i];
			var cube = Instantiate(GetTilePrefab(block.Type), new Vector3(i % VISIBLE_X, 0, i / VISIBLE_Z), Quaternion.identity) as GameObject;
            _cubes.Add(cube);
			//var view = cube.GetComponent<CubeLogic>();
			//view.setData(block);
			//view.Index = i;
        }

        var go = Instantiate(Resources.Load<GameObject>("CharacterPrefabs/AntWorker")) as GameObject;
        var c = go.GetComponent<CharacterAnimationController>();
        if (c == null)
            throw new Exception("Unable to create object from gameobject.getcomponent");

        Game.AI.addAntView(c);
	}

	void LoadTilePrefabs ()
	{
		_DirtWallPrefab = Resources.Load<GameObject>("Terrain/DirtWall") as GameObject;
		_GoldWallPrefab = Resources.Load<GameObject>("Terrain/GoldWall") as GameObject;
		_FoodWallPrefab = Resources.Load<GameObject>("Terrain/FoodWall") as GameObject;
		_DirtFloorPrefab = Resources.Load<GameObject>("Terrain/DirtFloor") as GameObject;
	}

	GameObject GetTilePrefab(GameWorld.BlockType type)
	{
		switch (type) {
		case GameWorld.BlockType.Dirt:
			return _DirtWallPrefab;
		case GameWorld.BlockType.DirtGround:
			return _DirtFloorPrefab;
		case GameWorld.BlockType.Food:
			return _FoodWallPrefab;
		case GameWorld.BlockType.Gold:
			return _GoldWallPrefab;
		default:
			throw new ArgumentException ("No prefab for " + type);
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
