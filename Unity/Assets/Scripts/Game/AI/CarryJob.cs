using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Bronk
{
	class CarryJob : IJob
	{
        public event JobCallback Completed;
        public event JobCallback AbortedByAnt;

        public GameObject WhatToCarry;
        public int TransportToBlockId;
        public int FetchedFromBlockID;

        public CarryJob(GameObject go, int fetchFromBlockId, int targetBlockId) {
            WhatToCarry = go;
            FetchedFromBlockID = fetchFromBlockId;
            TransportToBlockId = targetBlockId;
        }

        public void complete() {
            if (Completed != null)
                Completed.Invoke(this);
        }

        public void abortByAnt() {
            if (AbortedByAnt != null)
                AbortedByAnt.Invoke(this);
        }


        public void abortByUser() {
            throw new NotImplementedException();
        }
    }
}
