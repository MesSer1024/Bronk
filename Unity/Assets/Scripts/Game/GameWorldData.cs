using UnityEngine;
using System.Collections.Generic;

namespace Bronk
{
	public struct BlockData
	{
		public GameWorld.BlockType Type;
		public bool Discovered;
		public bool Selected;
	}

	struct BlockTypeChange
	{
		public int BlockID;
		public GameWorld.BlockType NewType;
		public float Time;
	}

	struct BlockSelectedChange
	{
		public int BlockID;
		public bool NewSelected;
		public float Time;
	}

	public class GameWorldData
	{
		// Cache delegate instances
		private System.Func<int, int> _WalkLeftFunc;
		private System.Func<int,int> _WalkRightFunc;
		private System.Func<int, int> _WalkTopFunc;
		private System.Func<int,int> _WalkBottomFunc;

		public int SizeX { get { return _sizeX; } }

		public int SizeZ { get { return _sizeZ; } }

		public IntRect DiscoveredBoundingBox { get { return _discoveredBoundingBox; } }

		List<BlockTypeChange> _futureBlockTypeChanges = new List<BlockTypeChange> ();
		List<BlockSelectedChange> _futureBlockSelectedChanges = new List<BlockSelectedChange> ();
		Queue<int> _floodfillQueue = new Queue<int> ();
		private int _sizeX;
		private int _sizeZ;
		private BlockData[] _data;
		private float _now;
		private IntRect _discoveredBoundingBox;

		public GameWorldData ()
		{
			_WalkLeftFunc = getLeftBlock;
			_WalkRightFunc = getRightBlock;
			_WalkTopFunc = getUpBlock;
			_WalkBottomFunc = getDownBlock;
		}

		public void init (BlockData[] data, int sizeX, int sizeZ, IntRect discoveredBoundingBox)
		{
			_data = data;
			_sizeX = sizeX;
			_sizeZ = sizeZ;
			_discoveredBoundingBox = discoveredBoundingBox;
		}

		public void init (GameWorldData otherData)
		{
			_data = new BlockData[otherData._data.Length];
			System.Array.Copy (otherData._data, _data, _data.Length);
			_sizeX = otherData.SizeX;
			_sizeZ = otherData.SizeZ;
			_discoveredBoundingBox = otherData._discoveredBoundingBox;
		}

		public void Update (float now)
		{
			_now = now;
			for (int i = 0; i < _futureBlockTypeChanges.Count; i++) {
				var blockChange = _futureBlockTypeChanges [i];
				if (blockChange.Time <= now) {
					SetBlockTypeDirect (blockChange.BlockID, blockChange.NewType);
					_futureBlockTypeChanges.RemoveAt (i);
					i--;
				}
			}
			for (int i = 0; i < _futureBlockSelectedChanges.Count; i++) {
				var blockChange = _futureBlockSelectedChanges [i];
				if (blockChange.Time <= now) {
					SetBlockSelectedDirect (blockChange.BlockID, blockChange.NewSelected);
					_futureBlockSelectedChanges.RemoveAt (i);
					i--;
				}
			}
		}

		public Vector2 getBlockPosition (int blockID)
		{
			return new Vector2 (blockID % _sizeX, (int)(blockID / _sizeZ));
		}

		public GameWorld.BlockType GetBlockType (int blockID)
		{
			return _data [blockID].Type;
		}

		public BlockData getBlock (int blockID)
		{
			return _data [blockID];
		}

		public int getBlockIDByPosition (Vector2 pos)
		{
			int index = (int)(pos.x) + (int)(pos.y) * _sizeX;
			if (index >= _data.Length || index < 0)
				return -1;
			return index;
		}

		/// <summary>
		/// Can only have one pending change active. Replaces any pending changes.
		/// </summary>
		public void SetBlockType (int blockID, GameWorld.BlockType type, float time)
		{
			if (GetBlockType (blockID) == type) {
				for (int i = 0; i < _futureBlockTypeChanges.Count; i++) {
					if (_futureBlockTypeChanges [i].BlockID == blockID) {
						_futureBlockTypeChanges.RemoveAt (i);
						break;
					}
				}
				return;
			}
			if (time <= _now) {
				for (int i = 0; i < _futureBlockTypeChanges.Count; i++) {
					if (_futureBlockTypeChanges [i].BlockID == blockID) {
						_futureBlockTypeChanges.RemoveAt (i);
						break;
					}
				}
				SetBlockTypeDirect (blockID, type);
			} else {
				bool foundChange = false;
				for (int i = 0; i < _futureBlockTypeChanges.Count; i++) {
					var blockChange = _futureBlockTypeChanges [i];
					if (blockChange.BlockID == blockID) {
						blockChange.Time = time;
						blockChange.NewType = type;
						_futureBlockTypeChanges [i] = blockChange;
						foundChange = true;
					}
				}
				if (!foundChange) {
					_futureBlockTypeChanges.Add (new BlockTypeChange () {
						BlockID = blockID,
						NewType = type,
						Time = time,
					});
				}
			}
		}

