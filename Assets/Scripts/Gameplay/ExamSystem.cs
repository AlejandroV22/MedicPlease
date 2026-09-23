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
    private int examinationTries;
    private bool examTime;

    public GameObject actionMenu;
    
    public void ShowExam(){
        if(examinationTries <=6){
        examTime = true;
        string rawChoices = "{choice}{choicest2 c:";
            foreach(string exam in exams)
            {
                rawChoices = @rawChoices + "\"" + exam + "\"" + " "; 
            }
            rawChoices = rawChoices + "} ";
            
            choiceBox.SetActive(true);
            choiceBox.GetComponent<ChoiceSystem>().ChoiceMenu(rawChoices);
        }else{
                examTime = false;
                dialogSystem.GetComponent<DialogSystem>().AddPhrase("(No puedo realizarle mas examenes al paciente)");
                //chapuza historica parte 3
                dialogSystem.GetComponent<DialogSystem>().AddPhrase("Fin del prototipo 1");
        }
    }
    public void ExamLoop()
    {
        if(examTime == true && examinationTries <=6)
        {
            examinationTries++;
            Debug.Log("examTimeValue " + examTime);
            actionMenu.SetActive(true);
        }else
        {
            examTime=false;
        }
    }
    public void ExamSelected(){
        choiceBox.GetComponent<ChoiceSystem>().GetStart();
        //TODO agregar a clinical case los resultados de los examenes.
    }
}
