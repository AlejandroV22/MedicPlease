using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ClinicalHistoryButton : MonoBehaviour
{
    public GameObject clinicalHistory;
    public Button exitCaseButton;

    public GameObject dialogBox;
    public GameObject choiceBox;

    public bool isActive = false;

    private void Start()
    {
        dialogBox = GameObject.Find("DialogBoxImage");
        choiceBox = GameObject.Find("ChoiceBox");
    }

    public void ActiveChange()
    {
        bool shouldOpen = !clinicalHistory.activeSelf;

        clinicalHistory.SetActive(shouldOpen);
        dialogBox.SetActive(!shouldOpen);

        if (!dialogBox.GetComponent<Button>().interactable)
        {
            choiceBox.SetActive(!shouldOpen);
        }

        exitCaseButton.interactable = !shouldOpen;
        Debug.Log(dialogBox);
        Debug.Log(choiceBox);
    }
}