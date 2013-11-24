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
	private GameObject _DirtFloorPrefab;
	private GameObject _FoodWallPrefab;
	private GameObject _DirtWallPrefab;
	private GameObject _GoldWallPrefab;

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
		GameObject obj = Instantiate (GetTilePrefab (type), new Vector3 (blockIndex % GameWorld.SIZE_X, 0, blockIndex / GameWorld.SIZE_Z), Quaternion.identity) as GameObject;
		BlockObject blockObj = obj.GetComponentInChildren<BlockObject> ();
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
		_DirtWallPrefab = Resources.Load<GameObject> ("Terrain/DirtWall") as GameObject;
		_GoldWallPrefab = Resources.Load<GameObject> ("Terrain/GoldWall") as GameObject;
		_FoodWallPrefab = Resources.Load<GameObject> ("Terrain/FoodWall") as GameObject;
		_DirtFloorPrefab = Resources.Load<GameObject> ("Terrain/DirtFloor") as GameObject;
	}

	GameObject GetTilePrefab (GameWorld.BlockType type)
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
