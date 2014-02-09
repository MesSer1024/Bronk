using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Bronk
{
    public class ArtifactObject : ICarryObject
    {
        public ArtifactObject(int startBlockID, int itemID)
            : base(startBlockID, itemID)
        {
        }
    }
}
