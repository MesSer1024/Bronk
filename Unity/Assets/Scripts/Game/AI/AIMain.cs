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

                    ant.changePositionFuture(Time.time + 1.5f, Game.World.getCubePosition(cube.Index));
                    ant.changeStateFuture(Time.time, GameEntity.States.Move);
                    ant.changeStateFuture(Time.time + 1.5f, GameEntity.States.Mine);
                    ant.changeStateFuture(Time.time + 4, GameEntity.States.Idle);
                }
            }
        }

        internal void addAntView(CharacterAnimationController c)
        {
            _antViews.Add(c);
        }
    }
}
