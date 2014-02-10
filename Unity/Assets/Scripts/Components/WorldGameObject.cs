using UnityEngine;
using System.Collections.Generic;
using Bronk;
using System;

public class WorldGameObject : MonoBehaviour, IMessageListener
{
    public GameObject cubePrefab;
    private GameWorldData _blockData;
    private BlockObject[] _BlockSceneObjects;
    Dictionary<int, ITimelineObject> _Objects;
    Dictionary<int, ICarryObject> _CarryObjects;
    public GameObject _GoldItemPrefab;
    private GameObject _AntPrefab;
    private GameObject _ArtifactPrefab;

    private Transform _TerrainParent;
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
	private Material _UnknownMaterial;
	private Vector3[] _DecoratorVertexBuffer;
	private Color[] _DecoratorColorBuffer;
    
    void Awake()
    {
        Game.World.ViewComponent = this;
        LoadResources();
    }

    public void init()
    {
        BlockDecorators.Initialize();
        _Objects = new Dictionary<int, ITimelineObject>();
        _CarryObjects = new Dictionary<int, ICarryObject>();

        var terrainParent = new GameObject("Terrain");
        _TerrainParent = terrainParent.transform;

		MessageManager.AddListener (this);
		_blockData = new GameWorldData ();
		_BlockSceneObjects = new BlockObject[Game.World.Blocks.SizeX * Game.World.Blocks.SizeZ];
		_blockData.init (Game.World.Blocks);
		for (int i = 0; i < _BlockSceneObjects.Length; i++) {
			UpdateBlockView (i);
		}
	}

	BlockObject GetViewBlock (int blockIndex, ref BlockData blockData)
	{
		GameWorld.BlockType type = blockData.Type;
		Quaternion rotation = Quaternion.identity;
		GameObject prefab = GetTilePrefab (ref blockData, blockIndex, ref rotation);
		GameObject obj = Instantiate (prefab, new Vector3 (blockIndex % GameWorld.SIZE_X, 0, blockIndex / GameWorld.SIZE_Z), rotation) as GameObject;
        obj.transform.parent = _TerrainParent;
		BlockObject blockObj = obj.GetComponentInChildren<BlockObject> ();

        blockObj.BlockType = type;
		blockObj.Discovered = blockData.Discovered;

		if (blockData.Discovered == false)
			blockObj.DefaultMaterial = _UnknownMaterial;
		else if (type == GameWorld.BlockType.Gold)
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
		blockObj.BlockID = blockIndex;
		blockObj.SetSelected (blockData.Selected);
		return blockObj;
	}

    void ReleaseViewBlock(BlockObject obj)
    {
        GameObject.Destroy(obj.gameObject);
    }

    void LoadResources()
    {
        _Side0 = Resources.Load<GameObject>("Terrain/Walls/Side0") as GameObject;
        _Side1 = Resources.Load<GameObject>("Terrain/Walls/Side1") as GameObject;
        _Side2 = Resources.Load<GameObject>("Terrain/Walls/Side2") as GameObject;
        _Side3 = Resources.Load<GameObject>("Terrain/Walls/Side3") as GameObject;
        _Side4 = Resources.Load<GameObject>("Terrain/Walls/Side4") as GameObject;
        _Corner2 = Resources.Load<GameObject>("Terrain/Walls/Corner2") as GameObject;
        _Floor = Resources.Load<GameObject>("Terrain/Walls/Floor") as GameObject;

        _DirtMaterial = Resources.Load<Material>("Materials/DirtMaterial") as Material;
        _FoodMaterial = Resources.Load<Material>("Materials/FoodMaterial") as Material;
        _GoldMaterial = Resources.Load<Material>("Materials/GoldMaterial") as Material;
        _FloorMaterial = Resources.Load<Material>("Materials/FloorMaterial") as Material;
		_UnknownMaterial = Resources.Load<Material> ("Materials/UnknownMaterial") as Material;

        _AntPrefab = Resources.Load<GameObject>("CharacterPrefabs/AntWorker") as GameObject;
        _ArtifactPrefab = Resources.Load<GameObject>("PropPrefabs/Artifact") as GameObject;

		_GoldItemPrefab = Resources.Load<GameObject> ("PropPrefabs/Chair") as GameObject;
    }

