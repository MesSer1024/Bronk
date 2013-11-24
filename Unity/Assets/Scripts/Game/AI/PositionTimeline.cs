using UnityEngine;
using System.Collections;

public class PositionTimeline : Timeline<Vector3, Vector3>
{
	public static PositionTimeline Create ()
	{
		return Create <PositionTimeline, Vector3, Vector3>(Interpolate);
	}

	private static Vector3 Interpolate (Vector3 v1, Vector3 v2, float t)
	{
		return Vector3.Lerp (v1, v2, t);
	}
}
