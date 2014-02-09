using UnityEngine;
using System.Collections;
using Bronk;

public interface ITimelineObject
{
	ITimeline GetTimeline (TimelineType type);
}

public abstract class ICarryObject
{
    public GameObject View { get; set; }
    public int BlockId;
    public int ItemId;

    public ICarryObject(int blockId, int itemId)
    {
        BlockId = blockId;
        ItemId = itemId;
    }
}