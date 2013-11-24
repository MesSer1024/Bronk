using System;

namespace Bronk
{
	public class CubeData
	{
		internal GameWorld.BlockType Type { get { return _type; } }

		internal int Index { get { return _index; } }

		private int _index;
		private GameWorld.BlockType _type;

		public CubeData (int index, GameWorld.BlockType type)
		{
			_index = index;
			_type = type;
		}

		public bool IsGround ()
		{
			return _type == GameWorld.BlockType.DirtGround;
		}
	}
}