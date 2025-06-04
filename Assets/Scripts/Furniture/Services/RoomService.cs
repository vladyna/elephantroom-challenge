using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomService
{
    #region Private Fields
    private List<Vector2> roomPolygon;
    private Vector3 lastValidPosition = Vector3.zero;
    private Vector3 roomCenter;
    private Vector2 roomCenterPlane;
    #endregion

    #region Public Properties
    public Vector3 RoomCenter => roomCenter;
    #endregion

    #region Constructor
    public RoomService(List<Transform> wallPoints)
    {
        roomPolygon = RoomPolygonBuilder.BuildPolygonFromWalls(wallPoints, out roomCenter);
        roomCenterPlane = new Vector2(roomCenter.x, roomCenter.z);
    }
    #endregion

    #region Public Methods
    public void TryGetValidPositionAndRotationInsideRoom(FurnitureModel furniture, Vector3 position, Quaternion rotation, out Vector3 validPosition, out Quaternion validRotation)
    {
        if (FurnitureGeometry.IsFullyInsidePolygon(furniture, position, rotation, roomPolygon))
        {
            validPosition = lastValidPosition = position;
            validRotation = rotation;
        }
        else
        {
            validPosition = GetClosestValidPosition(furniture, position, rotation, out validRotation);
        }
    }
    #endregion

    #region Private Methods
    private Vector3 GetClosestValidPosition(FurnitureModel furniture, Vector3 targetPos, Quaternion currentRotation, out Quaternion bestRotation)
    {
        if (FurnitureGeometry.IsFullyInsidePolygon(furniture, targetPos, currentRotation, roomPolygon))
        {
            bestRotation = currentRotation;
            return lastValidPosition = targetPos;
        }

        if (TryFindWallAlignedRotation(furniture, targetPos, currentRotation, out Vector3 adjustedPosition, out bestRotation))
        {
            return lastValidPosition = adjustedPosition;
        }

        return TryPushFurnitureInward(furniture, targetPos, currentRotation, out bestRotation);
    }

    private bool TryFindWallAlignedRotation(FurnitureModel furniture, Vector3 position, Quaternion rotation, out Vector3 newPosition, out Quaternion newRotation)
    {
        Vector2[] corners = furniture.GetBottomCornersXZ(position, rotation);
        Vector2 closestCorner = Vector2.zero;
        Vector2 closestEdgePoint = Vector2.zero;
        Vector2 closestEdgeDirection = Vector2.zero;
        float closestDist = float.MaxValue;

        foreach (var corner in corners)
        {
            if (GeometryUtils.PointInPolygon(corner, roomPolygon)) continue;

            for (int i = 0; i < roomPolygon.Count; i++)
            {
                Vector2 p1 = roomPolygon[i];
                Vector2 p2 = roomPolygon[(i + 1) % roomPolygon.Count];
                Vector2 pointOnEdge = GeometryUtils.ClosestPointOnSegment(corner, p1, p2);

                float dist = Vector2.Distance(corner, pointOnEdge);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestCorner = corner;
                    closestEdgePoint = pointOnEdge;
                    closestEdgeDirection = (p2 - p1).normalized;
                }
            }
        }

        if (closestDist < float.MaxValue)
        {
            Vector2 wallNormal2D = new Vector2(-closestEdgeDirection.y, closestEdgeDirection.x);
            Vector2 toCenter = (roomCenterPlane - closestEdgePoint).normalized;
            if (Vector2.Dot(toCenter, wallNormal2D) < 0)
                wallNormal2D = -wallNormal2D;

            Vector3 wallNormal3D = new Vector3(wallNormal2D.x, 0f, wallNormal2D.y);
            Vector3 closestAlign = FurnitureGeometry.ClosestCardinalDirection(furniture, -wallNormal3D);
            Quaternion wallRotation = Quaternion.LookRotation(-wallNormal3D, Vector3.up) * Quaternion.AngleAxis(Vector3.Angle(furniture.transform.forward, closestAlign), Vector3.up);

            if (FurnitureGeometry.IsFullyInsidePolygon(furniture, position, wallRotation, roomPolygon))
            {
                newRotation = wallRotation;
                newPosition = position;
                return true;
            }
        }

        newRotation = rotation;
        newPosition = position;
        return false;
    }

    private Vector3 TryPushFurnitureInward(FurnitureModel furniture, Vector3 startPos, Quaternion rotation, out Quaternion bestRotation)
    {
        const int MaxTries = 20;
        const float Step = 0.05f;
        Vector3 currentPos = startPos;

        for (int i = 0; i < MaxTries; i++)
        {
            if (FurnitureGeometry.IsFullyInsidePolygon(furniture, currentPos, rotation, roomPolygon))
            {
                bestRotation = rotation;
                return lastValidPosition = currentPos;
            }

            Vector2[] testCorners = furniture.GetBottomCornersXZ(currentPos, rotation);
            Vector2 pushDir = Vector2.zero;

            foreach (var corner in testCorners)
            {
                if (!GeometryUtils.PointInPolygon(corner, roomPolygon))
                {
                    pushDir += (roomCenterPlane - corner).normalized;
                }
            }

            if (pushDir != Vector2.zero)
            {
                Vector2 offset = pushDir.normalized * Step;
                currentPos += new Vector3(offset.x, 0f, offset.y);
            }
            else
                break;
        }

        bestRotation = rotation;
        return lastValidPosition;
    }
    #endregion
}
