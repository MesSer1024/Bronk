using UnityEngine;
using System.Collections;

namespace Bronk
{
	public class JobTimeline : Timeline<DigJob, DigJob>
	{
		public override TimelineType Type {
			get {
				return TimelineType.Job;
			}
		}

		public static JobTimeline Create ()
		{
			return Create <JobTimeline, DigJob, DigJob> (Interpolate);
		}

		private static DigJob Interpolate (DigJob v1, DigJob v2, float t)
		{
			if (t == 1)
				return v2;
			return v1;
		}
	}
}