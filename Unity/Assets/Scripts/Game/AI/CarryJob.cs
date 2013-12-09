using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Bronk
{
	class CarryJob : IJob
	{
        public int BlockID_start;
        public int BlockID_end;

        public List<Ant> AssignedAnts = new List<Ant>();

        public float StartTime { get; private set; }
        public float EndTime { get; private set; }

        private float _earliestPickupTime;

        public CarryJob(int fetchFromBlockId, int targetBlockId, float earliestPickupTime) {
            BlockID_start = fetchFromBlockId;
            BlockID_end = targetBlockId;
            _earliestPickupTime = earliestPickupTime;
        }

        public void plan(Ant ant, List<Pathfinding.Node> pathToPickup, List<Pathfinding.Node> pathToDropOffZone) {
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

            for (int nodeIndex = 1; nodeIndex < pathToPickup.Count - 1; nodeIndex++) {
                var node = pathToPickup[nodeIndex];
                Vector2 blockPosition = Game.World.getCubePosition(node.blockID);
                float distance = (blockPosition - lastPosition).magnitude;
                dt += distance / moveSpeed;
                ant.addPositionKeyframe(dt, blockPosition);
                lastPosition = blockPosition;
            }

            //wait for right amount of time to pass
            if (dt < _earliestPickupTime) {
                ant.addStateKeyframe(dt, new StateData(GameEntity.States.WaitingForOtherJobToFinish));
            }
            //start carrying when object is available
            dt = Math.Max(dt, _earliestPickupTime); 
            ant.addPositionKeyframe(dt, lastPosition);

            //carry to target
            ant.addStateKeyframe(dt, new StateData(GameEntity.States.Carry));

            for (int nodeIndex = 1; nodeIndex < pathToDropOffZone.Count - 1; nodeIndex++) {
                var node = pathToDropOffZone[nodeIndex];
                Vector2 blockPosition = Game.World.getCubePosition(node.blockID);
                float distance = (blockPosition - lastPosition).magnitude;
                dt += distance / moveSpeed;
                ant.addPositionKeyframe(dt, blockPosition);
                lastPosition = blockPosition;
            }

            //when everything is finished...
            EndTime = dt;
            ant.addStateKeyframe(EndTime, new StateData(GameEntity.States.Idle));
            Game.World.ViewComponent.SetTimeline(ant.ID, ant.GetPositionTimeline());
            Game.World.ViewComponent.SetTimeline(ant.ID, ant.GetStateTimeline());

            var antJobs = ant.GetJobTimeline();
            antJobs.AddKeyframe(StartTime, this);
            antJobs.AddKeyframe(EndTime, null);
        }

        public void dispose() {
            AssignedAnts.Clear();
        }

        public void abortByAnt() {
            foreach (var ant in AssignedAnts) {
                clearStateOfAnt(ant);
            }
            AssignedAnts.Clear();
        }

        private void clearStateOfAnt(Ant ant) {
            ant.resetStateFuture();
            ant.resetPositionFuture();
            ant.GetJobTimeline().removeKeyframesInFuture(Game.LogicTime);
            ant.addStateKeyframe(Game.LogicTime, new StateData(GameEntity.States.Idle));

            //TODO: Drop anything being carried at the ants current position
        }

        public void abortByUser() {
            foreach (var ant in AssignedAnts) {
                clearStateOfAnt(ant);
            }
            AssignedAnts.Clear();
        }


        public bool isFinished() {
            if (AssignedAnts.Count == 0 || StartTime == 0 || EndTime == 0)
                return false;

            return Game.LogicTime >= EndTime;
        }


        public bool isPlanned() {
            if (AssignedAnts.Count == 0 || StartTime == 0 || EndTime == 0)
                return false;

            return AssignedAnts.Count > 0;
        }
    }
}
