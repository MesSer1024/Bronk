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
        private Queue<MiningTimeline> _jobs = new Queue<MiningTimeline>();
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

            foreach (var ant in _ants)
            {
                ant.update(delta);
            }

            foreach (var ant in _antViews)
            {
                ant.updateState(_ants[0]);
            }
        }

        public void onMessage(IMessage message)
        {
            if (message is CubeClickedMessage)
            {
                var msg = message as CubeClickedMessage;
                var cube = msg.getCube();
                foreach (var ant in _ants)
                {
                    Logger.Log("Applying stuff to ant!");

                    Func<Vector3, Vector3> convertTo2dAlignedVector = (Vector3 v) =>
                    {
                        Vector3 asdf = v;
                        asdf.y = v.z;
                        asdf.z = 0;
                        return asdf;
                    };

                    var p = new Pathfinding(GameWorld.SIZE_X, GameWorld.SIZE_Z, Game.World.Cubes);

                    var path = p.findPath(convertTo2dAlignedVector(ant.Position), convertTo2dAlignedVector(Game.World.getCubePosition(cube.Index)));
                    if (path != null && path.Count > 0)
                    {
                        Logger.Log(String.Format("Moving between {0} and {1}", ant.Position, Game.World.getCubePosition(cube.Index)));
                        foreach (var node in path)
                        {
                            Logger.Log(String.Format("\t using: ({0},{1})", node.x, node.y));
                        }
                    }
                    else
                    {
                        Logger.Log(String.Format("Could not find path between {0} and {1}", ant.Position, Game.World.getCubePosition(cube.Index)));
                    }

                    float dt = Time.time + 1.5f;
                    var walk = new WalkTimeline(ant.Position, Game.World.getCubePosition(cube.Index), Time.time, dt, ant);
                    var mine = new MiningTimeline(Game.World.getCubeData(cube.Index), dt, dt + 3);
                    ant.addTimeline(walk);
                    ant.addTimeline(mine);
                }
            }
        }

        internal void addAntView(CharacterAnimationController c)
        {
            _antViews.Add(c);
        }
    }
}
