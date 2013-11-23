using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bronk
{
	class CubeSemiSelectedMessage : IMessage
	{
		private string _id;
		private CubeLogic _cube;
		private bool _semiSelected;

		public CubeSemiSelectedMessage (string id, CubeLogic cube, bool semiSelected)
		{
			_id = id;
			_cube = cube;
			_semiSelected = semiSelected;
		}

		public string getGroup ()
		{
			return MessageManager.GameMessage;
		}

		public string getId ()
		{
			return _id;
		}

		public CubeLogic getCube ()
		{
			return _cube;
		}

		public bool getSemiSelected ()
		{
			return _semiSelected;
		}
	}
}
