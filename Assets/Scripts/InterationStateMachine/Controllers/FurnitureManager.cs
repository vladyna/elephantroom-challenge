using UnityEngine;
namespace Elephantroom.StateMachine
{
    public class FurnitureManager : MonoBehaviour
    {
        #region Serialized Variables
        [SerializeField] private GameObject furniturePrefab;
        [SerializeField] private LayerMask furnitureLayer;
        #endregion

        #region Private Variables
        private FurnitureStateMachine stateMachine;
        #endregion
        #region Unity's Methods
        private void Start()
        {
            stateMachine = GetComponent<FurnitureStateMachine>();
            stateMachine.SetState(new SelectFurnitureState(stateMachine));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
                stateMachine.SetState(new PlaceFurnitureState(stateMachine, furniturePrefab));

            if (Input.GetKeyDown(KeyCode.M))
            {
                GameObject selected = GameObject.FindWithTag("Furniture");
                if (selected)
                    stateMachine.SetState(new MoveFurnitureState(stateMachine, selected));
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                GameObject selected = GameObject.FindWithTag("Furniture");
                if (selected)
                    stateMachine.SetState(new RotateFurnitureState(stateMachine, selected));
            }
        }
        #endregion
    }
}
