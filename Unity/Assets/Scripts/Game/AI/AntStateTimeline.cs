using UnityEngine;
using Bronk;
using System.Collections;
public struct StateData
{
	public GameEntity.States State;
	public int BlockID;
    public GoldObject Gold;

	public StateData (GameEntity.States state, int dataId = -1)
	{
		State = state;
		BlockID = dataId;
        Gold = null;
	}
}
public class AntStateTimeline : Timeline<StateData, StateData>
{
	public override TimelineType Type {
		get {
			return TimelineType.AntState;
		}
	}
	public static AntStateTimeline Create ()
	{
		return Create <AntStateTimeline, StateData, StateData>(Interpolate);
	}

	private static StateData Interpolate (StateData v1, StateData v2, float t)
	{
		if (t == 1)
			return v2;
		return v1;
	}
}
