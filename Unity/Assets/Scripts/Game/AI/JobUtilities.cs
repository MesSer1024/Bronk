using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Bronk;

public static class JobUtilities
{
    public static void FillPositionTimeline(Ant ant, List<Pathfinding.Node> path, ref float dt, float moveSpeed, ref Vector2 lastPosition)
    {
        for (int nodeIndex = 1; nodeIndex < path.Count - 1; nodeIndex++)
        {
            var node = path[nodeIndex];
            Vector2 blockPosition = Game.World.getCubePosition(node.blockID);
            float distance = (blockPosition - lastPosition).magnitude;
            dt += distance / moveSpeed;
            ant.addPositionKeyframe(dt, blockPosition);
            lastPosition = blockPosition;
        }
    }
}