		void SetBlockTypeDirect (int blockID, GameWorld.BlockType type)
		{
			BlockData block = _data [blockID];
			if (block.Type != type) {
				var newBlock = block;

				newBlock.Type = type;
				_data [blockID] = newBlock;
				if (SetDiscoveredBlocks (blockID) == false)
					CallOnBlockChanged (blockID, ref block, ref newBlock);
			}
		}

		bool SetDiscoveredBlocks (int originBlockID)
		{
			bool changedOrigin = SetBlockDiscovered (originBlockID);
			_floodfillQueue.Enqueue (originBlockID);
			while (_floodfillQueue.Count > 0) {
				int block = _floodfillQueue.Dequeue ();
				SetBlockDiscovered (block);
				int rightIter = block;
				int lastRight;
				do {
					lastRight = rightIter;
					rightIter = getRightBlock (rightIter);
				} while (rightIter != -1 && SetBlockDiscovered (rightIter) && IsBlockValid (rightIter));
				int leftIter = block;
				int lastLeft;
				do {
					lastLeft = leftIter;
					leftIter = getLeftBlock (leftIter);
				} while (leftIter != -1 && SetBlockDiscovered (leftIter) && IsBlockValid (leftIter));


				int blockY = (block / SizeZ);
				int xLeft = (lastLeft % SizeX);
				int xRight = (lastRight % SizeX);
				for (int i = xLeft; i <= xRight; i++) {
					int blockUp = getUpBlock (i + blockY * SizeX);
					if (blockUp != -1
					    && SetBlockDiscovered (blockUp)
					    && IsBlockValid (blockUp)) {
						_floodfillQueue.Enqueue (blockUp);
					}
					int blockDown = getDownBlock (i + blockY * SizeX);
					if (blockDown != -1
					    && SetBlockDiscovered (blockDown)
					    && IsBlockValid (blockDown)) {
						_floodfillQueue.Enqueue (blockDown);
					}
				}
			}
			return changedOrigin;
		}

		bool SetBlockDiscovered (int blockID)
		{
			int x = blockID % GameWorld.SIZE_X;
			int y = blockID / GameWorld.SIZE_Z;
			if (blockID >= 0 && blockID < _data.Length) {
				BlockData blockData = _data [blockID];
				if (blockData.Discovered == false) {
					BlockData newData = blockData;
					newData.Discovered = true;
					if (IsBlockValid (blockID))
						newData.Selected = false;
					_data [blockID] = newData;
					CallOnBlockChanged (blockID, ref blockData, ref newData);

					_discoveredBoundingBox.xMin = Mathf.Min (x, _discoveredBoundingBox.xMin);
					_discoveredBoundingBox.xMax = Mathf.Max (x, _discoveredBoundingBox.xMax);
					_discoveredBoundingBox.yMin = Mathf.Min (y, _discoveredBoundingBox.yMin);
					_discoveredBoundingBox.yMax = Mathf.Max (y, _discoveredBoundingBox.yMax);
					return true;
				}
			}
			return false;
		}

		void SetBlockSelectedDirect (int blockID, bool newSelected)
		{
			BlockData block = _data [blockID];
			if (block.Selected != newSelected) {
				var newBlock = block;

				newBlock.Selected = newSelected;
				_data [blockID] = newBlock;
				CallOnBlockChanged (blockID, ref block, ref newBlock);
			}
		}

		void CallOnBlockChanged (int blockID, ref BlockData oldblock, ref BlockData newBlock)
		{
			MessageManager.QueueMessage (new BlockChangedMessage (blockID, oldblock, newBlock));
		}

		public int getRightBlock (int blockID)
		{
			int tarId = blockID + 1;
			if (tarId % _sizeX > 0) {
				return tarId;
			}
			return -1;
		}

		public int getLeftBlock (int blockID)
		{
			var tarId = (blockID % _sizeX) - 1;
			if (tarId < 0) {
				return -1;
			}
			return blockID - 1;
		}

		public int getUpBlock (int blockID)
		{
			var tarId = blockID - _sizeX;
			if (tarId >= 0) {
				return tarId;
			}
			return -1;
		}

