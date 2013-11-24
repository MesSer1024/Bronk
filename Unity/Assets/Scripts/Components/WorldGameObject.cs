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

	private GameObject _Side0;
	private GameObject _Side1;
	private GameObject _Side2;
	private GameObject _Side3;
	private GameObject _Side4;
	private GameObject _Corner2;
	private GameObject _Floor;

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
			var cube = Instantiate(GetTilePrefab(block.Type, block), new Vector3(i % VISIBLE_X, 0, i / VISIBLE_Z), Quaternion.identity) as GameObject;
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
				
		_Side0 = Resources.Load<GameObject>("Terrain/Walls/Side0") as GameObject;
		_Side1 = Resources.Load<GameObject>("Terrain/Walls/Side1") as GameObject;
		_Side2 = Resources.Load<GameObject>("Terrain/Walls/Side2") as GameObject;
		_Side3 = Resources.Load<GameObject>("Terrain/Walls/Side3") as GameObject;
		_Side4 = Resources.Load<GameObject>("Terrain/Walls/Side4") as GameObject;
		_Corner2 = Resources.Load<GameObject>("Terrain/Walls/Corner2") as GameObject;
		_Floor = Resources.Load<GameObject>("Terrain/Walls/Floor") as GameObject;
	}

	GameObject GetTilePrefab(GameWorld.BlockType type, CubeData block)
	{
		if (block.IsGround())
			return _Floor;
		else
			return _Side4;
		/*

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
		}*/
	}


	void asd(int index)
	{
		bool left = true;
		bool right = true;
		bool up = true;
		bool down = true;

		if (left && right && up && down)
		{

		}

		//new Vector3(index % VISIBLE_X, 0, index / VISIBLE_Z)
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
