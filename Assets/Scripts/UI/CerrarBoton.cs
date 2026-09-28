using UnityEngine;
using UnityEngine.UI; // Necesario si quieres asignar el evento por código

public class CerrarBoton : MonoBehaviour
{
    [Header("Opcional")]
    [Tooltip("Si lo dejas vacío, desactivará automáticamente al objeto padre de este botón.")]
    public GameObject objetoACerrar;

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
            objetoACerrar.SetActive(false);
        }
        else
        {
            // Como caso límite (si el botón no tuviera padre), se desactiva a sí mismo
            gameObject.SetActive(false);
        }
    }
}