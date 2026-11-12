using UnityEngine;

public class Panel : MonoBehaviour
{
    Panel ParentPanel => transform.parent == null
        ? null
        : transform.parent.GetComponentInParent<Panel>();

    public void Open()
    {
        ParentPanel?.Open();
        gameObject.SetActive(true);
    }

    public void Close()
    {
        foreach (var child in GetComponentsInChildren<Panel>(true))
            if (child != this) child.gameObject.SetActive(false);

        gameObject.SetActive(false);
    }

    // Diagnóstico temporal: avisa cada vez que este panel se desactiva
    void OnDisable() => Debug.Log($"[Panel] Disabled {name}", this);
}