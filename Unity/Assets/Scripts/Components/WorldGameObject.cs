using UnityEngine;
using System.Collections.Generic;
using Bronk;
using System;

public class WorldGameObject : MonoBehaviour
{
	public GameObject cubePrefab;
	private GameWorldData _blockData;
	private BlockObject[] _BlockSceneObjects;
	Dictionary<int, ITimelineObject> _Objects;
	private List<BlockTypeChange> _futureBlockChanges = new List<BlockTypeChange> ();
	private GameObject _AntPrefab;
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
	private Material _FloorMaterial;
	private Vector3[] _DecoratorVertexBuffer;
	private Color[] _DecoratorColorBuffer;

	void Awake ()
	{
		Game.World.ViewComponent = this;
	}

	public void init ()
	{
		BlockDecorators.Initialize ();
		_Objects = new Dictionary<int, ITimelineObject> ();

		LoadResources ();
		_blockData = new GameWorldData ();
		_blockData.OnBlockChanged += OnBlockChanged;
		_BlockSceneObjects = new BlockObject[Game.World.Blocks.SizeX * Game.World.Blocks.SizeZ];
		_blockData.init (Game.World.Blocks);
		for (int i = 0; i < _BlockSceneObjects.Length; i++) {
			_BlockSceneObjects [i] = GetViewBlock (i, _blockData.GetBlockType(i));
		}
	}

	void OnBlockChanged (int blockID, BlockData oldBlock, BlockData newBlock)
	{
		if (oldBlock.Type != newBlock.Type) {

			UpdateBlockView (blockID);

			int leftBlock = _blockData.getLeftCube (blockID);
			int rightBlock = _blockData.getRightCube (blockID);
			int topBlock = _blockData.getTopCube (blockID);
			int bottomBlock = _blockData.getBottomCube (blockID);

			if (leftBlock != -1)
				UpdateBlockView (leftBlock);
			if (rightBlock != -1)
				UpdateBlockView (rightBlock);
			if (topBlock != -1)
				UpdateBlockView (topBlock);
			if (bottomBlock != -1)
				UpdateBlockView (bottomBlock);
		}
		if (oldBlock.Selected != newBlock.Selected) {
			BlockObject viewObject = _BlockSceneObjects [blockID];
			if (viewObject != null)
				viewObject.SetSelected (newBlock.Selected);
		}
	}

	BlockObject GetViewBlock (int blockIndex, GameWorld.BlockType type)
	{
		Quaternion rotation = Quaternion.identity;
		GameObject prefab = GetTilePrefab (type, blockIndex, ref rotation);
		GameObject obj = Instantiate (prefab, new Vector3 (blockIndex % GameWorld.SIZE_X, 0, blockIndex / GameWorld.SIZE_Z), rotation) as GameObject;
		BlockObject blockObj = obj.GetComponentInChildren<BlockObject> ();

		blockObj.BlockType = type;

		if (type == GameWorld.BlockType.Gold)
			blockObj.DefaultMaterial = _GoldMaterial;
		else if (type == GameWorld.BlockType.Food)
			blockObj.DefaultMaterial = _FoodMaterial;
		else if (type == GameWorld.BlockType.DirtGround)
			blockObj.DefaultMaterial = _FloorMaterial;
		else
			blockObj.DefaultMaterial = _DirtMaterial;
		blockObj.UpdateMaterial ();

		if (blockObj == null)
			throw new Exception ("Failed to get BlockObject component when instantiating " + type + " block");
		blockObj.Index = blockIndex;
		return blockObj;
	}

	void ReleaseViewBlock (BlockObject obj)
	{
		GameObject.Destroy (obj.gameObject);
	}

	void LoadResources ()
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
		_FloorMaterial = Resources.Load<Material> ("Materials/FloorMaterial") as Material;

