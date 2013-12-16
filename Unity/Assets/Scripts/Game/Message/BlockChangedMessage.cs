using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bronk
{
	class BlockChangedMessage : IMessage
	{
        public int BlockID { get; private set; }
        public BlockData OldBlock { get; private set; }
        public BlockData NewBlock { get; private set; }

        public BlockChangedMessage(int blockID, BlockData oldBlock, BlockData newBlock) {
            BlockID = blockID;
            OldBlock = oldBlock;
            NewBlock = newBlock;
        }

        public string getGroup() {
            return MessageManager.GameMessage;
        }

        public string getId() {
            return "asdf";
        }
    }
}
