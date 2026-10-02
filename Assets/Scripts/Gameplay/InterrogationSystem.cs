using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine.UI;
using UnityEngine;
using Unity.AppUI.UI;
using System;

public class InterrogationSystem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject dialogBox;
    public GameObject choiceBox;
    private string rawChoices;
    private bool interrogationTime;
    private bool interrogationOver= false;
    private int interrogationTries=1;
    public GameObject actionMenu;
    public GameObject ClinicalCaseButton;
    void Start()
    {
        //dialogBox = GameObject.Find("DialogBoxImage");
        //choiceBox = GameObject.Find("ChoiceBox");
    }
    public bool GetInterrogationTime()
    {
        return interrogationTime;
    }
    public bool GetInterrogationOver(){
        return interrogationOver;
    }
    public void InterrogationChoice()
        {
            if(interrogationTries <=3){
                if(ClinicalCaseButton.GetComponent<ClinicalHistoryButton>().isActiveFirstTime == false){
                    dialogBox.GetComponent<DialogSystem>().AddPhrase("(Debería revisar primero la historia del paciente)");
                    dialogBox.GetComponent<DialogSystem>().AddPhrase("{a}¿Qué debería hacer?");
                }else{
                interrogationTime = true;
                string[] interrogationQuestions=dialogBox.GetComponent<DialogSystem>().clinicalCase.questions;
                rawChoices = "{choice}{choicest2 c:";
                foreach(string question in interrogationQuestions)
                {
                    rawChoices = @rawChoices + "\"" + question + "\"" + " "; 
                }
                rawChoices = rawChoices + "} ";
            
                choiceBox.SetActive(true);
                choiceBox.GetComponent<ChoiceSystem>().ChoiceMenu(rawChoices);
                dialogBox.GetComponent<DialogSystem>().AddPhrase("(¿Qué debería preguntarle al paciente?)");
                }
            }else{
                interrogationOver = true;
                interrogationTime = false;
                dialogBox.GetComponent<DialogSystem>().AddPhrase("(El paciente no respondera mas preguntas)");
                //chapuza historica parte 2
                dialogBox.GetComponent<DialogSystem>().AddPhrase("{a}¿Qué debería hacer?");
            }
            
        }
    public void InterrogationLoop()
    {
        if(interrogationTime == true && interrogationTries <=3){
                interrogationTries++;
                InterrogationChoice();
                actionMenu.SetActive(false);
        }
        else
        {
            interrogationTime=false;
            
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
