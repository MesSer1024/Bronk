using UnityEngine;
using Bronk;
using System.Collections;

public class SelectedTimeline : Timeline<bool, bool>
{
	public override TimelineType Type {
		get {
			return TimelineType.Selected;
		}
	}
	public static SelectedTimeline Create ()
	{
		return Create <SelectedTimeline, bool, bool>(Interpolate);
	}

	private static bool Interpolate (bool v1, bool v2, float t)
	{
		if (t == 1)
			return v2;
		return v1;
	}
}
