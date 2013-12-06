using UnityEngine;
using System.Collections.Generic;

namespace Bronk
{
	public struct BlockData
	{
		public GameWorld.BlockType Type;
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
		public delegate void BlockChangeDelegate(int blockID, BlockData oldBlock, BlockData newBlock);
		public int SizeX { get { return _sizeX; } }
		public int SizeZ { get { return _sizeZ; } }

		List<BlockTypeChange> _futureBlockTypeChanges = new List<BlockTypeChange> ();
		List<BlockSelectedChange> _futureBlockSelectedChanges = new List<BlockSelectedChange> ();
		public BlockChangeDelegate OnBlockChanged;
		private int _sizeX;
		private int _sizeZ;

		private BlockData[] _data;
		private float _now;



		public void init (BlockData[] data, int sizeX, int sizeZ)
		{
			_data = data;
			_sizeX = sizeX;
			_sizeZ = sizeZ;
		}
		public void init (GameWorldData otherData)
		{
			_data = new BlockData[otherData._data.Length];
			System.Array.Copy (otherData._data, _data, _data.Length);
			_sizeX = otherData.SizeX;
			_sizeZ = otherData.SizeZ;
		}

		public void Update(float now)
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

		public GameWorld.BlockType GetBlockType(int blockID)
		{
			return _data [blockID].Type;
		}
		public BlockData getBlock(int blockID)
		{
			return _data [blockID];
		}

		public int getBlockIDByPosition(Vector2 pos)
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
			if (GetBlockType(blockID) == type) {
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
				CallOnBlockChanged (blockID, ref block, ref newBlock);
			}
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
			if (OnBlockChanged != null)
				OnBlockChanged (blockID, oldblock, newBlock);
		}

		public int getRightCube(int blockID)
        {
			int tarId = blockID + 1;
			if (tarId % _sizeX > 0)
            {
                return tarId;
            }
			return -1;
        }

		public int getLeftCube(int blockID)
        {
			var tarId = (blockID % _sizeX) - 1;
            if (tarId < 0)
            {
				return -1;
            }
			return blockID - 1;
        }

		public int getTopCube(int blockID)
        {
			var tarId = blockID - _sizeX;
            if (tarId >= 0)
            {
                return tarId;
            }
			return -1;
        }

		public int getBottomCube(int blockID)
        {
			var tarId = blockID + _sizeX;
			if (tarId < _data.Length)
            {
                return tarId;
            }
			return -1;
		}
		public void setBlockSelected(int blockID, bool selected, float time)
		{
			if (getBlockSelected(blockID) == selected) {
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

		public bool CanSee(Vector2 p1, Vector2 p2)
		{
			Debug.Log ("p1 " + p1 + " p2 " + p2);
			Vector2 distance = (p1 - p2);
			Vector2 dir = distance.normalized;
			int targetBlock = getBlockIDByPosition (p2);
			if (targetBlock == -1)
				return false;
			int block = getBlockIDByPosition (p1);
			if (block == -1)
				return false;
			if (block == targetBlock)
				return true;
			Vector2 progress = p1;
			while (true) {
				Debug.Log ("block " + block % SizeX + " " + block / SizeZ);
				float xProgress = progress.x % 1;
				float yProgress = progress.y % 1;
				float xVelocity = dir.x;
				float yVelocity = dir.y;
				float distanceRemainingX = 1 - xProgress;
				float distanceRemainingY = 1 - yProgress;
				float arrivalX = distanceRemainingX / xVelocity;
				float arrivalY = distanceRemainingY / yVelocity;
				if (xVelocity == 0 || arrivalX > arrivalY) {
					progress += dir * arrivalY;
					if (dir.y > 0)
						block = getTopCube (block);
					else
						block = getBottomCube (block);
				} else {
					progress += dir * arrivalX;
					if (dir.x > 0)
						block = getRightCube (block);
					else
						block = getLeftCube (block);
				}
				if (block == -1) {
					Debug.Log ("outside");
					return false;
				}
				if (GetBlockType (block) != GameWorld.BlockType.DirtGround) {
					Debug.Log ("block type " + GetBlockType(block));
					return false;
				}
				if (block == targetBlock)
					return true;
			}
		}
		public bool getBlockSelected(int blockID)
		{
			return _data [blockID].Selected;
		}
	}
}
