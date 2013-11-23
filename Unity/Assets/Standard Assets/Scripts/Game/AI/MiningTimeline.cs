using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Bronk
{
	class MiningTimeline : ITimeline
	{
        private float _endTime;
        private float _startTime;
        private CubeData _cube;

        public MiningTimeline(CubeData cube, float startTime, float endTime)
        {
            _cube = cube;
            _startTime = startTime;
            _endTime = endTime;

            if (Mathf.Approximately(_startTime, _endTime) || _startTime > _endTime)
            {
                throw new Exception("invalid input time, either starttime=endtime or starttime > endtime");
            }
        }

        public float getCompletionStatus(float gameTime)
        {
            var t = Math.Min(Math.Max(_startTime, gameTime), _endTime);
            return Mathf.Lerp(_startTime, _endTime, t - _startTime);
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