		public int getDownBlock (int blockID)
		{
			var tarId = blockID + _sizeX;
			if (tarId < _data.Length) {
				return tarId;
			}
			return -1;
		}

		public void setBlockSelected (int blockID, bool selected, float time)
		{
			if (getBlockSelected (blockID) == selected) {
				for (int i = 0; i < _futureBlockSelectedChanges.Count; i++) {
					if (_futureBlockSelectedChanges [i].BlockID == blockID) {
						_futureBlockSelectedChanges.RemoveAt (i);
						break;
					}
				}
				return;
			}
			if (time <= _now) {
				for (int i = 0; i < _futureBlockSelectedChanges.Count; i++) {
					if (_futureBlockSelectedChanges [i].BlockID == blockID) {
						_futureBlockSelectedChanges.RemoveAt (i);
						break;
					}
				}
				SetBlockSelectedDirect (blockID, selected);
			} else {
				bool foundChange = false;
				for (int i = 0; i < _futureBlockSelectedChanges.Count; i++) {
					var blockChange = _futureBlockSelectedChanges [i];
					if (blockChange.BlockID == blockID) {
						blockChange.Time = time;
						blockChange.NewSelected = selected;
						_futureBlockSelectedChanges [i] = blockChange;
						foundChange = true;
					}
				}
				if (!foundChange) {
					_futureBlockSelectedChanges.Add (new BlockSelectedChange () {
						BlockID = blockID,
						NewSelected = selected,
						Time = time,
					});
				}
			}
		}

		public bool CanSee (Vector2 p1, Vector2 p2)
		{

			int targetBlock = getBlockIDByPosition (p2);
			int block = getBlockIDByPosition (p1);

			return CanSee (block, targetBlock);
		}

		public bool CanSee (int block, int targetBlock)
		{
			var p1 = getBlockPosition (block);
			if (block == -1)
				return false;
			if (targetBlock == -1)
				return false;
			if (block == targetBlock)
				return true;
			int xDelta = (block % SizeX) - (targetBlock % SizeX);
			int yDelta = (block / SizeZ) - (targetBlock / SizeZ);
			int xDistance = Mathf.Abs (xDelta);
			int yDistance = Mathf.Abs (yDelta);
			int totalDistance = xDistance + yDistance;

			System.Func<int,int> horizontalWalk = xDelta < 0 ? _WalkRightFunc : _WalkLeftFunc;
			System.Func<int,int> verticalWalk = yDelta > 0 ? _WalkTopFunc : _WalkBottomFunc;

			float xBlockRatio = (float)xDistance / (float)totalDistance;
			float yBlockRatio = (float)yDistance / (float)totalDistance;

			float progress = 0;
			while (true) {
				if (xBlockRatio == 0) {
					block = verticalWalk (block);
				} else if (yBlockRatio == 0) {
					block = horizontalWalk (block);
				} else {

					float xBlocksTraversed = xBlockRatio * progress;
					float yBlocksTraversed = yBlockRatio * progress;
					float xRemainder = Mathf.Repeat (xBlocksTraversed, 1.0f);
					float yRemainder = Mathf.Repeat (yBlocksTraversed, 1.0f);

					float progressNeededForX = (1 - xRemainder) / xBlockRatio;
					float progressNeededForY = (1 - yRemainder) / yBlockRatio;

					if (Mathf.Approximately (progressNeededForX, progressNeededForY)) {
						progress += progressNeededForX;
						int verticalBlock = verticalWalk (block);
						int horizontalBlock = horizontalWalk (block);
						bool verticalValid = IsBlockValid (verticalBlock);
						bool horizontalValid = IsBlockValid (horizontalBlock);

						if (verticalValid == false
						    || horizontalValid == false) {
							return false;
						}
						if (verticalBlock == targetBlock && verticalValid) {
							return true;
						}
						if (horizontalBlock == targetBlock && horizontalValid) {
							return true;
						}
						block = horizontalWalk (verticalBlock);
					} else if (progressNeededForX < progressNeededForY) {
						progress += progressNeededForX;
						block = horizontalWalk (block);
					} else if (progressNeededForX > progressNeededForY) {
						progress += progressNeededForY;
						block = verticalWalk (block);
					} 
				}
				if (IsBlockValid (block) == false) {
					return false;
				}

				if (block == targetBlock) {
					return true;
				}
			}
		}

		bool IsBlockValid (int block)
		{
			if (block == -1) {
				return false;
			}
			if (GetBlockType (block) != GameWorld.BlockType.DirtGround) {
				return false;
			}
			return true;
		}

		public bool getBlockSelected (int blockID)
		{
			return _data [blockID].Selected;
		}
	}
}
