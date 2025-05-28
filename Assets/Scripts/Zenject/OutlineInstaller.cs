using UnityEngine;
using Zenject;

public class OutlineInstaller : MonoInstaller
{
    #region Serialized Fields
    [SerializeField] private Outline prefabOutline;
    #endregion
    public override void InstallBindings()
    {
        var outlineService = new OutlineService(prefabOutline);
        Container.Bind<OutlineService>().FromInstance(outlineService).AsSingle();
    }
}
