using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class RoomPolygonBuilder
{
    public static List<Vector2> BuildPolygonFromWalls(List<Transform> walls, out Vector3 center)
    {
        List<Vector2> points = new List<Vector2>();

        foreach (var wall in walls)
        {
            BoxCollider collider = wall.GetComponent<BoxCollider>();
            if (!collider) continue;

            Vector3 size = Vector3.Scale(collider.size, wall.lossyScale) * 0.5f;
            Vector3 centerPos = wall.TransformPoint(collider.center);
            Vector3 right = wall.right;
            Vector3 forward = wall.forward;

            Vector3 innerOffset = -forward * size.z;
            Vector3 corner1 = centerPos + innerOffset + right * size.x;
            Vector3 corner2 = centerPos + innerOffset - right * size.x;

            points.Add(new Vector2(corner1.x, corner1.z));
            points.Add(new Vector2(corner2.x, corner2.z));
        }

        Vector2 avgCenter = points.Aggregate(Vector2.zero, (sum, p) => sum + p) / points.Count;
        center = new Vector3(avgCenter.x, 0f, avgCenter.y);

        return points.OrderBy(p => Mathf.Atan2(p.y - avgCenter.y, p.x - avgCenter.x)).ToList();
    }
}
