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
            validPosition = lastValidPosition;
        }
        return validPosition;
    }
    #endregion
    #region Unity's Methods
    void Start()
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
    bool IsFurnitureInsidePolygonXZ(FurnitureModel furniture, Vector3 pos, Quaternion rot)
    {
        Vector2[] corners = furniture.GetBottomCornersXZ(pos, rot);
        foreach (var corner in corners)
        {
            if (!PointInPolygon(corner, roomPolygon))
                return false;
        }
        return true;
    }

    bool PointInPolygon(Vector2 point, List<Vector2> polygon)
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

    private Vector3 GetClosestValidPosition(FurnitureModel furniture, Vector3 targetPos, Quaternion rotation)
    {
        const int maxIterations = 20;
        const float pushStep = 0.05f;

        Vector3 currentPos = targetPos;

        for (int i = 0; i < maxIterations; i++)
        {
            Vector2[] corners = furniture.GetBottomCornersXZ(currentPos, rotation);

            bool allInside = true;
            Vector2 correction = Vector2.zero;

            foreach (var corner in corners)
            {
                if (!PointInPolygon(corner, roomPolygon))
                {
                    allInside = false;

                    Vector2 roomCenter = roomPolygon.Aggregate(Vector2.zero, (sum, p) => sum + p) / roomPolygon.Count;
                    Vector2 pushDir = (roomCenter - corner).normalized;
                    correction += pushDir;
                }
            }

            if (allInside)
                return currentPos;

            if (correction != Vector2.zero)
            {
                Vector2 offset = correction.normalized * pushStep;
                currentPos += new Vector3(offset.x, 0f, offset.y);
            }
            else
            {
                currentPos = lastValidPosition;
                break; // No valid correction — escape loop
            }
        }

        return currentPos;
    }

    Vector2 ClosestPointOnLineSegment(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
    {
        Vector2 lineDir = lineEnd - lineStart;
        float lineLength = lineDir.magnitude;
        lineDir.Normalize();

        float projection = Vector2.Dot(point - lineStart, lineDir);
        projection = Mathf.Clamp(projection, 0, lineLength);

        return lineStart + lineDir * projection;
    }
    List<Vector2> BuildRoomPolygonFromWalls(List<Transform> walls)
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