    GameObject GetTilePrefab(ref BlockData blockData, int blockID, ref Quaternion rotation)
    {
        GameWorld.BlockType type = blockData.Type;
        if (blockData.Discovered == false)
            return _Side0;
        if (type == GameWorld.BlockType.DirtGround)
            return _Floor;

        int leftBlock = _blockData.getLeftBlock(blockID);
        int rightBlock = _blockData.getRightBlock(blockID);
        int topBlock = _blockData.getUpBlock(blockID);
        int bottomBlock = _blockData.getDownBlock(blockID);

        bool left = leftBlock != -1 ? IsGroundBlock(leftBlock) : false;
        bool right = rightBlock != -1 ? IsGroundBlock(rightBlock) : false;
        bool top = topBlock != -1 ? IsGroundBlock(topBlock) : false;
        bool bottom = bottomBlock != -1 ? IsGroundBlock(bottomBlock) : false;

        int freeSides = 0;
        freeSides += left ? 1 : 0;
        freeSides += right ? 1 : 0;
        freeSides += top ? 1 : 0;
        freeSides += bottom ? 1 : 0;

        GameObject go = _Side0;
        if (freeSides == 1)
        {
            go = _Side1;
            if (left)
                rotation = Quaternion.Euler(Vector3.up * 180);
            if (right)
                rotation = Quaternion.Euler(Vector3.up * 0);
            if (top)
                rotation = Quaternion.Euler(Vector3.up * 90);
            if (bottom)
                rotation = Quaternion.Euler(Vector3.up * 270);
        }
        else if (freeSides == 2)
        {
            if (left && right || top && bottom)
            {
                go = _Side2;
                if (left && right)
                    rotation = Quaternion.Euler(Vector3.up * 0);
                if (top && bottom)
                    rotation = Quaternion.Euler(Vector3.up * 90);
            }
            else
            {
                go = _Corner2;
                if (left && top)
                    rotation = Quaternion.Euler(Vector3.up * 180);
                if (top && right)
                    rotation = Quaternion.Euler(Vector3.up * 90);
                if (right && bottom)
                    rotation = Quaternion.Euler(Vector3.up * 0);
                if (bottom && left)
                    rotation = Quaternion.Euler(Vector3.up * 270);
            }
        }
        else if (freeSides == 3)
        {
            go = _Side3;
            if (!left)
                rotation = Quaternion.Euler(Vector3.up * 270);
            if (!right)
                rotation = Quaternion.Euler(Vector3.up * 90);
            if (!top)
                rotation = Quaternion.Euler(Vector3.up * 180);
            if (!bottom)
                rotation = Quaternion.Euler(Vector3.up * 0);
        }
        if (freeSides == 4)
        {
            go = _Side4;
        }
        return go;
    }

	public Vector2 GetBlockPosition (int blockID)
	{
		return _blockData.getBlockPosition (blockID);
	}

    void Update()
    {
        _blockData.Update(Time.time);
    }

    private bool IsGroundBlock(int blockID)
    {
        return _blockData.GetBlockType(blockID) == GameWorld.BlockType.DirtGround;
    }
    
    public GameObject getVisualCubeObject(int blockID)
    {
        var viewObject = _BlockSceneObjects[blockID];
        return viewObject != null ? viewObject.gameObject : null;
    }

    public object getCubesBetween(int _item1Index, int _item2Index)
    {
        return null;
    }

    public void CreateAnt(int id, int type)
    {
        switch (type)
        {
            case 1:
                var go = Instantiate(_AntPrefab) as GameObject;
                var c = go.GetComponent<CharacterAnimationController>();
                if (c == null)
                    throw new Exception("Unable to create object from gameobject.getcomponent");

                _Objects.Add(id, c);
                break;
            default:
                break;
        }
    }

    public void AddCarryItem(ICarryObject carryItem)
    {
        _CarryObjects.Add(carryItem.ItemId, carryItem);
    }

    public void RemoveCarryItem(ICarryObject carryItem)
    {
        _CarryObjects.Remove(carryItem.ItemId);
    }

    public GameObject InstantiateCarryItemView(ICarryObject carryItem)
    {
        GameObject prefab = null;
        if (carryItem is GoldObject)
            prefab = _GoldItemPrefab;
        else if (carryItem is ArtifactObject)
            prefab = _ArtifactPrefab;
        else
            Debug.LogError("Prefab not found for ICarryObject:" + carryItem.ToString());

        Debug.Log("Instantiating gameObject based on:" + carryItem.BlockId);
        var go = Instantiate(prefab, new Vector3(carryItem.BlockId % GameWorld.SIZE_X, 0, carryItem.BlockId / GameWorld.SIZE_Z), Quaternion.Euler(Vector3.up * 0)) as GameObject;
        ClickableItem item = go.GetComponent<ClickableItem>();
        item.CarryObject = carryItem;
        return go;
    }

