using UnityEngine;
namespace Elephantroom.StateMachine
{
    public class MoveFurnitureState : IFurnitureState
    {
        #region Private Variables
        private GameObject selectedFurniture;
        private Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        private FurnitureStateMachine context;
        private Vector3 positionOffset;
        #endregion
        #region Constructor
        public MoveFurnitureState(FurnitureStateMachine ctx, GameObject furniture)
        {
            context = ctx;
            selectedFurniture = furniture;
        }
        #endregion
        #region IFurnitureState Implementation
        public void Enter()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (groundPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                positionOffset = selectedFurniture.transform.position - hitPoint;
            }
        }

        public void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (groundPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                selectedFurniture.transform.position = hitPoint + positionOffset;
            }

            if (Input.GetMouseButtonDown(0))
            {
 
                context.SetState(new SelectFurnitureState(context));
            }
        }

        public void Exit() 
        {
            selectedFurniture.GetComponent<ISelectable>().Deselect();
        }
        #endregion
    }
}