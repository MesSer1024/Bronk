using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Bronk
{
	class BlockMinedMessage : IMessage
	{
        public int BlockID { get; private set; }

        public BlockMinedMessage(int blockID) {
            BlockID = blockID;
        }

        public string getGroup() {
            return MessageManager.GameMessage;
        }

        public string getId() {
            return "block";
        }
    }
}
