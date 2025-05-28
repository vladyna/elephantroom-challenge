using GLTFast;
using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public class FurnitureLoader : MonoBehaviour
{
    #region Serilized Fields
    [SerializeField] private string[] modelUrls = new string[]
    {
        "https://storage.googleapis.com/furniture-models/armchair/poltrona-biza_a3bc4a40-53e9-4be0-89f1-a7573d600971.glb",
        "https://storage.googleapis.com/furniture-models/dining-chair/victoria-ghost_8d4ea5cc-78a8-4edc-961d-17a3f6e83d4f.glb"
    };
    [SerializeField] private Transform placeHolder;
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
            gltfAsset.transform.position = RoomService.Instance.transform.position;
        }
        else
        {
            Debug.LogError("Failed to load the GLTF model from URL");
        }
    }
    #endregion

}
