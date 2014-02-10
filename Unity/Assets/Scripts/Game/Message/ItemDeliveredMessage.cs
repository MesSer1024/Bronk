using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bronk
{
	class ItemDeliveredMessage : IMessage
	{
        public int BlockFrom { get; private set; }
        public int BlockTo { get; private set; }
        public CarryObject Item { get; private set; }

        public ItemDeliveredMessage(CarryObject itemToDeliver)
        {
            //BlockFrom = oldBlockID;
            //BlockTo = newBlockID;
            Item = itemToDeliver;
        }

        public string getGroup() {
            return MessageManager.GameMessage;
        }

        public string getId() {
            return "ItemDeliveredMessage";
        }
    }
}
