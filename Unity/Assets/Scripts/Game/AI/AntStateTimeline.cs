using UnityEngine;
using System.Collections;

public enum AntState
{
	Idle,
	Walking,
	Mining,
}
public class AntStateTimeline : Timeline<AntState, AntState>
{
	public AntStateTimeline Create ()
	{
		return Create <AntStateTimeline, AntState, AntState>(Interpolate);
	}

	private static AntState Interpolate (AntState v1, AntState v2, float t)
	{
		if (t == 1)
			return v2;
		return v1;
	}
}
