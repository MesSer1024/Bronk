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

        public int TransportToBlockId;
        public int FetchedFromBlockID;

        public CarryJob(int fetchFromBlockId, int targetBlockId) {
            FetchedFromBlockID = fetchFromBlockId;
            TransportToBlockId = targetBlockId;
        }

        public void plan(Ant ant) {

        }

        public void dispose() {
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


        public bool isFinished() {
            throw new NotImplementedException();
        }


        public bool isPlanned() {
            throw new NotImplementedException();
        }


        public float StartTime {
            get { throw new NotImplementedException(); }
        }

        public float EndTime {
            get { throw new NotImplementedException(); }
        }
    }
}
