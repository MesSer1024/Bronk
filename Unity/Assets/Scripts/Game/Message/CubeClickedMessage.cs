using System.Collections.Generic;
using UnityEngine;

namespace Bronk
{
	class CubeClickedMessage : IMessage
	{
		private string _id;
		private int _index;

		public CubeClickedMessage (string id, int cubeIndex)
		{
			_id = id;
			_index = cubeIndex;
		}

		public string getGroup ()
		{
			return MessageManager.GameMessage;
		}

		public string getId ()
		{
			return _id;
		}

		public int getCubeIndex ()
		{
			return _index;
		}
	}
}
