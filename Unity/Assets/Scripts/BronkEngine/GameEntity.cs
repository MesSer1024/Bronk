using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Bronk
{
    public class GameEntity
	{
        public enum States
        {
            Removable,
            Idle,
            Carry,
            Mine,
            Combat,
            Retreat, 
            Dead,
            Move,
			Sleep
        }

        public virtual States State
        {
            get { return _state; }
            set { _state = value; }
        }

        public virtual Vector3 Position {
            get { return _position; }
            set { _position = value; }
        }

        private float _health;
        protected States _state;
        protected Vector3 _position;

        public virtual void update(float deltatime) {}
	}
}
