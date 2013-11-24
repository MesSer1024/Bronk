using System;
using UnityEngine;

namespace Bronk
{
	public class CubeData
	{
		internal GameWorld.BlockType Type { get { return _type; } }

		internal int Index { get { return _index; } }

		internal bool Selected { get { return _selected; } set { _selected = value; }}

		private int _index;
		private bool _selected;
		private GameWorld.BlockType _type;

		public CubeData (int index, GameWorld.BlockType type)
		{
			_index = index;
			_type = type;
			_selected = false;
		}

		public bool IsGround ()
		{
			return _type == GameWorld.BlockType.DirtGround;
		}
	}
}