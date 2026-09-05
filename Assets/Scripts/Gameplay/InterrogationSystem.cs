using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine.UI;
using UnityEngine;
using Unity.AppUI.UI;

public class InterrogationSystem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject dialogBox;
    public GameObject choiceBox;
    private string rawChoices;
    private bool interrogationTime;
    private int interrogationTries=1;

    void Start()
    {
        //dialogBox = GameObject.Find("DialogBoxImage");
        //choiceBox = GameObject.Find("ChoiceBox");
    }
    public void InterrogationChoice()
        {
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
            
        }
    public void InterrogationLoop()
    {
        if(interrogationTime == true && interrogationTries <=3){
                interrogationTries++;
                InterrogationChoice();
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
