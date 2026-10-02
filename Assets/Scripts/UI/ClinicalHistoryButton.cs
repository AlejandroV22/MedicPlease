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
        bool shouldOpen = !clinicalHistory.activeSelf;

        clinicalHistory.SetActive(shouldOpen);
        dialogBox.SetActive(!shouldOpen);

        if (choiceBox != null && dialogBox.TryGetComponent<Button>(out Button dialogBtn))
        {
            if (!dialogBtn.interactable)
            {
                choiceBox.SetActive(!shouldOpen);
            }
        }

        if (exitCaseButton != null)
        {
            exitCaseButton.interactable = !shouldOpen;
        }
    }
    public void ActiveFirstTime(){
        if(isActiveFirstTime == false){
            isActiveFirstTime = true;
        }
    }
}