using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Bronk
{
    class CarryJob : IJob
    {
        public int BlockID_end;
        public float EarliestPickupTime;

        public List<Ant> AssignedAnts = new List<Ant>();

        public float StartTime { get; private set; }
        public float EndTime { get; private set; }
        public CarryObject ItemToPickup { get; set; }

        public CarryJob(int targetBlockId, float earliestPickupTime, CarryObject itemToPickup)
        {
            BlockID_end = targetBlockId;
            EarliestPickupTime = earliestPickupTime;
            ItemToPickup = itemToPickup;
        }

        public void plan(Ant ant, List<Pathfinding.Node> pathToPickup, List<Pathfinding.Node> pathToDropOffZone)
        {
            if (ant == null || pathToPickup == null || pathToDropOffZone == null)
                throw new ArgumentException();

            AssignedAnts.Add(ant);
            float dt = Game.LogicTime;
            StartTime = dt;

            ant.resetPositionFuture();
            ant.resetJobFuture();
            ant.resetStateFuture();
            float moveSpeed = ant.MoveSpeed;
            Vector2 lastPosition = ant.Position;

            //move to pickup spot
            if (pathToPickup.Count > 0)
                ant.GetStateTimeline().AddKeyframe(dt, new StateData(GameEntity.States.Move));

            JobUtilities.FillPositionTimeline(ant, pathToPickup, ref dt, moveSpeed, ref lastPosition);

            //wait for right amount of time to pass
            if (dt < EarliestPickupTime)
            {
                ant.addStateKeyframe(dt, new StateData(GameEntity.States.WaitingForOtherJobToFinish));
            }
            //start carrying when object is available
            dt = Math.Max(dt, EarliestPickupTime);
            ant.addPositionKeyframe(dt, lastPosition);

            //carry to target
            var data = new StateData(GameEntity.States.Carry);
            data.CarryObject = ItemToPickup;
            ant.addStateKeyframe(dt, data);

            JobUtilities.FillPositionTimeline(ant, pathToDropOffZone, ref dt, moveSpeed, ref lastPosition);

            //when everything is finished...
            EndTime = dt;
            ant.addStateKeyframe(EndTime, new StateData(GameEntity.States.Idle));
            Game.World.ViewComponent.SetTimeline(ant.ID, ant.GetPositionTimeline());
            Game.World.ViewComponent.SetTimeline(ant.ID, ant.GetStateTimeline());

            var antJobs = ant.GetJobTimeline();
            antJobs.AddKeyframe(StartTime, this);
            antJobs.AddKeyframe(EndTime, null);
        }
        
        public void dispose()
        {
            AssignedAnts.Clear();
        }

        public void abortByAnt()
        {
            foreach (var ant in AssignedAnts)
            {
                clearStateOfAnt(ant);
            }
            AssignedAnts.Clear();
        }

        private void clearStateOfAnt(Ant ant)
        {
            Debug.Log("CarryJob clearStateOfAnt ID=" + ant.ID);
            ant.resetStateFuture();
            ant.resetPositionFuture();
            ant.GetJobTimeline().removeKeyframesInFuture(Game.LogicTime);
            ant.addStateKeyframe(Game.LogicTime, new StateData(GameEntity.States.Idle));
            Game.World.ViewComponent.SetTimeline(ant.ID, ant.GetPositionTimeline());
            Game.World.ViewComponent.SetTimeline(ant.ID, ant.GetStateTimeline());

            //TODO: Drop anything being carried at the ants current position
        }

        public void abortByUser()
        {
            Debug.Log("CarryJob abortByUser ants=" + AssignedAnts.Count);
            foreach (var ant in AssignedAnts)
            {
                clearStateOfAnt(ant);
            }
            AssignedAnts.Clear();
            if (ItemToPickup != null && ItemToPickup is GoldObject)
            {
                Game.World.ViewComponent.RemoveCarryItem(ItemToPickup);
            }
        }


        public bool isFinished()
        {
            if (AssignedAnts.Count == 0 || StartTime == 0 || EndTime == 0)
                return false;

            return Game.LogicTime >= EndTime;
        }


        public bool isPlanned()
        {
            if (AssignedAnts.Count == 0 || StartTime == 0 || EndTime == 0)
                return false;

            return AssignedAnts.Count > 0;
        }
    }
}
