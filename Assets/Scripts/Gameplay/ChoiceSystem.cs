using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
public class ChoiceSystem : MonoBehaviour
{
    private GameObject dialogBox;
    private bool isInteractable;
    private TMP_Dropdown choiceOptions;
    private Button confirmButton;
    public List<string> choices;
    private string correctAnswer;
    private bool menuGenerated = false;
    private string[] badChoices= {"a","b"};
    public GameObject typeofChoice;

    private void Start(){
        dialogBox = GameObject.Find("DialogBoxImage");
        choiceOptions = gameObject.GetComponentInChildren<TMP_Dropdown>();
        confirmButton = gameObject.GetComponentInChildren<Button>();
        confirmButton.onClick.AddListener(ConfirmChoice);
        gameObject.SetActive(false);
    }
    public void GetStart(){
        Start();
    }
    private void Update(){
        isInteractable = dialogBox.GetComponent<Button>().interactable;
        if(!isInteractable){
            if(!menuGenerated){
                Debug.Log("entre *suena musica de hacker*");
                ChoiceMenu();
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
    public void ChoiceMenu(/*TODO hacer que el choiceMenu reciba cualquier opcion a travez del texto*/){
        choiceOptions.ClearOptions();
        Debug.Log("xdxdx");
        //implementacion temporal, TODO hacer que el tipo de decision sea definida con el texto escrito.
        if(!typeofChoice.activeSelf){
            Debug.Log("holaxd");
            string[] badChoices = {"Descanso", "Comer"};
            for (int i = 0; i < badChoices.Length; i++){
                this.badChoices[i] = badChoices[i]; 
            }
            correctAnswer=dialogBox.GetComponent<DialogSystem>().clinicalCase.correctTreatment;
        }else{
            Debug.Log("holaxd2");
            string[] badChoices = {"Cancer Cerebral", "Sindrome del impostor"};
            for (int i = 0; i < badChoices.Length; i++){
            this.badChoices[i] = badChoices[i]; 
            }
            correctAnswer=dialogBox.GetComponent<DialogSystem>().clinicalCase.correctDiagnosis;
        }
        Debug.Log(correctAnswer);
        RandomChoices(correctAnswer);
        choiceOptions.AddOptions(choices);
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
        dialogBox.GetComponent<DialogSystem>().NextPhrase();
        gameObject.SetActive(false);
    }
}