		_AntPrefab = Resources.Load<GameObject> ("CharacterPrefabs/AntWorker") as GameObject;
	}

	GameObject GetTilePrefab (GameWorld.BlockType type, int blockID, ref Quaternion rotation)
	{
		if (type == GameWorld.BlockType.DirtGround)
			return _Floor;

		int leftBlock = _blockData.getLeftCube (blockID);
		int rightBlock = _blockData.getRightCube (blockID);
		int topBlock = _blockData.getTopCube (blockID);
		int bottomBlock = _blockData.getBottomCube (blockID);

		bool left = leftBlock != -1 ? IsGroundBlock (leftBlock) : false;
		bool right = rightBlock != -1 ? IsGroundBlock (rightBlock) : false;
		bool top = topBlock != -1 ? IsGroundBlock (topBlock) : false;
		bool bottom = bottomBlock != -1 ? IsGroundBlock (bottomBlock) : false;
		
		int freeSides = 0;
		freeSides += left ? 1 : 0;
		freeSides += right ? 1 : 0;
		freeSides += top ? 1 : 0;
		freeSides += bottom ? 1 : 0;
		
		GameObject go = _Side0;
		if (freeSides == 1) {
			go = _Side1;
			if (left)
				rotation = Quaternion.Euler (Vector3.up * 180);
			if (right)
				rotation = Quaternion.Euler (Vector3.up * 0);
			if (top)
				rotation = Quaternion.Euler (Vector3.up * 90);
			if (bottom)
				rotation = Quaternion.Euler (Vector3.up * 270);
		} else if (freeSides == 2) {
			if (left && right || top && bottom) {
				go = _Side2;
				if (left && right)
					rotation = Quaternion.Euler (Vector3.up * 0);
				if (top && bottom)
					rotation = Quaternion.Euler (Vector3.up * 90);
			} else {
				go = _Corner2;
				if (left && top)
					rotation = Quaternion.Euler (Vector3.up * 180);
				if (top && right)
					rotation = Quaternion.Euler (Vector3.up * 90);
				if (right && bottom)
					rotation = Quaternion.Euler (Vector3.up * 0);
				if (bottom && left)
					rotation = Quaternion.Euler (Vector3.up * 270);
			}
		} else if (freeSides == 3) {
			go = _Side3;
			if (!left)
				rotation = Quaternion.Euler (Vector3.up * 270);
			if (!right)
				rotation = Quaternion.Euler (Vector3.up * 90);
			if (!top)
				rotation = Quaternion.Euler (Vector3.up * 180);
			if (!bottom)
				rotation = Quaternion.Euler (Vector3.up * 0);
		}
		if (freeSides == 4) {
			go = _Side4;
		}
		
		return go;
	}

	void Update ()
	{
		_blockData.Update (Time.time);
	}

	private bool IsGroundBlock (int blockID)
	{
		return _blockData.GetBlockType (blockID) == GameWorld.BlockType.DirtGround;
	}

	public Vector2 GetBlockPosition(int blockID)
	{
		return _blockData.getBlockPosition (blockID);
	}

	public GameObject getVisualCubeObject (int blockID)
	{
		var viewObject = _BlockSceneObjects [blockID];
		return viewObject != null ? viewObject.gameObject : null;
	}

	public object getCubesBetween (int _item1Index, int _item2Index)
	{
		return null;
	}

	public void CreateAnt (int id, int type)
	{
		switch (type) {
		case 1:
			var go = Instantiate (_AntPrefab) as GameObject;
			var c = go.GetComponent<CharacterAnimationController> ();
			if (c == null)
				throw new Exception ("Unable to create object from gameobject.getcomponent");

			_Objects.Add (id, c);
			break;
		default:
			break;
		}
	}

	public GameWorld.BlockType GetBlockType (int blockID)
	{
		return _blockData.GetBlockType (blockID);
	}

	public void SetBlockType (int blockID, GameWorld.BlockType type, float time)
	{
		_blockData.SetBlockType (blockID, type, time);
	}

	public void SetBlockSelected (int blockID, bool selected, float time)
	{
		_blockData.setBlockSelected (blockID, selected, time);
	}

	void UpdateBlockView (int blockID)
	{
		BlockObject viewObject = _BlockSceneObjects [blockID];
		if (viewObject != null) {
			ReleaseViewBlock (viewObject);
		}
		var blockData = _blockData.getBlock (blockID);
		viewObject = GetViewBlock (blockID, blockData.Type);
		if (viewObject != null)
			viewObject.SetSelected (blockData.Selected);
		_BlockSceneObjects [blockID] = viewObject;
	}

	public void SetTimeline (int objectID, ITimeline timeline)
	{
		SetTimeline (objectID, timeline.Type, timeline);
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

	public void OnBlockInteract (int blockID)
	{
		SetBlockSelected (blockID, !_blockData.getBlockSelected(blockID), Time.time); // simulate selection on view
		MessageManager.ExecuteMessage (new CubeClickedMessage ("cube", blockID));
	}
}
