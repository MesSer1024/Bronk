﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Bronk
{
    public class GoldObject : ICarryObject
    {
        public GoldObject(int startBlockID, int itemID)
            : base(startBlockID, itemID)
        {
        }
    }
}