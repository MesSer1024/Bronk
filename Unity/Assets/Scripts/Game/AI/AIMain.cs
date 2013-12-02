//#define VERBOSE_PATHFINDING

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Bronk
{

    public class AIMain : IMessageListener
	{
		private int _AntIDCounter;
		private List<Ant> _ants = new List<Ant>();
		private List<DigJob> _availableJobs = new List<DigJob> ();
		private Pathfinding _pathfinding = new Pathfinding (GameWorld.SIZE_X, GameWorld.SIZE_Z, Game.World.Blocks);

        public AIMain()
        {
            MessageManager.AddListener(this);
        }

        public Ant createAnt(int type = 1)
        {
            Ant ant;
            switch (type)
            {
				case 1:
					ant = new Ant (_AntIDCounter++);
					_ants.Add (ant);
					ant.State = GameEntity.States.Removable;
					Game.World.ViewComponent.CreateAnt (ant.ID, type);
                    break;
                default:
                    throw new Exception("TODO: make a decent solution for what type of ant to create!");
            }
            return ant;
        }

		public void update(float delta)
        {
            //iterate through all available jobs
			for (int antIndex = 0; antIndex < _ants.Count; antIndex++) {
				var ant = _ants [antIndex];
				JobTimeline antJobs = ant.GetJobTimeline ();
				if (antJobs.GetValue(Game.LogicTime) == null) 
				{
					for (int i = 0; i < _availableJobs.Count; i++) 
					{
						var job = _availableJobs [i];
						if (job.AssignedAnts.Count == 0) 
						{
							var path = PathfindForBlock (ant.Position, job.BlockID);

							if (path != null) {

								ant.resetPositionFuture ();
								ant.resetStateFuture ();
								float dt = Game.LogicTime;
								float movementTime = 0.75f;
								float miningTime = 3.0f;
								//ant.addPositionKeyframe(dt, Game.World.getCubePosition(path[0].cube.Index));
								//ant.addPositionKeyframe(dt, ant.Position);
								ant.addStateKeyframe (dt, GameEntity.States.Move);
								//starting on second item since first item is "start position" finishing on second last since last is end position
								for (int nodeIndex = 1; nodeIndex < path.Count - 1; nodeIndex++) {
									var node = path [nodeIndex];
									dt += movementTime;
									ant.addPositionKeyframe (dt, Game.World.getCubePosition (node.blockID));
								}
								ant.addStateKeyframe (dt, GameEntity.States.Mine);
								ant.addStateKeyframe (dt + miningTime, GameEntity.States.Idle);
								Game.World.ViewComponent.SetTimeline (ant.ID, ant.GetPositionTimeline ());
								Game.World.ViewComponent.SetTimeline (ant.ID, ant.GetStateTimeline());

								job.AssignedAnts.Add (ant);

								antJobs.AddKeyframe (Game.LogicTime, job);
								antJobs.AddKeyframe (dt + miningTime, null);

								Game.World.Blocks.SetBlockType (job.BlockID, GameWorld.BlockType.DirtGround, dt + miningTime);
								Game.World.Blocks.setBlockSelected (job.BlockID, false, dt + miningTime);
								Game.World.ViewComponent.SetBlockType (job.BlockID, GameWorld.BlockType.DirtGround, dt + miningTime);
								Game.World.ViewComponent.SetBlockSelected (job.BlockID, false, dt + miningTime);
							}
						}
					}
				}
			}
        }

        public void onMessage(IMessage message)
        {
            if (message is CubeClickedMessage)
            {
                var msg = message as CubeClickedMessage;

				int cubeIndex = msg.getCubeIndex ();

				bool newSelected = Game.World.Blocks.getBlockSelected(cubeIndex); 
				newSelected = !newSelected;
				Game.World.Blocks.setBlockSelected (cubeIndex, newSelected, Time.time); // set selected on view time
				if (newSelected) 
				{
					_availableJobs.Add (new DigJob (cubeIndex));
				} 
				else 
				{
					for (int jobIndex = 0; jobIndex < _availableJobs.Count; jobIndex++) 
					{
						if (_availableJobs [jobIndex].BlockID == cubeIndex) 
						{
							var job = _availableJobs [jobIndex];
							for (int i = 0; i < job.AssignedAnts.Count; i++) {
								UnassignJobFromAnt (job, job.AssignedAnts [i]);
							}
							_availableJobs.RemoveAt (jobIndex);
						}
					}
				}
            }
        }

		void UnassignJobFromAnt (DigJob job, Ant ant)
		{
			ant.resetStateFuture ();
			ant.resetPositionFuture ();
			ant.GetJobTimeline ().removeKeyframesInFuture (Game.LogicTime);
			ant.addStateKeyframe(Game.LogicTime, GameEntity.States.Idle);
			Game.World.ViewComponent.SetTimeline (ant.ID, ant.GetPositionTimeline ());
			Game.World.ViewComponent.SetTimeline (ant.ID, ant.GetStateTimeline());
			var block = Game.World.Blocks.getBlock (job.BlockID);
			Game.World.ViewComponent.SetBlockType (job.BlockID, block.Type, Game.LogicTime);
			Game.World.ViewComponent.SetBlockSelected (job.BlockID, block.Selected, Game.LogicTime);
		}

		List<Pathfinding.Node> PathfindForBlock (Vector2 position, int blockID)
		{
			var path = _pathfinding.findPath (position, Game.World.getCubePosition (blockID));
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
			return path;

		}

        private List<Pathfinding.Node> trimPath(List<Pathfinding.Node> path)
        {
            if (path.Count < 3) return path;
            
            //asdfasasifjsadifasjidofjasdfoi can't figure this out - DD
            return path;
            //for (int i = 0; i < path.Count; i++)
            //{
            //    var node = path[i];
            //    Pathfinding.Node lastValidNode = null;
            //    for (int j = i + 2; j < path.Count; ++j)
            //    {
            //        //check can move directly to item instead of traversing through stuff...
            //        //draw line, does that line cross any other stuff that is possible to walk through, we can remove the previous node
            //        var nextWaypoint = path[j];
                    
            //        float k = (float)((nextWaypoint.y - node.y)) / (float)((nextWaypoint.x - node.x));
            //        //k = 1 means every time x is increased by 1, y is increased by 1
            //        //k = 1.25 means every time x is increased by 1, y is increased by 1.25

            //        var touchedTiles = new List<Pathfinding.Node>();

            //    }
            //}

            //int removedEntries = 0;
            //int timesMovingX = 0;
            //int timesMovingY = 0;
            //for (int i = path.Count - 2; i >= 0; --i)
            //{
            //    var node = path[i];
            //    var nextNode = path[i-1];
            //    if(node.y == nextNode.y) {
            //        //moving x-direction
            //        timesMovingX++;
            //        timesMovingY = 0;

            //        if (timesMovingX > 1)
            //        {
            //            path.RemoveAt(i);
            //            removedEntries++;
            //        }
            //    }
            //    else if (node.x == nextNode.x)
            //    {
            //        //moving y-direction
            //        timesMovingX = 0;
            //        timesMovingY++;

            //        if (timesMovingY > 1)
            //        {
            //            path.RemoveAt(i);
            //            removedEntries++;
            //        }
            //    }
            //}
            return path;
        }
    }
}
