using System.Diagnostics.Contracts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClinicalHistoryButton : MonoBehaviour
{
    public GameObject clinicalHistory;
    public GameObject generalPanel;
    private GameObject dialogBox;
    private GameObject choiceBox;
    public bool isActive = false;
    private void Start()
    {
        dialogBox = GameObject.Find("DialogBoxImage");
        choiceBox = GameObject.Find("ChoiceBox");    
    }
    public void ActiveChange(){
        if(isActive){
            clinicalHistory.SetActive(!isActive);
            dialogBox.SetActive(isActive);
            if (!dialogBox.GetComponent<Button>().interactable){
                choiceBox.SetActive(isActive);
            }
            isActive=false;
        }else if(!isActive){
            clinicalHistory.SetActive(!isActive);
            dialogBox.SetActive(isActive);
            if (!dialogBox.GetComponent<Button>().interactable){
                choiceBox.SetActive(isActive);
            }
            isActive=true;
        }
    }
}
