using DG.Tweening;
using UnityEditor.Search;
using UnityEngine;
using Zenject;

namespace Elephantroom.StateMachine
{
    public class MoveFurnitureState : IFurnitureState
    {
        #region Private Variables
        private RoomService roomService;
        private SelectFurnitureStateFactory selectFurnitureStateFactory;
        private GameObject selectedFurniture;
        private Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        private FurnitureStateMachine context;
        private Vector3 positionOffset;
        #endregion
        #region Constructor
        [Inject]
        public MoveFurnitureState(RoomService roomService, FurnitureStateMachine ctx, GameObject furniture, SelectFurnitureStateFactory selectFurnitureStateFactory)
        {
            context = ctx;
            selectedFurniture = furniture;
            this.roomService = roomService;
            this.selectFurnitureStateFactory = selectFurnitureStateFactory;
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
                Vector3 offsetedPoint = hitPoint + positionOffset;

                roomService.TryGetValidPositionAndRotationInsideRoom(selectedFurniture.GetComponent<FurnitureModel>(), offsetedPoint, selectedFurniture.transform.rotation, out Vector3 validPosition, out Quaternion validRotation);
                selectedFurniture.transform.position = validPosition;
                Sequence sequence = DOTween.Sequence();
                sequence.Append(selectedFurniture.transform.DORotate(validRotation.eulerAngles, 0.2f));
            }

            if (Input.GetMouseButtonDown(0))
            {
 
                context.SetState(selectFurnitureStateFactory.Create(context));
            }
        }

        public void Exit() 
        {
            selectedFurniture.GetComponent<ISelectable>().Deselect();
        }
        #endregion
    }
}