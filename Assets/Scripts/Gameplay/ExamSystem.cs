using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using System;
public class ExamSystem : MonoBehaviour
{
    public GameObject choiceBox;
    private TMP_Dropdown choiceOptions;
    public string[] exams;
    public GameObject dialogSystem;

    public bool examTime;

    public GameObject actionMenu;
    
    public void ShowExam(){
        examTime = true;
        string rawChoices = "{choice}{choicest2 c:";
            foreach(string exam in exams)
            {
                rawChoices = @rawChoices + "\"" + exam + "\"" + " "; 
            }
            rawChoices = rawChoices + "} ";
            
            choiceBox.SetActive(true);
            choiceBox.GetComponent<ChoiceSystem>().ChoiceMenu(rawChoices);
                       
    }
    public void ExamLoop()
    {
        if(examTime == true)
        {
            Debug.Log("examTimeValue " + examTime);
            actionMenu.SetActive(true);
        }
    }
    public void ExamSelected(){
        choiceBox.GetComponent<ChoiceSystem>().GetStart();
        //TODO agregar a clinical case los resultados de los examenes.
    }
}
