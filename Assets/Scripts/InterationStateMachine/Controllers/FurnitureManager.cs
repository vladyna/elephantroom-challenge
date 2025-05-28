using UnityEngine;
using Zenject;

namespace Elephantroom.StateMachine
{
    public class FurnitureManager : MonoBehaviour
    {
        #region Serialized Variables
        [SerializeField] private LayerMask furnitureLayer;
        #endregion

        #region Private Variables
        private FurnitureStateMachine stateMachine;
        private MoveFurnitureStateFactory moveFurnitureStateFactory;
        private SelectFurnitureStateFactory selectFurnitureStateFactory;
        #endregion
        #region Injection
        [Inject]
        private void Injection(MoveFurnitureStateFactory moveFurnitureStateFactory, SelectFurnitureStateFactory selectFurnitureStateFactory)
        {
            this.moveFurnitureStateFactory = moveFurnitureStateFactory;
            this.selectFurnitureStateFactory = selectFurnitureStateFactory;
        }
        #endregion

        #region Unity's Methods
        private void Start()
        {
            stateMachine = GetComponent<FurnitureStateMachine>();
            stateMachine.SetState(selectFurnitureStateFactory.Create(stateMachine));
        }
        #endregion
    }
}
