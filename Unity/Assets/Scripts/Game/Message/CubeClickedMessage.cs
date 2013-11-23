using System.Collections.Generic;
using UnityEngine;

namespace Bronk
{
	class CubeClickedMessage : IMessage
	{
        private string _id;
        private CubeLogic _item;
        private string p;

        public CubeClickedMessage(string id, CubeLogic item) {
            _id = id;
            _item = item;
        }

        public string getGroup() {
            return MessageManager.GameMessage;
        }

        public string getId() {
            return _id;
        }

        public CubeLogic getCube() {
            return _item;
        }
    }
}
