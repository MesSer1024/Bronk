using System.Collections.Generic;
using UnityEngine;

namespace Bronk
{
	class ItemClickedMessage : IMessage
	{
		private string _id;
        private int _index;
        private int _itemId;
        private CarryObject _item;

        public ItemClickedMessage(string id, int itemId, int cubeIndex, CarryObject gold)
		{
			_id = id;
            _itemId = itemId;
			_index = cubeIndex;
            _item = gold;
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

        public int getItemId()
        {
            return _itemId;
        }

        public CarryObject getItem()
        {
            return _item;
        }
	}
}
