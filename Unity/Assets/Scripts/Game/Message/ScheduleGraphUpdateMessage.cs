using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bronk
{
	class ScheduleGraphUpdateMessage : IMessage
	{
        public int BlockID { get; private set; }
        public List<Pathfinding.Node> Neighbours { get; private set; }

        public ScheduleGraphUpdateMessage(int blockID, List<Pathfinding.Node> neighbours) {
            if (neighbours.Count < 2)
                throw new Exception("Unless a block has two neighbours, it is impossible for it to have a cheaper way and require a graph update!");
            BlockID = blockID;
            Neighbours = neighbours;
        }

        public string getGroup() {
            return MessageManager.GameMessage;
        }

        public string getId() {
            return "ScheduleGraphUpdateMessage";
        }
    }
}
