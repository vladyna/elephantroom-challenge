using System.Collections.Generic;
using UnityEngine;

public static class FurnitureGeometry
{
    public static bool IsFullyInsidePolygon(FurnitureModel furniture, Vector3 position, Quaternion rotation, List<Vector2> polygon)
    {
        foreach (var corner in furniture.GetBottomCornersXZ(position, rotation))
        {
            if (!GeometryUtils.PointInPolygon(corner, polygon))
                return false;
        }
        return true;
    }

    public static Vector3 ClosestCardinalDirection(FurnitureModel furniture, Vector3 desiredDirection)
    {
        Vector3[] directions = {
            furniture.transform.forward,
            -furniture.transform.forward,
            furniture.transform.right,
            -furniture.transform.right
        };

        float minAngle = float.MaxValue;
        Vector3 bestDir = directions[0];

        foreach (var dir in directions)
        {
            float angle = Vector3.Angle(desiredDirection, dir);
            if (angle < minAngle)
            {
                minAngle = angle;
                bestDir = dir;
            }
        }

        return bestDir;
    }
}
