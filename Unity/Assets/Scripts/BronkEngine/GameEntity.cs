﻿using System;
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
            WaitingForOtherJobToFinish,
            Carry,
            Mine,
            Combat,
            Retreat, 
            Dead,
            Move,
			Sleep,
			PickUp,
			DropOff
        }

		public virtual StateData State
        {
            get { return _state; }
            set { _state = value; }
        }

        public virtual Vector3 Position {
            get { return _position; }
            set { _position = value; }
        }
		public int ID { get { return _ID; } }
		private int _ID;
        private float _health;
		protected StateData _state;
        protected Vector3 _position;

		public GameEntity(int id)
		{
			_ID = id;
		}

        public virtual void update(float deltatime) {}
	}
}
