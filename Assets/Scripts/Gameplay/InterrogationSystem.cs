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
    private int interrogationTries=1;
    public GameObject actionMenu;

    void Start()
    {
        //dialogBox = GameObject.Find("DialogBoxImage");
        //choiceBox = GameObject.Find("ChoiceBox");
    }
    public bool getInterrogationTime()
    {
        return interrogationTime;
    }
    public void InterrogationChoice()
        {
            if(interrogationTries <=3){
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
            dialogBox.GetComponent<DialogSystem>().AddPhrase("(¿Qué deberia preguntarle al paciente?)");
            }else{
                interrogationTime = false;
                dialogBox.GetComponent<DialogSystem>().AddPhrase("(El paciente no respondera mas preguntas)");
                dialogBox.GetComponent<DialogSystem>().AddPhrase("{main}");
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
