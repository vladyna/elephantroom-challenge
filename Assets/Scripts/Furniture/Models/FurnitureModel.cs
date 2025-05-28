using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

[RequireComponent(typeof(BoxCollider))]
public class FurnitureModel : MonoBehaviour, ISelectable
{
    #region Private Variables
    private BoxCollider boxCollider;
    private OutlineService outlineService;
    #endregion

    #region Injection
    [Inject]
    private void Injection(OutlineService outlineService)
    {
        this.outlineService = outlineService;
    }
    #endregion

    #region Unity's Methods
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            Debug.LogWarning($"No box collider in furniture{name}");
            return;
        }
    }
    #endregion

    #region Public Methods
    public Vector2[] GetBottomCornersXZ(Vector3 pos, Quaternion rot)
    {
        Vector3 worldScale = transform.lossyScale;
        Vector3 scaledSize = Vector3.Scale(boxCollider.size, worldScale);
        Vector3 scaledCenter = Vector3.Scale(boxCollider.center, worldScale);
        Vector3[] offsets = new Vector3[]
        {
        new Vector3(-1, 0, -1),
        new Vector3(1, 0, -1),
        new Vector3(1, 0, 1),
        new Vector3(-1, 0, 1),
        };

        Vector2[] result = new Vector2[4];
        for (int i = 0; i < 4; i++)
        {
            Vector3 localCorner = scaledCenter + Vector3.Scale(offsets[i], scaledSize * 0.5f);
            Vector3 worldCorner = pos + rot * localCorner;
            result[i] = new Vector2(worldCorner.x, worldCorner.z);
        }

        return result;
    }
    #endregion

    #region ISelectable Implementation
    public void Select()
    {
        outlineService.AddOutline(gameObject);
    }

    public void Deselect()
    {
        outlineService.RemoveOutline(gameObject);
    }
    #endregion
}
