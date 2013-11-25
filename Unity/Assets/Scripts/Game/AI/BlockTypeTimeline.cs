using UnityEngine;
using Bronk;
using System.Collections;

public class BlockTypeTimeline : Timeline<GameWorld.BlockType, GameWorld.BlockType>
{
	public override TimelineType Type {
		get {
			return TimelineType.BlockType;
		}
	}
	public static BlockTypeTimeline Create ()
	{
		return Create <BlockTypeTimeline, GameWorld.BlockType, GameWorld.BlockType>(Interpolate);
	}

	private static GameWorld.BlockType Interpolate (GameWorld.BlockType v1, GameWorld.BlockType v2, float t)
	{
		if (t == 1)
			return v2;
		return v1;
	}
}
