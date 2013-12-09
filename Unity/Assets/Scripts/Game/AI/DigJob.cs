using UnityEngine;
using System.Collections.Generic;
using System;

namespace Bronk
{
	public class DigJob : IJob
	{
		public int BlockID;
		public List<Ant> AssignedAnts = new List<Ant>();

        public float StartTime { get; private set; }
        public float EndTime { get; private set;}

		public DigJob(int blockID) {
			BlockID = blockID;
		}

        public void plan(Ant ant, List<Pathfinding.Node> path) {
            AssignedAnts.Add(ant);

            ant.resetPositionFuture();
            ant.resetStateFuture();
            float dt = Game.LogicTime;
            float moveSpeed = ant.MoveSpeed;
            float miningTime = 3.0f;
            ant.addStateKeyframe(dt, new StateData(GameEntity.States.Move));
            Vector2 lastPosition = ant.Position;
            for (int nodeIndex = 1; nodeIndex < path.Count - 1; nodeIndex++) {
                var node = path[nodeIndex];
                Vector2 blockPosition = Game.World.getCubePosition(node.blockID);
                float distance = (blockPosition - lastPosition).magnitude;
                dt += distance / moveSpeed;
                ant.addPositionKeyframe(dt, blockPosition);
                lastPosition = blockPosition;
            }

            StartTime = dt;
            EndTime = dt + miningTime;

            ant.addStateKeyframe(StartTime, new StateData(GameEntity.States.Mine, BlockID));
            ant.addStateKeyframe(EndTime, new StateData(GameEntity.States.Idle));
            Game.World.ViewComponent.SetTimeline(ant.ID, ant.GetPositionTimeline());
            Game.World.ViewComponent.SetTimeline(ant.ID, ant.GetStateTimeline());

            AssignedAnts.Add(ant);
            var antJobs = ant.GetJobTimeline();
            antJobs.AddKeyframe(StartTime, this);
            antJobs.AddKeyframe(EndTime, null);

            //update world/blocks when mining is complete
            Game.World.Blocks.SetBlockType(BlockID, GameWorld.BlockType.DirtGround, EndTime);
            Game.World.Blocks.setBlockSelected(BlockID, false, EndTime);
            Game.World.ViewComponent.SetBlockType(BlockID, GameWorld.BlockType.DirtGround, EndTime);
            Game.World.ViewComponent.SetBlockSelected(BlockID, false, EndTime);
        }

        public void dispose() {
            AssignedAnts.Clear();
        }

        public void abortByAnt() {
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

        public void abortByUser() {
            foreach (var ant in AssignedAnts) {
                ant.resetStateFuture();
                ant.resetPositionFuture();
                ant.GetJobTimeline().removeKeyframesInFuture(Game.LogicTime);
                ant.addStateKeyframe(Game.LogicTime, new StateData(GameEntity.States.Idle));
                Game.World.ViewComponent.SetTimeline(ant.ID, ant.GetPositionTimeline());
                Game.World.ViewComponent.SetTimeline(ant.ID, ant.GetStateTimeline());

                //change world...
                var block = Game.World.Blocks.getBlock(BlockID);
                Game.World.Blocks.SetBlockType(BlockID, block.Type, Game.LogicTime);
                Game.World.ViewComponent.SetBlockType(BlockID, block.Type, Game.LogicTime);
                Game.World.ViewComponent.SetBlockSelected(BlockID, block.Selected, Game.LogicTime);
            }
            AssignedAnts.Clear();
        }
    }
}