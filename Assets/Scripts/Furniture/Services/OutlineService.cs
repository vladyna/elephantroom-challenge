using System.Collections.Generic;
using UnityEngine;

public class OutlineService : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private Outline prefabOutline;
    #endregion

    #region Private Variables

    private static OutlineService _instance;
    #endregion
    #region Public Properties
    public static OutlineService Instance => _instance;
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
    }
    #endregion

    #region Public Methods
    public void AddOutline(GameObject gameobject)
    {
        var outline = gameobject.AddComponent<Outline>();
        outline.OutlineColor = prefabOutline.OutlineColor;
        outline.OutlineWidth = prefabOutline.OutlineWidth;
    }
    public void RemoveOutline(GameObject gameobject)
    {
        var outline = gameobject.GetComponent<Outline>();
        if (outline != null)
        {
            Destroy (outline);
        }
    }
    #endregion
}
