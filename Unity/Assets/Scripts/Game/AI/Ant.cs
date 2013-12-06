using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Bronk
{
    public class Ant : GameEntity
	{
		public float MoveSpeed = 3f;
        private AntStateTimeline _stateTimeline;
        private PositionTimeline _positionTimeline;
		private JobTimeline _jobTimeline;


        public override Vector3 Position
        {
            get
            {
                return _positionTimeline.GetValue(Game.LogicTime);
            }
            set
            {
                _positionTimeline.AddKeyframe(Game.LogicTime, value);
            }
        }

		public override StateData State
        {
            get
            {
				return _stateTimeline.GetValue(Game.LogicTime);
            }
            set
            {
				_stateTimeline.AddKeyframe(Game.LogicTime, value);
            }
        }

		public Ant(int id)
			: base(id)
        {
            _stateTimeline = AntStateTimeline.Create();
            _positionTimeline = PositionTimeline.Create();
			_jobTimeline = JobTimeline.Create ();
			_stateTimeline.AddKeyframe(0, new StateData(GameEntity.States.Idle));
        }

        public void init()
        {

        }

        public void resetStateFuture()
        {
			_stateTimeline.removeKeyframesInFuture(Game.LogicTime);
        }

        public void resetPositionFuture()
        {
            var oldPos = _positionTimeline.GetValue(Game.LogicTime);
			_positionTimeline.removeKeyframesInFuture(Game.LogicTime);
            _positionTimeline.AddKeyframe(Game.LogicTime, oldPos);
        }

		public void addStateKeyframe(float time, StateData state) {
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

		public JobTimeline GetJobTimeline()
		{
			return _jobTimeline;
		}
    }
}
