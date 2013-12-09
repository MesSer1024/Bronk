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
		private List<Ant> _ants = new List<Ant> ();
		private List<DigJob> _availableJobs = new List<DigJob> ();
		private Pathfinding _pathfinding = new Pathfinding (GameWorld.SIZE_X, GameWorld.SIZE_Z, Game.World.Blocks);

		public AIMain ()
		{
			MessageManager.AddListener (this);
		}

		public Ant createAnt (int type = 1)
		{
			Ant ant;
			switch (type) {
			case 1:
				ant = new Ant (_AntIDCounter++);
				_ants.Add (ant);
				ant.State = new StateData (GameEntity.States.Removable);
				Game.World.ViewComponent.CreateAnt (ant.ID, type);
				break;
			default:
				throw new Exception ("TODO: make a decent solution for what type of ant to create!");
			}
			return ant;
		}

		public void update (float delta)
		{
			for (int i = 0; i < _availableJobs.Count; i++) {
				var job = _availableJobs [i];
				if (job.AssignedAnts.Count > 0)
					continue;
				Vector2 jobPos = Game.World.Blocks.getBlockPosition (job.BlockID);
				int closestAntIndex = FindClosestUnoccupiedAntIndex (jobPos);
				if (closestAntIndex == -1)
					break;
				var ant = _ants [closestAntIndex];
				var antJobs = ant.GetJobTimeline ();
				var path = PathfindForBlock (ant.Position, job.BlockID);

				if (path != null) {
					ant.resetPositionFuture ();
					ant.resetStateFuture ();
					float dt = Game.LogicTime;
					float moveSpeed = ant.MoveSpeed;
					float miningTime = 3.0f;
					//ant.addPositionKeyframe(dt, Game.World.getCubePosition(path[0].cube.Index));
					//ant.addPositionKeyframe(dt, ant.Position);
					ant.addStateKeyframe (dt, new StateData (GameEntity.States.Move));
					Vector2 lastPosition = ant.Position;
					for (int nodeIndex = 1; nodeIndex < path.Count - 1; nodeIndex++) {
						var node = path [nodeIndex];
						Vector2 blockPosition = Game.World.getCubePosition (node.blockID);
						float distance = (blockPosition - lastPosition).magnitude;
						dt += distance / moveSpeed;
						ant.addPositionKeyframe (dt, blockPosition);
						lastPosition = blockPosition;
					}
					ant.addStateKeyframe (dt, new StateData (GameEntity.States.Mine, job.BlockID));
					ant.addStateKeyframe (dt + miningTime, new StateData (GameEntity.States.Idle));
					Game.World.ViewComponent.SetTimeline (ant.ID, ant.GetPositionTimeline ());
					Game.World.ViewComponent.SetTimeline (ant.ID, ant.GetStateTimeline ());

					job.AssignedAnts.Add (ant);
					antJobs.AddKeyframe (Game.LogicTime, job);
					antJobs.AddKeyframe (dt + miningTime, null);

					Game.World.Blocks.SetBlockType (job.BlockID, GameWorld.BlockType.DirtGround, dt + miningTime);
					Game.World.Blocks.setBlockSelected (job.BlockID, false, dt + miningTime);
					Game.World.ViewComponent.SetBlockType (job.BlockID, GameWorld.BlockType.DirtGround, dt + miningTime);
					Game.World.ViewComponent.SetBlockSelected (job.BlockID, false, dt + miningTime);
					break;
				}
			}
		}

		int FindClosestUnoccupiedAntIndex (Vector2 jobPos)
		{
			int closestAntIndex = -1;
			float closestDistance = float.MaxValue;
			for (int j = 0; j < _ants.Count; j++) {
				var ant = _ants [j];
				if (ant.GetJobTimeline ().HasFuture (Game.LogicTime) == false) {
					float distance = Vector2.Distance (jobPos, ant.Position);
					if (distance < closestDistance) {
						closestAntIndex = j;
						closestDistance = distance;
					}
				}
			}
			return closestAntIndex;
		}

		public void onMessage (IMessage message)
		{
			if (message is CubeClickedMessage) {
				var msg = message as CubeClickedMessage;

				int cubeIndex = msg.getCubeIndex ();

				bool newSelected = Game.World.Blocks.getBlockSelected (cubeIndex); 
				newSelected = !newSelected;
				Game.World.Blocks.setBlockSelected (cubeIndex, newSelected, Time.time); // set selected on view time
				if (newSelected) {
					_availableJobs.Add (new DigJob (cubeIndex));
				} else {
					for (int jobIndex = 0; jobIndex < _availableJobs.Count; jobIndex++) {
						if (_availableJobs [jobIndex].BlockID == cubeIndex) {
							var job = _availableJobs [jobIndex];
							for (int i = 0; i < job.AssignedAnts.Count; i++) {
								UnassignJobFromAnt (job, job.AssignedAnts [i]);
							}
							_availableJobs.RemoveAt (jobIndex--);
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
			ant.addStateKeyframe (Game.LogicTime, new StateData (GameEntity.States.Idle));
			Game.World.ViewComponent.SetTimeline (ant.ID, ant.GetPositionTimeline ());
			Game.World.ViewComponent.SetTimeline (ant.ID, ant.GetStateTimeline ());
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
			if (path != null)
				path = trimPath (path);
			return path;

		}

		private List<Pathfinding.Node> trimPath (List<Pathfinding.Node> path)
		{
			if (path.Count < 3)
				return path;

			Pathfinding.Node previousNode = path [1];
			for (int i = 1; i < path.Count - 2; i++) {
				bool canSee = Game.World.Blocks.CanSee (Game.World.getCubePosition (previousNode.blockID), Game.World.getCubePosition (path [i].blockID));
				if (canSee == false) {
					previousNode = path [i];
				} else {
					path.RemoveAt (i);
					i--;
				}
			}
			return path;
		}
	}
}
