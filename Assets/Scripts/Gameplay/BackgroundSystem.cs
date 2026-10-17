using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Unity.AppUI.UI;

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
        if(chVisibility == false)
        {
            charaImage.SetActive(false);
        }else if (chVisibility == true)
        {
            charaImage.SetActive(true);
        }
    if(chName == "default"){
       charaImage.GetComponent<Image>().sprite = defaultCh;
    }else{
        var handle = Addressables.LoadAssetAsync<Texture2D>("Assets/Art/" + chName + ".png");
        ch = handle.WaitForCompletion();

        if (handle.Status != AsyncOperationStatus.Succeeded || ch == null)
        {
            Debug.LogError($"No se cargó: {chName}");
            return;
        }
        Sprite chSprite = ConvertTextureToSprite(ch);
        charaImage.GetComponent<Image>().sprite = chSprite;
    }
    }
    public Sprite ConvertTextureToSprite(Texture2D texture)
    {
        Rect rect = new Rect(0, 0, texture.width, texture.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f);

        return Sprite.Create(texture, rect, pivot);
    }
}