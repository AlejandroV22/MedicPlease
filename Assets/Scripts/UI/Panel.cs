using UnityEngine;

public class Panel : MonoBehaviour
{
    // Panel más cercano hacia arriba (excluye este objeto)
    public Panel Parent => transform.parent == null
        ? null
        : transform.parent.GetComponentInParent<Panel>();

    public bool IsOpen => gameObject.activeSelf;

    // Abre este panel y asegura que sus padres también estén abiertos
    public void Open()
    {
        Parent?.Open();
        gameObject.SetActive(true);
    }

    // Cierra este panel y sus submenús. El padre queda abierto (eso es "volver atrás")
    public void Close()
    {
        foreach (var sub in GetComponentsInChildren<Panel>(true))
            if (sub != this) sub.gameObject.SetActive(false);

        gameObject.SetActive(false);
    }
}