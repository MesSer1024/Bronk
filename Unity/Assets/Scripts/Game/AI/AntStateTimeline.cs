using UnityEngine;
using Bronk;
using System.Collections;

public class AntStateTimeline : Timeline<GameEntity.States, GameEntity.States>
{
	public override TimelineType Type {
		get {
			return TimelineType.AntState;
		}
	}
	public static AntStateTimeline Create ()
	{
		return Create <AntStateTimeline, GameEntity.States, GameEntity.States>(Interpolate);
	}

	private static GameEntity.States Interpolate (GameEntity.States v1, GameEntity.States v2, float t)
	{
		if (t == 1)
			return v2;
		return v1;
	}
}