    public GameWorld.BlockType GetBlockType(int blockID)
    {
        return _blockData.GetBlockType(blockID);
    }

    public void SetBlockType(int blockID, GameWorld.BlockType type, float time)
    {
        _blockData.SetBlockType(blockID, type, time);
    }

    public void SetBlockSelected(int blockID, bool selected, float time)
    {
        _blockData.setBlockSelected(blockID, selected, time);
    }

	void UpdateBlockView (int blockID)
	{
		BlockObject viewObject = _BlockSceneObjects [blockID];
		if (viewObject != null) {
			ReleaseViewBlock (viewObject);
			viewObject = null;
		}
		var blockData = _blockData.getBlock (blockID);
		viewObject = GetViewBlock (blockID, ref blockData);
		if (viewObject != null) {
			viewObject.SetSelected (blockData.Selected);
		}
		_BlockSceneObjects [blockID] = viewObject;
	}

    public void SetTimeline(int objectID, ITimeline timeline)
    {
        SetTimeline(objectID, timeline.Type, timeline);
    }

    void SetTimeline(int objectID, TimelineType timelineType, ITimeline timeline)
    {
        ITimelineObject obj = GetObject(objectID);
        if (obj == null)
            throw new ArgumentException("Could not find object with id " + objectID);
        SetTimeline(obj, timelineType, timeline);
    }

    void SetTimeline(ITimelineObject obj, TimelineType timelineType, ITimeline timeline)
    {
        ITimeline objTimeline = obj.GetTimeline(timelineType);
        if (objTimeline == null)
            throw new ArgumentException("Object " + obj + " returned null when asked for timeline type " + timelineType);
        objTimeline.CopyFrom(timeline);
    }

    ITimelineObject GetObject(int objectID)
    {
        ITimelineObject obj;
        _Objects.TryGetValue(objectID, out obj);
        return obj;
    }

	public void OnBlockInteract (int blockID)
	{
		BlockData data = _blockData.getBlock (blockID);
		if (data.Discovered
		    && IsGroundBlock (blockID))
			return;
		SetBlockSelected (blockID, !data.Selected, Time.time); // simulate selection on view
		if (IsGroundBlock (blockID) == false)
			MessageManager.QueueMessage (new CubeClickedMessage ("cube", blockID));
	}

	public void onMessage (IMessage message)
	{
		if (message is BlockChangedMessage) {
			var msg = message as BlockChangedMessage;
			onBlockChanged (msg.BlockID, msg.OldBlock, msg.NewBlock);
		}
	}

	void onBlockChanged (int blockID, BlockData oldBlock, BlockData newBlock)
	{
		if (oldBlock.Type != newBlock.Type) {

			UpdateBlockView (blockID);

			int leftBlock = _blockData.getLeftBlock (blockID);
			int rightBlock = _blockData.getRightBlock (blockID);
			int topBlock = _blockData.getUpBlock (blockID);
			int bottomBlock = _blockData.getDownBlock (blockID);

			if (leftBlock != -1)
				UpdateBlockView (leftBlock);
			if (rightBlock != -1)
				UpdateBlockView (rightBlock);
			if (topBlock != -1)
				UpdateBlockView (topBlock);
			if (bottomBlock != -1)
				UpdateBlockView (bottomBlock);

            if (oldBlock.Type == GameWorld.BlockType.Gold)
            {
                var item = getCarryObjectFromStartBlock(blockID);
                //item.View check required since this message is sent both when selected/deselected and when the block is really changed...
                if (item != null && item.View == null)
                {
                    item.View = InstantiateCarryItemView(item as GoldObject);
                }
            }
        }
        if (oldBlock.Selected != newBlock.Selected)
        {
            BlockObject viewObject = _BlockSceneObjects[blockID];
            if (viewObject != null)
                viewObject.SetSelected(newBlock.Selected);
        }
    }

    public ICarryObject getCarryObjectFromStartBlock(int blockID)
    {
        //TODO: Rewrite to actually work in a decent way
        foreach (var item in _CarryObjects.Values)
        {
            if (item.BlockId == blockID)
            {
                return item;
            }
        }
        return null;
    }
}
