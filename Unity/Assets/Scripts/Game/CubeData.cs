using System;
using UnityEngine;

namespace Bronk
{
	public class CubeData
	{
		internal BlockTypeTimeline TypeTimeline { get { return _type; } }

		internal int Index { get { return _index; } }

		internal SelectedTimeline SelectedTimeline { get { return _selected; }}

		private int _index;
		private SelectedTimeline _selected;
		private BlockTypeTimeline _type;

		public CubeData (int index, GameWorld.BlockType type)
		{
			_index = index;
			_type = BlockTypeTimeline.Create ();
			_type.AddKeyframe (Time.time, type);
			_selected = SelectedTimeline.Create ();
		}

		public bool IsGround (float time)
		{
			return _type.GetValue(time) == GameWorld.BlockType.DirtGround;
		}
	}
}