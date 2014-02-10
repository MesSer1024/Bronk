using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Bronk {
    public abstract class CarryObject {
        public GameObject View { get; set; }
        public int BlockId;
        public int ItemId;

        public CarryObject(int blockId, int itemId) {
            BlockId = blockId;
            ItemId = itemId;
        }
    }
}
