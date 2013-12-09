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

                if (job is DigJob) {
                    //for digging, we need a valid block and a free ant to continue
                    var digjob = job as DigJob;
                    Vector2 jobPos = Game.World.Blocks.getBlockPosition(digjob.BlockID);
                    int closestAntIndex = FindClosestUnoccupiedAntIndex(jobPos);
                    if (closestAntIndex == -1)
                        break;

                    var ant = _ants[closestAntIndex];
                    digjob.plan(ant);
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
                    var job = new DigJob(cubeIndex, _pathfinding);
					_availableJobs.Add (job);
				} else {
					for (int jobIndex = 0; jobIndex < _availableJobs.Count; jobIndex++) {
						if (_availableJobs [jobIndex].BlockID == cubeIndex) {
                            _availableJobs[jobIndex].abortByUser();
							_availableJobs.RemoveAt (jobIndex--);
						}
					}
				}
			}
		}
	}
}
