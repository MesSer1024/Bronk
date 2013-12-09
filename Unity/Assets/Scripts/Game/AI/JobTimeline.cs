using UnityEngine;
using System.Collections;

namespace Bronk
{
    public class JobTimeline : Timeline<IJob, IJob>
	{
		public override TimelineType Type {
			get {
				return TimelineType.Job;
			}
		}

		public static JobTimeline Create ()
		{
            return Create<JobTimeline, IJob, IJob>(Interpolate);
		}

        private static IJob Interpolate(IJob v1, IJob v2, float t)
		{
			if (t == 1)
				return v2;
			return v1;
		}
	}
}