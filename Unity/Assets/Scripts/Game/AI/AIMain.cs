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
