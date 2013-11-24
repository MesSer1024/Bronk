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
			Quaternion rotation = Quaternion.identity;
			GameObject blockGO = GetTilePrefab(block.Type, block, ref rotation);
			var cube = Instantiate(blockGO, new Vector3(i % VISIBLE_X, 0, i / VISIBLE_Z), rotation) as GameObject;
            _cubes.Add(cube);
            var view = cube.GetComponentInChildren<CubeLogic>();
            view.setData(block);
            view.Index = i;
        }

        for (int i = 0; i < 4; i++)
        {
            var go = Instantiate(Resources.Load<GameObject>("CharacterPrefabs/AntWorker")) as GameObject;
            var c = go.GetComponent<CharacterAnimationController>();
            if (c == null)
                throw new Exception("Unable to create object from gameobject.getcomponent");

            //c.transform.position = new Vector3(Game.World.StartArea.x, 0, Game.World.StartArea.y);

            Game.AI.addAntView(c);
        }
        Game.World.ViewComponent = this;
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

	GameObject GetTilePrefab(GameWorld.BlockType type, CubeData block, ref Quaternion rotation)
	{
		if (block.IsGround())
			return _Floor;

		bool left = Game.World.getLeftCube(block) != null ? Game.World.getLeftCube(block).IsGround() : false;
		bool right = Game.World.getRightCube(block) != null ? Game.World.getRightCube(block).IsGround() : false;
		bool up = Game.World.getTopCube(block) != null ? Game.World.getTopCube(block).IsGround() : false;
		bool down = Game.World.getBottomCube(block) != null ? Game.World.getBottomCube(block).IsGround() : false;

		int freeSides = 0;
		freeSides += left ? 1 : 0;
		freeSides += right ? 1 : 0;
		freeSides += up ? 1 : 0;
		freeSides += down ? 1 : 0;

		GameObject go = _Side0;
		if (freeSides == 1)
		{
			go = _Side1;
			if (left) rotation = Quaternion.Euler(Vector3.up * 180);
			if (right) rotation = Quaternion.Euler(Vector3.up * 0);
			if (up) rotation = Quaternion.Euler(Vector3.up * 90);
			if (down) rotation = Quaternion.Euler(Vector3.up * 270);
		}
		else if (freeSides == 2)
		{
			if (left && right || up && down)
			{
				go = _Side2;
				if (left && right) rotation = Quaternion.Euler(Vector3.up * 0);
				if (up && down) rotation = Quaternion.Euler(Vector3.up * 90);
			}
			else
			{
				go = _Corner2;
				if (left && up) rotation = Quaternion.Euler(Vector3.up * 180);
				if (up && right) rotation = Quaternion.Euler(Vector3.up * 90);
				if (right && down) rotation = Quaternion.Euler(Vector3.up * 0);
				if (down && left) rotation = Quaternion.Euler(Vector3.up * 270);
			}
		}
		else if (freeSides == 3)
		{
			go = _Side3;
			if (!left) rotation = Quaternion.Euler(Vector3.up * 270);
			if (!right) rotation = Quaternion.Euler(Vector3.up * 90);
			if (!up) rotation = Quaternion.Euler(Vector3.up * 180);
			if (!down) rotation = Quaternion.Euler(Vector3.up * 0);
		}
		if (freeSides == 4)
		{
			go = _Side4;
		}

		return go;

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
		}
		 */
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
