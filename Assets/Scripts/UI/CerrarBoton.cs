using UnityEngine;
using UnityEngine.UI; // Necesario si quieres asignar el evento por código

public class CerrarBoton : MonoBehaviour
{
    [Header("Opcional")]
    [Tooltip("Si lo dejas vacío, desactivará automáticamente al objeto padre de este botón.")]
    public GameObject objetoACerrar;
    public GameObject dialogSystem;
    public GameObject interrogationSystem;
    public GameObject examSystem;

    private void Start()
    {
        if (objetoACerrar == null && transform.parent != null)
        {
            objetoACerrar = transform.parent.gameObject;
        }
    }

    /// <summary>
    /// Método público para vincular al evento OnClick() del Button en el Inspector.
    /// </summary>
    public void CerrarMenu()
    {
        if (objetoACerrar != null)
        {
            if(interrogationSystem != null){
                if(interrogationSystem.GetComponent<InterrogationSystem>().GetInterrogationTime() == true){
                    interrogationSystem.GetComponent<InterrogationSystem>().SetInterrogationTime(false);
                }
            }
            objetoACerrar.SetActive(false);
            dialogSystem?.GetComponent<DialogSystem>().AddPhrase("{a}(¿Qué debería hacer?)");
            dialogSystem.GetComponent<Button>().interactable = true;
            
            
        }
        else
        {
            // Como caso límite (si el botón no tuviera padre), se desactiva a sí mismo
            if(interrogationSystem != null){
                if(interrogationSystem.GetComponent<InterrogationSystem>().GetInterrogationTime() == true){
                    interrogationSystem.GetComponent<InterrogationSystem>().SetInterrogationTime(false);
                }
            }
            gameObject.SetActive(false);
            dialogSystem?.GetComponent<DialogSystem>().AddPhrase("{a}(¿Qué debería hacer?)");
            dialogSystem.GetComponent<Button>().interactable = true;
        }
    }
}