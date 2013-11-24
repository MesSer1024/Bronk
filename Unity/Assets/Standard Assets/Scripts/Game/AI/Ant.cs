using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Bronk
{
	class Ant : GameEntity
	{
        private List<ITimeline> _timelines;
        private List<ITimeline> _activeTimelines;

        public Ant()
        {
            _timelines = new List<ITimeline>();
            _activeTimelines = new List<ITimeline>();
        }

        public void init()
        {

        }

        public override void update(float delta)
        {
            _activeTimelines.Clear();
            State = States.Idle;

            foreach (var i in _timelines)
            {
                var t = Time.time;
                if (t >= i.StartTime && t <= i.EndTime)
                {
                    _activeTimelines.Add(i);

                    if (i is MiningTimeline)
                        State = States.Mine;
                    else if (i is WalkTimeline)
                        State = States.Move;
                }
            }
        }

        public void addTimeline(ITimeline timeline)
        {
            _timelines.Add(timeline);
        }

        public List<ITimeline> getActiveTimelines()
        {
            return _activeTimelines;
        }

        public List<ITimeline> getTimelines()
        {
            return _timelines;
        }
    }
}
