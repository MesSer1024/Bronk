using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Bronk
{
	public class GoldObject : ITimelineObject
	{
        public GameObject View { get; set; }
        public int StartBlockID { get; set; }
        public int ItemID { get; private set; }

        public GoldObject(int startBlockID, int itemID) {
            StartBlockID = startBlockID;
            ItemID = itemID;
        }

        public ITimeline GetTimeline(TimelineType type) {
            throw new NotImplementedException();
        }

    }
}
