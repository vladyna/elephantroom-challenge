using UnityEngine;
namespace Elephantroom.StateMachine
{
    public class FurnitureStateMachine : MonoBehaviour
    {
        #region Private Variables
        private IFurnitureState currentState;
        #endregion
        #region Public Methods
        public void SetState(IFurnitureState newState)
        {
            if (currentState != null)
                currentState.Exit();

            currentState = newState;
            currentState.Enter();
        }
        #endregion
        #region Unity's Methods

        private void Update()
        {
            if (currentState != null)
                currentState.Update();
        }
        #endregion
    }
}
