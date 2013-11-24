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
			var cube = Instantiate (GetTilePrefab (block.TypeTimeline.GetValue (Time.time)), new Vector3 (i % GameWorld.SIZE_X, 0, i / GameWorld.SIZE_Z), Quaternion.identity) as GameObject;
			_cubes [i].ViewObject = cube;
			_cubes [i].BlockTypeTimeline = BlockTypeTimeline.Create ();
			_cubes [i].SelectedTimeline = SelectedTimeline.Create ();
			var view = cube.GetComponentInChildren<BlockObject> ();
			if (view != null)
				view.Index = i;
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
		UpdateBlocks ();
	}

	void UpdateBlocks()
	{
		for (int i = 0; i < _cubes.Length; i++) {
			ViewBlock block = _cubes [i];
			GameObject viewObject = block.ViewObject;
			if (viewObject != null) {
				bool selected = _cubes [i].SelectedTimeline.GetValue (Time.time);
				if (selected) {
					viewObject.GetComponentInChildren<Renderer> ().sharedMaterial = null;
				}
			}
		}
	}

	public GameObject getVisualCubeObject (int index)
	{
		if (index < 0 || index >= _cubes.Length)
			throw new ArgumentOutOfRangeException ("index");
		return _cubes [index].ViewObject;
	}

	public object getCubesBetween (int _item1Index, int _item2Index)
	{
		return null;
	}

	public BlockTypeTimeline GetBlockTypeTimeline (int blockID)
	{
		if (blockID < 0 || blockID >= _cubes.Length)
			throw new ArgumentOutOfRangeException ("index");
		return _cubes [blockID].BlockTypeTimeline;
	}

	public void SetBlockSelected(int blockID, float time, bool selected)
	{
		SelectedTimeline selectCurve = Game.World.Cubes [blockID].SelectedTimeline;
		selectCurve.AddKeyframe (time, selected);
		UpdateBlockSelected (blockID, selectCurve);
	}

	public void SetBlockType(int blockID, float time, GameWorld.BlockType type)
	{
		BlockTypeTimeline blockCurve = Game.World.Cubes [blockID].TypeTimeline;
		blockCurve.AddKeyframe (time, type); 
		UpdateBlockType (blockID, blockCurve);
	}

	void UpdateBlockType (int blockID, BlockTypeTimeline timeline)
	{
		if (blockID < 0 || blockID >= _cubes.Length)
			throw new ArgumentOutOfRangeException ("index");

		_cubes [blockID].BlockTypeTimeline.CopyFrom (timeline);
	}

	void UpdateBlockSelected (int blockID, SelectedTimeline timeline)
	{
		if (blockID < 0 || blockID >= _cubes.Length)
			throw new ArgumentOutOfRangeException ("index");

		_cubes [blockID].SelectedTimeline.CopyFrom (timeline);
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
		MessageManager.ExecuteMessage (new CubeClickedMessage ("cube", index));
	}
}
