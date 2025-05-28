using System.Collections.Generic;
using UnityEngine;

public class OutlineService
{
    #region Private Fields
    private Outline prefabOutline;
    #endregion

    #region Constructor
    public OutlineService(Outline prefabOutline)
    {
        this.prefabOutline = prefabOutline;
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
            Object.Destroy(outline);
        }
    }
    #endregion
}
