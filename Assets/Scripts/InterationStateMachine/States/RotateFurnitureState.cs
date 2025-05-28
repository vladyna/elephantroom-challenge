using UnityEngine;
namespace Elephantroom.StateMachine
{
    public class RotateFurnitureState : IFurnitureState
    {
        #region Private Variables
        private GameObject selectedFurniture;
        private float rotationSpeed = 100f;
        private FurnitureStateMachine context;
        #endregion
        #region Constructors
        public RotateFurnitureState(FurnitureStateMachine ctx, GameObject selected)
        {
            context = ctx;
            selectedFurniture = selected;
        }
        #endregion
        #region IFurnitureState Implementations
        public void Enter() { }

        public void Update()
        {
            if (Input.GetKey(KeyCode.LeftArrow))
                selectedFurniture.transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
            else if (Input.GetKey(KeyCode.RightArrow))
                selectedFurniture.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

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
