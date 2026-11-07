using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ClinicalHistoryButton : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject clinicalHistory;
    public Button exitCaseButton;

    public GameObject dialogBox;
    public GameObject choiceBox;
    public bool isActiveFirstTime;
    public bool isActive = false;


    public void ActiveChange()
    {
        if (clinicalHistory == null || dialogBox == null)
        {
            Debug.LogError("Faltan referencias por asignar en el Inspector de " + gameObject.name);
            return;
        }
        ActiveFirstTime();
        bool shouldOpen = !IsVisible(clinicalHistory);

        SetVisible(clinicalHistory, shouldOpen);
        SetVisible(dialogBox, !shouldOpen);

        if (choiceBox != null && dialogBox.TryGetComponent<Button>(out Button dialogBtn))
        {
            if (!dialogBtn.interactable)
            {
                SetVisible(choiceBox, !shouldOpen);
            }
        }

        if (exitCaseButton != null)
        {
            exitCaseButton.interactable = !shouldOpen;
        }
    }

    public void ActiveFirstTime()
    {
        if (isActiveFirstTime == false)
        {
            isActiveFirstTime = true;
        }
    }
    private void SetVisible(GameObject go, bool visible)
    {
        if (go == null) return;

        go.SetActive(true);

        CanvasGroup cg = go.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            cg = go.AddComponent<CanvasGroup>();
        }

        cg.alpha = visible ? 1f : 0f;
        cg.interactable = visible;
        cg.blocksRaycasts = visible;
    }

    private bool IsVisible(GameObject go)
    {
        if (go == null) return false;

        CanvasGroup cg = go.GetComponent<CanvasGroup>();
        return cg == null ? go.activeSelf : cg.alpha > 0f;
    }
}