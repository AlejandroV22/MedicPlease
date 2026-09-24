using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using System.Text.RegularExpressions;
//bruh
public class ChoiceSystem : MonoBehaviour
{
    private GameObject dialogBox;
    private bool isInteractable;
    private TMP_Dropdown choiceOptions;
    private Button confirmButton;
    public List<string> choices;
    public string rawChoices;
    private string correctAnswer;
    private bool menuGenerated = false;
    private List<string> badChoices;
    public GameObject typeofChoice;
    int searchTimes = 0;

    private void Start(){
        dialogBox = GameObject.Find("DialogBoxImage");
        choiceOptions = gameObject.GetComponentInChildren<TMP_Dropdown>();
        confirmButton = gameObject.GetComponentInChildren<Button>();
        gameObject.SetActive(false);
    }
    public void GetStart(){
        Start();
    }
    private void Update(){
        rawChoices = dialogBox.GetComponent<DialogSystem>().rawChoices;
        isInteractable = dialogBox.GetComponent<Button>().interactable;
        if(!isInteractable){
            if(!menuGenerated){
                Debug.Log("entre *suena musica de hacker*");
                ChoiceMenu(rawChoices);
                menuGenerated = true;
            }
            }else{
                menuGenerated = false;
        }
    }
    private void OnDisable()
    {
        menuGenerated = false;
    }
    public void ChoiceMenu(string rawChoices){
        if(choiceOptions != null)
        {
         choiceOptions.ClearOptions();   
        }
        Debug.Log("xdxdx");
        Debug.Log("holaxd");
        // TODO hacer que vayan a distintos caminos dependiendo de la decision
            if (rawChoices.Contains("choicest1")){
                confirmButton.onClick.RemoveAllListeners();
                confirmButton.onClick.AddListener(ConfirmChoice);
                var regexBadChoices = new Regex(@"\{choice\}\{choicest1 b:(.*?) c:");
                var matchBadChoices = regexBadChoices.Match(rawChoices);
                List<string> badChoices = Regex.Matches(matchBadChoices.Groups[1].Value, @"""([^""]+)""")
                            .Select(m => m.Groups[1].Value)
                            .ToList();
                this.badChoices = badChoices;
                var regexCorrectAnswer = new Regex(@"\{choice\}\{choicest1 b:.*? c:""([^""]+)""\}");
                var matchCorrectAnswer = regexCorrectAnswer.Match(rawChoices);
                correctAnswer=matchCorrectAnswer.Groups[1].Value;
                Debug.Log("momazos entrar al if");
                RandomChoices(correctAnswer);
                choiceOptions.AddOptions(choices);
                foreach (string choice in choices)
                {
                    Debug.Log(choice);
                }
            } if (rawChoices.Contains("choicest2"))
            {
                confirmButton.onClick.RemoveAllListeners();
                confirmButton.onClick.AddListener(changePath);
                var regexChoices = new Regex(@"\{choice\}\{choicest2\s+c:\s*(.*?)\}");
                var matchChoices = regexChoices.Match(rawChoices);
                choices = Regex.Matches(matchChoices.Groups[1].Value, @"""([^""]*)""")
                            .Select(m => m.Groups[1].Value)
                            .ToList();
                choiceOptions.AddOptions(choices);
                foreach (string choice in choices)
                {
                    Debug.Log(choice);
                }
            }
            dialogBox.GetComponent<Button>().interactable = false;
            menuGenerated = true;
    }
    public void RandomChoices(string rchoise){
        choices.Clear();
        choices.Add(rchoise);
        foreach(string choice in badChoices){
            choices.Add(choice);
        }
        choices = choices.OrderBy(x => UnityEngine.Random.value).ToList();

    }
    public void ConfirmChoice(){
        int selectIndex = choiceOptions.value;
        if (choiceOptions.options[selectIndex].text == correctAnswer){
           dialogBox.GetComponent<DialogSystem>().correctAnswer += 1;
           Debug.Log("respuesta correcta"); 
        }
        dialogBox.GetComponent<Button>().interactable = true;
        gameObject.SetActive(false);
        dialogBox.GetComponent<DialogSystem>().NextPhrase();
    }
    public void changePath()
    {
        searchTimes++;//TODO agregar un identificador de caminos y un identificador de donde termina el texto
        Texto fullText= dialogBox.GetComponent<DialogSystem>().texto;
        Queue <string> queueDialogs = dialogBox.GetComponent<DialogSystem>().queueDialogs;
        int selectIndex = choiceOptions.value;
        string selectedOption = choiceOptions.options[selectIndex].text;
        int timesToSkip = 0;
        bool dialogFound = false;
        foreach(string dialog in queueDialogs)
        {
            timesToSkip++;
            if(dialog.Contains("{"+selectedOption +"}"))
            {
                dialogBox.GetComponent<DialogSystem>().SkipPhrase(timesToSkip);
                var regex = new Regex(@"\{[^}]+\}");
                if (regex.Match(dialog).Success)
                {
                    dialogFound = true;
                    dialogBox.GetComponent<DialogSystem>().AddPhrase(regex.Replace(dialog, ""));
                    dialogBox.GetComponent<Button>().interactable = true;
                    gameObject.SetActive(false);
                    dialogBox.GetComponent<DialogSystem>().NextPhrase();
                }
                break;
            }
        }
        if(dialogFound == false)
        {
            foreach (string textBuffer in fullText.textArray){
                queueDialogs.Enqueue(textBuffer);
            }
            dialogBox.GetComponent<DialogSystem>().queueDialogs= queueDialogs;
            if(searchTimes < 100){
                changePath();
            }
            dialogBox.GetComponent<Button>().interactable = true;
            gameObject.SetActive(false);
            dialogBox.GetComponent<DialogSystem>().NextPhrase();
        }


        Debug.Log("eso boton");
    }
}
