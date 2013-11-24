using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Bronk
{
    public class Ant : GameEntity, ITimelinedEntity
	{
        private AntStateTimeline _stateTimeline;
        private PositionTimeline _positionTimeline;

        public override Vector3 Position
        {
            get
            {
                return _positionTimeline.GetValue(Time.time);
            }
            set
            {
                _positionTimeline.AddKeyframe(Time.time, value);
            }
        }

        public override GameEntity.States State
        {
            get
            {
                return _stateTimeline.GetValue(Time.time);
            }
            set
            {
                _stateTimeline.AddKeyframe(Time.time, value);
            }
        }

        public Ant()
        {
            _stateTimeline = AntStateTimeline.Create();
            _positionTimeline = PositionTimeline.Create();

            _positionTimeline.AddKeyframe(Time.time, Position);
            _stateTimeline.AddKeyframe(Time.time, GameEntity.States.Idle);
        }

        public void init()
        {

        }

        public void changeStateFuture(float time, GameEntity.States state) {
            //TODO remove any possible future since that is no longer valid...
            _stateTimeline.AddKeyframe(time, state);
        }

        public void changePositionFuture(float time, Vector3 v)
        {
            _positionTimeline.AddKeyframe(Time.time, Position);
            //TODO remove any possible future since that is no longer valid...
            _positionTimeline.AddKeyframe(time, v);
        }

        public override void update(float delta)
        {
            

            //_activeTimelines.Clear();
            //State = States.Idle;

            //foreach (var i in _timelines)
            //{
            //    var t = Time.time;
            //    if (t >= i.StartTime && t <= i.EndTime)
            //    {
            //        _activeTimelines.Add(i);

            //        if (i is MiningTimeline)
            //            State = States.Mine;
            //        else if (i is WalkTimeline)
            //            State = States.Move;
            //    }
            //}
        }

        public AntStateTimeline GetStateTimeline()
        {
            return _stateTimeline;
        }

        public PositionTimeline GetPositionTimeline()
        {
            return _positionTimeline;
        }
    }
}
