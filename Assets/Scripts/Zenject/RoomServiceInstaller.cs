using Elephantroom.StateMachine;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class RoomServiceInstaller : MonoInstaller
{
    #region Serilized Variables
    [SerializeField] private List<Transform> wallPoints;
    #endregion
    public override void InstallBindings()
    {
        var roomService = new RoomService(wallPoints);
        Container.Bind<RoomService>().FromInstance(roomService).AsSingle();
        Container.BindFactory<FurnitureStateMachine, GameObject, MoveFurnitureState, MoveFurnitureStateFactory>();
        Container.BindFactory<FurnitureStateMachine, SelectFurnitureState, SelectFurnitureStateFactory>();
    }
}
