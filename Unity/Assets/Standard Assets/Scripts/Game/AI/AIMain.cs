using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Bronk
{
	class AIMain : IMessageListener
	{
        private List<Ant> _ants = new List<Ant>();
        private Queue<MiningTimeline> _jobs = new Queue<MiningTimeline>();


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
        }

        public void onMessage(IMessage message)
        {
            if (message is CubesSelectedMessage)
            {
                var msg = message as CubesSelectedMessage;
                //order cubes in accessibility order? No, does not work since it can be updated depending on what nodes that are accessible
                //need to support marking a large area, and then removing one or more objects from it, which should make it into 2 different jobs...
                //probably good to have more than one job running actively

                if(msg.getCubes().Count > 0) {
                    var cube = msg.getCubes()[0];
                    foreach (var ant in _ants)
                    {
                        float dt = Time.time + 1.5f;
                        var walk = new WalkTimeline(ant.Position, Game.World.getCubePosition(cube.Index), Time.time, dt, ant);
                        var mine = new MiningTimeline(Game.World.getCubeData(cube.Index), dt, dt + 3);
                        ant.addTimeline(walk);
                        ant.addTimeline(mine);
                    }
                }
            }
        }
    }
}
