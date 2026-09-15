using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BackgroundSystem : MonoBehaviour
{
    public Texture bg;
    public GameObject canva;

    public void ChangeBg(string bgName)
    {
        var handle = Addressables.LoadAssetAsync<Texture2D>("Assets/Art/" + bgName + ".png");
        bg = handle.WaitForCompletion();

        if (handle.Status != AsyncOperationStatus.Succeeded || bg == null)
        {
            Debug.LogError($"No se cargó: {bgName}");
            return;
        }

        canva.GetComponent<RawImage>().texture = bg;
    }
}