using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Bronk
{
	class GameEntity
	{
        internal enum States
        {
            Removable,
            Idle,
            Carry,
            Mine,
            Combat,
            Retreat, 
            Dead,
            Move
        }

        public float Health
        {
            get { return _health; }
            set { _health = value; }
        }

        public States State
        {
            get { return _state; }
            set { _state = value; }
        }

        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }

        private float _health;
        private States _state;

        public virtual void update(float deltatime) {}
	}
}
