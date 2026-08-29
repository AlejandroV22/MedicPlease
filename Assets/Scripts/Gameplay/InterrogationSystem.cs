using System.Runtime.CompilerServices;
using Unity.AppUI.UI;
using UnityEngine;

public class InterrogationSystem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private GameObject dialogBox;
    private GameObject choiceBox;
    private string rawChoices;
    void Start()
    {
        dialogBox = GameObject.Find("DialogBoxImage");
        choiceBox = GameObject.Find("ChoiceBox");
    }
    public void InterrogationChoice()
        {
            string[] interrogationQuestions=dialogBox.GetComponent<DialogSystem>().clinicalCase.questions;
            rawChoices = "{choice}{choicest2 c:";
            foreach(string question in interrogationQuestions)
            {
                rawChoices = @rawChoices + "\"" + question + "\"" + " "; 
            }
            rawChoices = rawChoices + "} ";
            choiceBox.SetActive(true);
            choiceBox.GetComponent<ChoiceSystem>().ChoiceMenu(rawChoices);
        }
    public void InterrogationLoop()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
