using UnityEngine;
using UnityEngine.EventSystems;

public class BlockClick : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        // No hace nada, solo evita que el click suba al botón del panel del menu de inicio xd
    }
}