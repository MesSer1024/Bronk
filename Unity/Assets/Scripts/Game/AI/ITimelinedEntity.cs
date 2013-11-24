using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Bronk
{
	public interface ITimelinedEntity
	{
        AntStateTimeline GetStateTimeline();
        PositionTimeline GetPositionTimeline();

        //void AddStateKeyframe(KeyframeItem<GameEntity.States> state);
        //void AddPositionKeyframe(KeyframeItem<Vector3> state);
	}
}
