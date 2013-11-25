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
        private List<Ant> _ants = new List<Ant>();
        private List<CharacterAnimationController> _antViews = new List<CharacterAnimationController>();

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
                    ant = new Ant();
                    _ants.Add(ant);
                    ant.State = GameEntity.States.Removable;
                    break;
                default:
                    throw new Exception("TODO: make a decent solution for what type of ant to create!");
            }
            return ant;
        }

        public void update(float delta)
        {
            //iterate through all available jobs

            int i = 0;
            foreach (var ant in _ants)
            {
                ant.update(delta);
            }

            foreach (var ant in _antViews)
            {
                ant.LogicCharacter = _ants[i++];
            }
        }

        public void onMessage(IMessage message)
        {
            if (message is CubeClickedMessage)
            {
                var msg = message as CubeClickedMessage;
				var cube = Game.World.Cubes [msg.getCubeIndex()];
                foreach (var ant in _ants)
                {
                    Debug.Log("Applying stuff to ant!");

                    Func<Vector3, Vector3> convertTo2dAlignedVector = (Vector3 v) =>
                    {
                        Vector3 asdf = v;
                        asdf.y = v.z;
                        asdf.z = 0;
                        return asdf;
                    };

                    var p = new Pathfinding(GameWorld.SIZE_X, GameWorld.SIZE_Z, Game.World.Cubes);

                    var path = p.findPath(convertTo2dAlignedVector(ant.Position), convertTo2dAlignedVector(Game.World.getCubePosition(cube.Index)));

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
                    {
                        path = trimPath(path);

                        #if VERBOSE_PATHFINDING
                        Debug.Log(String.Format("Trimmed path between {0} and {1}", ant.Position, Game.World.getCubePosition(cube.Index)));
                        foreach (var node in path)
                        {   
                            Debug.Log(String.Format("\t using: ({0},{1})", node.x, node.y), Game.World.ViewComponent.getVisualCubeObject(node.cube.Index));
                        }
                        #endif


                        ant.resetPositionFuture();
                        ant.resetStateFuture();

                        float dt = Time.time;
                        float movementTime = 0.75f;
                        float miningTime = 3.0f;

                        //ant.addPositionKeyframe(dt, Game.World.getCubePosition(path[0].cube.Index));
                        //ant.addPositionKeyframe(dt, ant.Position);
                        ant.addStateKeyframe(dt, GameEntity.States.Move);

                        //starting on second item since first item is "start position" finishing on second last since last is end position
                        for (int i = 1; i < path.Count - 1; i++)
                        {
                            var node = path[i];
                            dt += movementTime;
                            ant.addPositionKeyframe(dt, Game.World.getCubePosition(node.cube.Index));
                        }
                        ant.addStateKeyframe(dt, GameEntity.States.Mine);
                        ant.addStateKeyframe(dt + miningTime, GameEntity.States.Idle);

                    }

                }
            }
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

        internal void addAntView(CharacterAnimationController c)
        {
            _antViews.Add(c);
        }
    }
}
