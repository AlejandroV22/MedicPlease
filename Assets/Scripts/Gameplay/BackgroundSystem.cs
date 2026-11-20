using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public class BackgroundSystem : MonoBehaviour
{
    public Texture bg;
    public Texture2D ch;
    public GameObject canva;
    public GameObject charaImage;
    public ClinicalCase clinicalCase;
    private Sprite defaultCh;

    private void Start()
    {
        clinicalCase = GameManager.Instance.SelectedCase;
        defaultCh = clinicalCase.retratoPaciente;  
    }

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
    public void ChangeCh(string chName, bool chVisibility = true)
    {
        charaImage.SetActive(chVisibility);

        Image img = charaImage.GetComponent<Image>();
        img.preserveAspect = true; // Mantiene la proporción del sprite dentro del recuadro

        if (chName == "default")
        {
            img.sprite = defaultCh;
            return;
        }

        var handle = Addressables.LoadAssetAsync<Texture2D>("Assets/Art/" + chName + ".png");
        ch = handle.WaitForCompletion();

        if (handle.Status != AsyncOperationStatus.Succeeded || ch == null)
        {
            Debug.LogError($"No se cargó: {chName}");
            return;
        }

        img.sprite = ConvertTextureToSprite(ch);
    }
    public Sprite ConvertTextureToSprite(Texture2D texture)
    {
        Rect rect = new Rect(0, 0, texture.width, texture.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);

        return Sprite.Create(texture, rect, pivot);
    }
}