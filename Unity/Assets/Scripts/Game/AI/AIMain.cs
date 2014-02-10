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
		private static int _objectCounter;
		private List<Ant> _ants = new List<Ant> ();
        private List<IJob> _availableJobs = new List<IJob>();
		private Pathfinding _pathfinding;
        private PathfindingToBase _pathfindToBase;
        private static Dictionary<IJob, List<IJob>> _jobDependencies = new Dictionary<IJob, List<IJob>>();
        
		public AIMain ()
		{
			MessageManager.AddListener (this);
            _pathfinding = new Pathfinding(GameWorld.SIZE_X, GameWorld.SIZE_Z, Game.World.Blocks);
            _pathfindToBase = new PathfindingToBase(GameWorld.SIZE_X, GameWorld.SIZE_Z, Game.World.Blocks);
            _pathfindToBase.init(Game.World.Blocks.getBlockIDByPosition(Game.World.StartArea.center));
		}

        public static int GenerateUniqueId()
        {
            return _objectCounter++;
        }

		public Ant createAnt (int type = 1)
		{
			Ant ant;
			switch (type) {
			case 1:
				ant = new Ant (GenerateUniqueId());
				_ants.Add (ant);
				ant.State = new StateData (GameEntity.States.Removable);
				Game.World.ViewComponent.CreateAnt (ant.ID, type);
				break;
			default:
				throw new Exception ("TODO: make a decent solution for what type of ant to create!");
			}
			return ant;
		}

		public void update (float delta) {
            var addedJobs = new List<IJob>();
			for (int i = 0; i < _availableJobs.Count; i++) {
				var job = _availableJobs [i];

                //check if it is already planned, remove if finished
                if (job.isPlanned()) {
                    if (job.isFinished()) {
                        if (job is DigJob) {
                            var digJob = job as DigJob;
                            MessageManager.QueueMessage(new BlockMinedMessage(digJob.BlockID));
                        } else if (job is CarryJob) {
                            var carryJob = job as CarryJob;
                            MessageManager.QueueMessage(new ItemDeliveredMessage(carryJob.ItemToPickup));
                        }


                        job.dispose();
                        if (_jobDependencies.ContainsKey(job)) {
                            _jobDependencies[job].Clear();
                            _jobDependencies.Remove(job);
                        }
                        _availableJobs.RemoveAt(i--);
                    }
                    continue;
                }

                if (job is DigJob) {
                    //for digging, we need a valid block and a free ant to continue
                    var digjob = job as DigJob;
                    Vector2 jobPos = Game.World.Blocks.getBlockPosition(digjob.BlockID);
                    int bestAntIndex = FindClosestUnoccupiedAntIndex(jobPos);
                    if (bestAntIndex == -1)
                        break;

                    var ant = _ants[bestAntIndex];
                    var path = _pathfinding.PathfindForBlock(ant.Position, digjob.BlockID);
                    if (path != null) {
                        digjob.plan(ant, path);
                        
                        //create a pickup job
                        if (Game.World.Blocks.GetBlockType(digjob.BlockID) == GameWorld.BlockType.Gold) {
                            var gold = new GoldObject(digjob.BlockID, GenerateUniqueId());
                            Game.World.ViewComponent.AddCarryItem(gold);
                            //var carryJob = new CarryJob(Game.World.Blocks.getBlockIDByPosition(Game.World.StartArea.center), digjob.EndTime, gold);                            
                            //AddDependencyBetweenJobs(digjob, carryJob);
                            //addedJobs.Add(carryJob);
                        }
                    }
                } else if (job is CarryJob) {
                    //would probably be wise not to do any carry jobs unless they are highly prioritized or there are only carry jobs in queue
                    var carryJob = job as CarryJob;
                    var itemPosition = carryJob.ItemToPickup.BlockId;
                    Vector2 jobPos = Game.World.Blocks.getBlockPosition(itemPosition);
                    int bestAntIndex = FindClosestUnoccupiedAntIndex(jobPos);
                    if (bestAntIndex == -1)
                        break;

                    var ant = _ants[bestAntIndex];
                    var pathToPickup = _pathfinding.PathfindForBlock(ant.Position, itemPosition);
                    var pathToDropOff = _pathfindToBase.pathfindToHomebaseFrom(itemPosition);
                    if (pathToPickup != null && pathToDropOff != null) {
                        carryJob.plan(ant, pathToPickup, pathToDropOff);
                    }
                } else {
                    throw new Exception("No implementation for job of type: " + job);
                }
			}
            if(addedJobs.Count > 0)
                _availableJobs.AddRange(addedJobs);
		}

        private static void AddDependencyBetweenJobs(IJob parent, IJob child) {
            if(!_jobDependencies.ContainsKey(parent)) {
                _jobDependencies.Add(parent, new List<IJob>());
            }
            _jobDependencies[parent].Add(child);
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
					_availableJobs.Add (new DigJob(cubeIndex));
				} else {
					for (int jobIndex = 0; jobIndex < _availableJobs.Count; jobIndex++) {
                        var job = _availableJobs[jobIndex];
                        if(job is DigJob && (job as DigJob).BlockID == cubeIndex) {
                            if (_jobDependencies.ContainsKey(job)) {
                                Debug.Log(String.Format("Found {0} depedencise on this job", _jobDependencies[job].Count));
                                foreach (var child in _jobDependencies[job]) {
                                    child.abortByUser();
                                    _availableJobs.Remove(child);
                                }
                                _jobDependencies[job].Clear();
                            } else {
                                Debug.Log(String.Format("No dependencies found for the job being removed"));
                            }
                            job.abortByUser();
                            _availableJobs.RemoveAt(jobIndex--);
						}
					}
				}
            } else if (message is ScheduleGraphUpdateMessage) {
                //TODO: if we have sufficient amount of time left this frame or something...
                var msg = message as ScheduleGraphUpdateMessage;
                _pathfindToBase.updateGraphBasedOnNode(msg.BlockID, msg.Neighbours);
            }
            else if (message is ItemClickedMessage)
            {
                //TODO: if we have sufficient amount of time left this frame or something...
                ItemClickedMessage msg = message as ItemClickedMessage;
                var dropOffTarget = Game.World.Blocks.getBlockIDByPosition(Game.World.StartArea.center);
                var item = msg.getItem();

                //TODO: make into a dictionary<id, item>-solution or something to skip this ugly hack...
                if (!Game.World.StockpileComponent.isItemInStockpile(item)) {
                    var result = _availableJobs.Find(a => a is CarryJob && (a as CarryJob).ItemToPickup == item);
                    if (result == null) {
                        var carryJob = new CarryJob(dropOffTarget, 0f, item);
                        _availableJobs.Add(carryJob);
                    }
                }
            }
		}
	}
}
