using UnityEngine;
using System.Collections.Generic;
using Bronk;
using System;

public class WorldGameObject : MonoBehaviour
{
	public GameObject cubePrefab;
	private ViewBlock[] _cubes;
	private List<CharacterAnimationController> _ants;
	Dictionary<int, ITimelineObject> _Objects;
	
	private GameObject _Side0;
	private GameObject _Side1;
	private GameObject _Side2;
	private GameObject _Side3;
	private GameObject _Side4;	
	private GameObject _Corner2;
	private GameObject _Floor;
	
	private Material _DirtMaterial;
	private Material _FoodMaterial;
	private Material _GoldMaterial;

	void Start ()
	{
		_ants = new List<CharacterAnimationController> ();
		LoadTilePrefabs ();
		_cubes = new ViewBlock[Game.World.Cubes.Count];

		for (var i = 0; i < Game.World.Cubes.Count; ++i) {
			var block = Game.World.Cubes [i];
			BlockObject view = GetViewBlock (i, block.Type);
			_cubes [i].BlockType = block.Type;
			view.Index = i;
			_cubes [i].ViewObject = view;
		}

		for (int i = 0; i < 4; i++) {
			var go = Instantiate (Resources.Load<GameObject> ("CharacterPrefabs/AntWorker")) as GameObject;
			var c = go.GetComponent<CharacterAnimationController> ();
			if (c == null)
				throw new Exception ("Unable to create object from gameobject.getcomponent");

			//c.transform.position = new Vector3(Game.World.StartArea.x, 0, Game.World.StartArea.y);

			Game.AI.addAntView (c);
		}
		Game.World.ViewComponent = this;
	}

	BlockObject GetViewBlock (int blockIndex, GameWorld.BlockType type)
	{
		Quaternion rotation = Quaternion.identity;
		GameObject prefab = GetTilePrefab (type, Game.World.getCubeData(blockIndex),ref rotation);
		GameObject obj = Instantiate (prefab, new Vector3 (blockIndex % GameWorld.SIZE_X, 0, blockIndex / GameWorld.SIZE_Z), rotation) as GameObject;
		BlockObject blockObj = obj.GetComponentInChildren<BlockObject> ();

		if (type == GameWorld.BlockType.Gold)
			blockObj.DefaultMaterial = _GoldMaterial;
		else if (type == GameWorld.BlockType.Food)
			blockObj.DefaultMaterial = _FoodMaterial;
		else
			blockObj.DefaultMaterial = _DirtMaterial;
		blockObj.UpdateMaterial();

		if (blockObj == null)
			throw new Exception ("Failed to get BlockObject component when instantiating " + type + " block");
		return blockObj;
	}

	void ReleaseViewBlock (BlockObject obj)
	{
		GameObject.Destroy (obj.gameObject);
	}

	void LoadTilePrefabs ()
	{
		_Side0 = Resources.Load<GameObject> ("Terrain/Walls/Side0") as GameObject;
		_Side1 = Resources.Load<GameObject> ("Terrain/Walls/Side1") as GameObject;
		_Side2 = Resources.Load<GameObject> ("Terrain/Walls/Side2") as GameObject;
		_Side3 = Resources.Load<GameObject> ("Terrain/Walls/Side3") as GameObject;
		_Side4 = Resources.Load<GameObject> ("Terrain/Walls/Side4") as GameObject;
		_Corner2 = Resources.Load<GameObject> ("Terrain/Walls/Corner2") as GameObject;
		_Floor = Resources.Load<GameObject> ("Terrain/Walls/Floor") as GameObject;

		_DirtMaterial = Resources.Load<Material> ("Materials/DirtMaterial") as Material;
		_FoodMaterial = Resources.Load<Material> ("Materials/FoodMaterial") as Material;
		_GoldMaterial = Resources.Load<Material> ("Materials/GoldMaterial") as Material;
	}

