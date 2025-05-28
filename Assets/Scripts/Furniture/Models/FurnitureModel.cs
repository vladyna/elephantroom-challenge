using UnityEngine;

public class FurnitureModel : MonoBehaviour, ISelectable
{
    #region ISelectable Implementation
    public void Select()
    {
        Debug.Log("Selected Furniture");
    }

    public void Deselect()
    {
        Debug.Log("Deselected Furniture");
    }
    #endregion
}
