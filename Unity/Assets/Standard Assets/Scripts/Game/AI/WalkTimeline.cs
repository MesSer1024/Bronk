using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Bronk
{
	class WalkTimeline : ITimeline
	{
        private Vector3 _startPos;
        private Vector3 _endPos;
        private float _startTime;
        private float _endTime;

        private GameEntity _unit;

        public WalkTimeline(Vector3 startPos, Vector3 endPos, float startTime, float endTime, GameEntity unit)
        {
            _startPos = startPos;
            _endPos = endPos;
            _startTime = startTime;
            _endTime = endTime;
            _unit = unit;
        }

        public Vector3 getPosition(float gameTime)
        {
            if (gameTime < _startTime || gameTime > _endTime)
                throw new Exception("Invalid time sent to this particular job (does this need to be handled with clamping time or on a bigger scale JobManager... ?)");

            return Vector3.Lerp(_startPos, _endPos, (gameTime - _startTime) / (_endTime - _startTime));
        }

        public float StartTime
        {
            get { return _startTime; }
        }

        public float EndTime
        {
            get { return _endTime; }
        }
    }
}
