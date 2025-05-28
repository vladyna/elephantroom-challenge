using UnityEngine;
namespace Elephantroom.StateMachine
{
    public class PlaceFurnitureState : IFurnitureState
    {
        #region Private Variables
        private FurnitureStateMachine context;
        private GameObject furniturePrefab;
        private GameObject selectedFurniture;
        private Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        #endregion
        #region Constructor
        public PlaceFurnitureState(FurnitureStateMachine ctx, GameObject prefab)
        {
            context = ctx;
            furniturePrefab = prefab;
        }
        #endregion
        #region Public Methods
        public void Enter()
        {
            selectedFurniture = GameObject.Instantiate(furniturePrefab);
        }

        public void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (groundPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                selectedFurniture.transform.position = hitPoint;
            }

            if (Input.GetMouseButtonDown(0))
            {
            }
        }

        public void Exit()
        {
            selectedFurniture.GetComponent<ISelectable>().Deselect();
            selectedFurniture = null;
        }
        #endregion
    }
}
