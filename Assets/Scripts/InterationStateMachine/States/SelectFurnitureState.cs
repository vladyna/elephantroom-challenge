using UnityEngine;
using Zenject;

namespace Elephantroom.StateMachine
{
    public class SelectFurnitureState : IFurnitureState
    {
        #region Private Variables
        private FurnitureStateMachine context;
        private LayerMask furnitureLayer = 64;
        private MoveFurnitureStateFactory moveFurnitureStateFactory;
        #endregion
        #region Constructor
        [Inject]
        public SelectFurnitureState(FurnitureStateMachine ctx, MoveFurnitureStateFactory moveFurnitureStateFactory)
        {
            context = ctx;
            this.moveFurnitureStateFactory = moveFurnitureStateFactory;
        }
        #endregion
        #region IFurnitureState Implementations
        public void Enter()
        {
            Debug.Log("Enter SelectFurnitureState");
        }

        public void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 100f, furnitureLayer))
                {
                    GameObject selected = hit.collider.gameObject;
                    ISelectable selectable = selected.GetComponent<ISelectable>();
                    if (selectable != null)
                    {
                        selectable.Select();
                        context.SetState(moveFurnitureStateFactory.Create(context, selected));
                    }
                }
            }
        }

        public void Exit()
        {
            Debug.Log("Exit SelectFurnitureState");
        }
        #endregion
    }
}
