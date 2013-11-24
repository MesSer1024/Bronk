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

        public void resetStateFuture()
        {
            //var oldState = _stateTimeline.GetValue(Time.time);
            _stateTimeline.removeKeyframesInFuture();
            //_stateTimeline.AddKeyframe(Time.time, oldState);
        }

        public void resetPositionFuture()
        {
            var oldPos = _positionTimeline.GetValue(Time.time);
            _positionTimeline.removeKeyframesInFuture();
            _positionTimeline.AddKeyframe(Time.time, oldPos);
        }

        public void addStateKeyframe(float time, GameEntity.States state) {
            _stateTimeline.AddKeyframe(time, state);
        }

        public void addPositionKeyframe(float time, Vector3 v)
        {
            _positionTimeline.AddKeyframe(time, v);
        }

        public override void update(float delta)
        {
            //unknown if this is needed
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
