using UnityEngine;
using System.Collections;
using Bronk;

public interface ITimelineObject
{
	ITimeline GetTimeline (TimelineType type);
}