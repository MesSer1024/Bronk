using UnityEngine;
using System.Collections.Generic;
using System;

namespace Bronk
{
	public class DigJob : IJob
	{
        //public event JobCallback Completed;
        //public event JobCallback AbortedByAnt;

		public int BlockID;
		public List<Ant> AssignedAnts = new List<Ant>();
        private Pathfinding _pathfinding;

		public DigJob(int blockID, Pathfinding pathfinder)
		{
			BlockID = blockID;
            _pathfinding = pathfinder;
		}

        public void plan(Ant ant) {
            AssignedAnts.Add(ant);

            var digjob = this;
            var antJobs = ant.GetJobTimeline();
            var path = PathfindForBlock(ant.Position, digjob.BlockID);
            if (path != null) {
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
                ant.addStateKeyframe(dt, new StateData(GameEntity.States.Mine, digjob.BlockID));
                ant.addStateKeyframe(dt + miningTime, new StateData(GameEntity.States.Idle));
                Game.World.ViewComponent.SetTimeline(ant.ID, ant.GetPositionTimeline());
                Game.World.ViewComponent.SetTimeline(ant.ID, ant.GetStateTimeline());

                digjob.AssignedAnts.Add(ant);
                antJobs.AddKeyframe(Game.LogicTime, digjob);
                antJobs.AddKeyframe(dt + miningTime, null);

                //update world/blocks when mining is complete
                Game.World.Blocks.SetBlockType(digjob.BlockID, GameWorld.BlockType.DirtGround, dt + miningTime);
                Game.World.Blocks.setBlockSelected(digjob.BlockID, false, dt + miningTime);
                Game.World.ViewComponent.SetBlockType(digjob.BlockID, GameWorld.BlockType.DirtGround, dt + miningTime);
                Game.World.ViewComponent.SetBlockSelected(digjob.BlockID, false, dt + miningTime);
            }
        }

        public void complete() {
            //if (Completed != null)
            //    Completed.Invoke(this);
        }

        public void abortByAnt() {
        }

        private List<Pathfinding.Node> PathfindForBlock(Vector2 position, int blockID) {
            var path = _pathfinding.findPath(position, Game.World.getCubePosition(blockID));
#if VERBOSE_PATHFINDING
			                    if (path != null && path.Count > 0)
                    {
                        Debug.Log(String.Format("Moving between {0} and {1}", ant.Position, Game.World.getCubePosition(cube.Index)));
                        foreach (var node in path)
                        {   
							Debug.Log(String.Format("\t using: ({0},{1})", node.x, node.y), Game.World.ViewComponent.getVisualCubeObject(node.cube.Index));
                        }
                    }
                    else
                    {
                        Logger.Log(String.Format("Could not find path between {0} and {1}", ant.Position, Game.World.getCubePosition(cube.Index)));
                    }
#endif
            if (path != null)
                path = trimPath(path);
            return path;

        }

        private List<Pathfinding.Node> trimPath(List<Pathfinding.Node> path) {

            if (path.Count < 3)
                return path;
            List<Pathfinding.Node> toRemove = new List<Pathfinding.Node>();

            int lastBadNode = 0;
            for (int i = 1; i < path.Count - 2; i++) {
                var b1 = Game.World.getCubePosition(path[lastBadNode].blockID);
                var b2 = Game.World.getCubePosition(path[i].blockID);
                bool canSee = Game.World.Blocks.CanSee(path[lastBadNode].blockID, path[i].blockID);
                if (canSee == false) {
                    for (int j = i - 2; j > lastBadNode; j--) {
                        toRemove.Add(path[j]);
                    }
                    lastBadNode = i - 1;
                }
            }
            int toIndex = lastBadNode;
            for (int j = path.Count - 3; j > toIndex; j--) {
                toRemove.Add(path[j]);
            }

            for (int i = toRemove.Count - 1; i >= 0; i--) {
                if (!path.Remove(toRemove[i]))
                    throw new Exception();
            }
            return path;
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
        }
    }
}