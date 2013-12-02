using UnityEngine;
using System.Collections;

public class PositionTimeline : Timeline<Vector2, Vector2>
{
	public override TimelineType Type {
		get {
			return TimelineType.Position;
		}
	}
	public static PositionTimeline Create ()
	{
		return Create <PositionTimeline, Vector2, Vector2>(Interpolate);
	}

	private static Vector2 Interpolate (Vector2 v1, Vector2 v2, float t)
	{
		return Vector2.Lerp (v1, v2, t);
	}
}
