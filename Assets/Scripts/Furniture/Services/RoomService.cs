using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

// NOTE(): Do a zenject instead of Singleton
public class RoomService : MonoBehaviour
{
    #region Serilized Variables
    [SerializeField] private List<Transform> wallPoints;
    #endregion
    #region Private Variables
    private List<Vector2> roomPolygon;
    private Camera cam;
    private Vector3 lastValidPosition;
    private static RoomService _instance;
    #endregion
    #region Public Properties
    public static RoomService Instance => _instance;
    #endregion
    #region Public Methods
    public Vector3 GetValidPositionInsideRoom(FurnitureModel furniture, Vector3 position, Quaternion rotation)
    {
        Vector3 validPosition;
        if (IsFurnitureInsidePolygonXZ(furniture, position, rotation))
        {
            validPosition = position;
            lastValidPosition = position;
        }
        else
        {
            validPosition = GetClosestValidPosition(furniture, position, rotation, out Quaternion newrotation);
            furniture.transform.rotation = newrotation;
        }
        return validPosition;
    }
    #endregion
    #region Unity's Methods
    private void Start()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        _instance = this;
        cam = Camera.main;
        lastValidPosition = transform.position;

        roomPolygon = BuildRoomPolygonFromWalls(wallPoints);
    }
    #endregion

    #region Private Methods
    private bool IsFurnitureInsidePolygonXZ(FurnitureModel furniture, Vector3 pos, Quaternion rot)
    {
        Vector2[] corners = furniture.GetBottomCornersXZ(pos, rot);
        foreach (var corner in corners)
        {
            if (!PointInPolygon(corner, roomPolygon))
                return false;
        }
        return true;
    }

    private bool PointInPolygon(Vector2 point, List<Vector2> polygon)
    {
        bool inside = false;
        int count = polygon.Count;
        for (int i = 0, j = count - 1; i < count; j = i++)
        {
            Vector2 pi = polygon[i];
            Vector2 pj = polygon[j];

            if ((pi.y > point.y) != (pj.y > point.y) &&
                point.x < (pj.x - pi.x) * (point.y - pi.y) / (pj.y - pi.y) + pi.x)
            {
                inside = !inside;
            }
        }
        return inside;
    }


    private Vector3 GetClosestValidPosition(FurnitureModel furniture, Vector3 targetPos, Quaternion currentRotation, out Quaternion bestRotation)
    {
        Vector2[] corners = furniture.GetBottomCornersXZ(targetPos, currentRotation);
        Vector2 roomCenter = roomPolygon.Aggregate(Vector2.zero, (sum, p) => sum + p) / roomPolygon.Count;

        if (corners.All(c => PointInPolygon(c, roomPolygon)))
        {
            bestRotation = currentRotation;
            lastValidPosition = targetPos;
            return targetPos;
        }

        Vector2 closestCorner = Vector2.zero;
        Vector2 closestPointOnEdge = Vector2.zero;
        Vector2 closestEdgeDir = Vector2.zero;
        float closestDist = float.MaxValue;

        foreach (var corner in corners)
        {
            if (PointInPolygon(corner, roomPolygon)) continue;

            for (int i = 0; i < roomPolygon.Count; i++)
            {
                Vector2 p1 = roomPolygon[i];
                Vector2 p2 = roomPolygon[(i + 1) % roomPolygon.Count];

                Vector2 pointOnEdge = ClosestPointOnLineSegment(corner, p1, p2);
                float dist = Vector2.Distance(corner, pointOnEdge);

                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestCorner = corner;
                    closestPointOnEdge = pointOnEdge;
                    closestEdgeDir = (p2 - p1).normalized;
                }
            }
        }

        if (closestDist < float.MaxValue)
        {
            Vector2 wallNormal2D = new Vector2(-closestEdgeDir.y, closestEdgeDir.x);
            Vector2 toCenter = (roomCenter - closestPointOnEdge).normalized;

            if (Vector2.Dot(toCenter, wallNormal2D) < 0)
                wallNormal2D = -wallNormal2D;

            Vector3 wallNormal3D = new Vector3(wallNormal2D.x, 0f, wallNormal2D.y);
            Quaternion wallRotation = Quaternion.LookRotation(-wallNormal3D, Vector3.up);

            if (IsObjectFullyInside(furniture, targetPos, wallRotation))
            {
                bestRotation = wallRotation;
                lastValidPosition = targetPos;
                return targetPos;
            }
        }
        const int maxIterations = 20;
        const float pushStep = 0.05f;
        Vector3 currentPos = targetPos;

        for (int i = 0; i < maxIterations; i++)
        {
            if (IsObjectFullyInside(furniture, currentPos, currentRotation))
            {
                bestRotation = currentRotation;
                lastValidPosition = currentPos;
                return currentPos;
            }

            Vector2[] testCorners = furniture.GetBottomCornersXZ(currentPos, currentRotation);
            Vector2 pushDir = Vector2.zero;

            foreach (var corner in testCorners)
            {
                if (!PointInPolygon(corner, roomPolygon))
                    pushDir += (roomCenter - corner).normalized;
            }

            if (pushDir != Vector2.zero)
            {
                Vector2 offset = pushDir.normalized * pushStep;
                currentPos += new Vector3(offset.x, 0f, offset.y);
            }
            else
            {
                break;
            }
        }

        bestRotation = currentRotation;
        return lastValidPosition;
    }
    private Vector2 ClosestPointOnLineSegment(Vector2 point, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        float t = Vector2.Dot(point - a, ab) / ab.sqrMagnitude;
        t = Mathf.Clamp01(t);
        return a + t * ab;
    }
    private bool IsObjectFullyInside(FurnitureModel furniture, Vector3 pos, Quaternion rot)
    {
        Vector2[] corners = furniture.GetBottomCornersXZ(pos, rot);
        foreach (var corner in corners)
        {
            if (!PointInPolygon(corner, roomPolygon))
                return false;
        }
        return true;
    }
    private List<Vector2> BuildRoomPolygonFromWalls(List<Transform> walls)
    {
        List<Vector2> corners = new List<Vector2>();

        foreach (var wall in walls)
        {
            var collider = wall.GetComponent<BoxCollider>();
            if (collider == null) continue;
            Transform tr = collider.transform;
            Vector3 center = tr.TransformPoint(collider.center);
            Vector3 halfSize = Vector3.Scale(collider.size * 0.5f, tr.lossyScale);
            Vector3[] localCorners = new Vector3[]
            {
                new Vector3(-halfSize.x, 0, -halfSize.z),
                new Vector3(halfSize.x, 0, -halfSize.z),
                new Vector3(halfSize.x, 0, halfSize.z),
                new Vector3(-halfSize.x, 0, halfSize.z),
            };

            foreach (var local in localCorners)
            {
                Vector3 world = center + tr.rotation * local;
                corners.Add(new Vector2(world.x, world.z));
            }
        }
        Vector2 centerPoint = corners.Aggregate(Vector2.zero, (sum, p) => sum + p) / corners.Count;

        return corners.OrderBy(p => Mathf.Atan2(p.y - centerPoint.y, p.x - centerPoint.x)).ToList();
    }
    #endregion
}
