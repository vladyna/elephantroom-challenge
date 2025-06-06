using GLTFast;
using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using Zenject;

public class FurnitureLoader : MonoBehaviour
{
    #region Serilized Fields
    [SerializeField] private string[] modelUrls = new string[]
    {
        "https://storage.googleapis.com/furniture-models/armchair/poltrona-biza_a3bc4a40-53e9-4be0-89f1-a7573d600971.glb",
        "https://storage.googleapis.com/furniture-models/dining-chair/victoria-ghost_8d4ea5cc-78a8-4edc-961d-17a3f6e83d4f.glb"
    };
    [SerializeField] private Transform placeHolder;
    [SerializeField] private LayerMask furnitureLayer;
    #endregion
    #region Private Variables
    private RoomService roomService;
    private DiContainer container;
    #endregion

    #region Injection 
    [Inject]
    public void Injection(RoomService roomService, DiContainer container)
    {
        this.roomService = roomService;
        this.container = container;
    }
    #endregion
    #region Unity's Methods
    private void Start()
    {
        foreach (var modelUrl in modelUrls)
        {
            StartCoroutine(LoadGltfModel(modelUrl));
        }
    }
    #endregion

    #region Private Methods

    IEnumerator LoadGltfModel(string gltfurl)
    {
        var gameObject = new GameObject();
        var gltfAsset = gameObject.GetComponent<GltfAsset>();
        if (gltfAsset == null)
        {
            gltfAsset = gameObject.AddComponent<GltfAsset>();
        }


        Task loadTask = LoadModelAsync(gltfAsset, gltfurl);

        yield return new WaitUntil(() => loadTask.IsCompleted);

        if (loadTask.IsCompletedSuccessfully)
        {
            Debug.Log("Model loaded successfully");
        }
        else
        {
            Debug.LogError("Failed to load the model.");
        }
    }

    private async Task LoadModelAsync(GltfAsset gltfAsset, string URL)
    {
        bool success = await gltfAsset.Load(URL);

  
        if (success)
        {
            gltfAsset.transform.SetParent(placeHolder);
            var furnitureModel = gltfAsset.AddComponent<FurnitureModel>();
            gltfAsset.AddComponent<BoxCollider>();
            gltfAsset.gameObject.layer = GetLayerFromMask(furnitureLayer);

            var position = roomService.RoomCenter;
            var meshRenderer = gltfAsset.GetComponentInChildren<MeshRenderer>();
            Debug.Log(meshRenderer.bounds);
            position.y = meshRenderer.bounds.extents.y - meshRenderer.bounds.center.y;

            gltfAsset.transform.position = position;
            container.Inject(furnitureModel);
        }
        else
        {
            Debug.LogError("Failed to load the GLTF model from URL");
        }
    }
    private int GetLayerFromMask(LayerMask mask)
    {
        int value = mask.value;
        for (int i = 0; i < 32; i++)
        {
            if ((value & (1 << i)) != 0)
                return i;
        }
        return -1; 
    }
    #endregion

}