	GameObject GetTilePrefab(GameWorld.BlockType type, CubeData block, ref Quaternion rotation)
	{
		if (block.IsGround())
			return _Floor;

		CubeData leftBlock = Game.World.getLeftCube(block);
		CubeData rightBlock = Game.World.getRightCube(block);
		CubeData topBlock = Game.World.getTopCube(block);
		CubeData bottomBlock = Game.World.getBottomCube(block);

		bool left = leftBlock != null ? leftBlock.IsGround() : false;
		bool right = rightBlock != null ? rightBlock.IsGround() : false;
		bool top = topBlock != null ? topBlock.IsGround() : false;
		bool bottom = bottomBlock != null ? bottomBlock.IsGround() : false;
		
		int freeSides = 0;
		freeSides += left ? 1 : 0;
		freeSides += right ? 1 : 0;
		freeSides += top ? 1 : 0;
		freeSides += bottom ? 1 : 0;
		
		GameObject go = _Side0;
		if (freeSides == 1)
		{
			go = _Side1;
			if (left) rotation = Quaternion.Euler(Vector3.up * 180);
			if (right) rotation = Quaternion.Euler(Vector3.up * 0);
			if (top) rotation = Quaternion.Euler(Vector3.up * 90);
			if (bottom) rotation = Quaternion.Euler(Vector3.up * 270);
		}
		else if (freeSides == 2)
		{
			if (left && right || top && bottom)
			{
				go = _Side2;
				if (left && right) rotation = Quaternion.Euler(Vector3.up * 0);
				if (top && bottom) rotation = Quaternion.Euler(Vector3.up * 90);
			}
			else
			{
				go = _Corner2;
				if (left && top) rotation = Quaternion.Euler(Vector3.up * 180);
				if (top && right) rotation = Quaternion.Euler(Vector3.up * 90);
				if (right && bottom) rotation = Quaternion.Euler(Vector3.up * 0);
				if (bottom && left) rotation = Quaternion.Euler(Vector3.up * 270);
			}
		}
		else if (freeSides == 3)
		{
			go = _Side3;
			if (!left) rotation = Quaternion.Euler(Vector3.up * 270);
			if (!right) rotation = Quaternion.Euler(Vector3.up * 90);
			if (!top) rotation = Quaternion.Euler(Vector3.up * 180);
			if (!bottom) rotation = Quaternion.Euler(Vector3.up * 0);
		}
		if (freeSides == 4)
		{
			go = _Side4;
		}
		
		return go;
	}

	void Update ()
	{

	}

	public GameObject getVisualCubeObject (int index)
	{
		if (index < 0 || index >= _cubes.Length)
			throw new ArgumentOutOfRangeException ("index");
		return _cubes [index].ViewObject.gameObject;
	}

	public object getCubesBetween (int _item1Index, int _item2Index)
	{
		return null;
	}

	public GameWorld.BlockType GetBlockType (int blockID)
	{
		if (blockID < 0 || blockID >= _cubes.Length)
			throw new ArgumentOutOfRangeException ("index");
		return _cubes [blockID].BlockType;
	}

	public void SetBlockSelected (int blockID, bool selected)
	{
		if (blockID < 0 || blockID >= _cubes.Length)
			throw new ArgumentOutOfRangeException ("index");

		ViewBlock viewBlock = _cubes [blockID];
		viewBlock.Selected = selected;
		if (viewBlock.ViewObject != null)
			viewBlock.ViewObject.SetSelected (viewBlock.Selected);
	}

	public void SetBlockType (int blockID, GameWorld.BlockType type)
	{
		if (blockID < 0 || blockID >= _cubes.Length)
			throw new ArgumentOutOfRangeException ("index");

		ViewBlock viewBlock = _cubes [blockID];
		if (viewBlock.BlockType != type && viewBlock.ViewObject != null) {
			ReleaseViewBlock (viewBlock.ViewObject);
		}
		viewBlock.BlockType = type;
		viewBlock.ViewObject = GetViewBlock (blockID, type);
		viewBlock.ViewObject.SetSelected (viewBlock.Selected);
	}

	void SetTimeline (int objectID, TimelineType timelineType, ITimeline timeline)
	{
		ITimelineObject obj = GetObject (objectID);
		if (obj == null)
			throw new ArgumentException ("Could not find object with id " + objectID);
		SetTimeline (obj, timelineType, timeline);
	}

	void SetTimeline (ITimelineObject obj, TimelineType timelineType, ITimeline timeline)
	{
		ITimeline objTimeline = obj.GetTimeline (timelineType);
		if (objTimeline == null)
			throw new ArgumentException ("Object " + obj + " returned null when asked for timeline type " + timelineType);
		objTimeline.CopyFrom (timeline);
	}

	ITimelineObject GetObject (int objectID)
	{
		ITimelineObject obj;
		_Objects.TryGetValue (objectID, out obj);
		return obj;
	}

	public void OnBlockInteract (int index)
	{
		SetBlockSelected (index, !_cubes [index].Selected); // simulate selection on view
		MessageManager.ExecuteMessage (new CubeClickedMessage ("cube", index));
	}
}
