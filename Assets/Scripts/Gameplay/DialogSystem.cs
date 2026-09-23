using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using System.Text.RegularExpressions;

public class DialogSystem : MonoBehaviour
{
    [NonSerialized] public Queue <string> queueDialogs = new();
    public Texto texto;
    // private Sprite expretion;
    [SerializeField] TextMeshProUGUI screenText;
    [SerializeField] TextMeshProUGUI charTalkNameText;
    //[SerializeField] Image screenImage;
    private string actualPhrase;
    private bool isWriting = false;
    private string isWritingPhrase;
    public ClinicalCase clinicalCase;
    private string[] textVariables;
    public int correctAnswer=0;
    public GameObject typeofChoice;
    public GameObject choiceBox;
    public GameObject interrogationBox;
    public string rawChoices;
    public GameData saveFile = new GameData();
    public GameDataController saveFileController;
    private Coroutine blinkingSprite;
    public GameObject inspectionBox;
    public GameObject listeningBox;
    public GameObject touchingBox;
    public GameObject actionMenu;
    public GameObject bgController;
    public GameObject audioController;
    public float texSpeed = 0.05f;
    public GameObject clinicalHistoryButton;

    public void Awake(){
        GetVariables();
        texto.GetTextToFile(clinicalCase.scriptPath);
        ShowText();
        GetComponent<Button>().onClick.AddListener(IsPressed);

    }
    public void GetVariables(){
        saveFile = saveFileController.LoadData(saveFile);
        clinicalCase = GameManager.Instance.SelectedCase;
        transform.GetChild(1).GetComponent<Image>().sprite = clinicalCase.retratoPaciente;
        Type tipo = clinicalCase.GetType();
        FieldInfo[] campos = tipo.GetFields();
        List<string> lista = new List<string> {};
        foreach(FieldInfo campo in campos){
            lista.Add(campo.Name);
        }
        textVariables = lista.ToArray();
        
    }
    public void ShowText(){
        if(charTalkNameText != null){
            charTalkNameText.text = "??????";
        }
        queueDialogs.Clear();
        foreach (string textBuffer in texto.textArray){
            queueDialogs.Enqueue(textBuffer);
        }
        NextPhrase();
    }
    public void NextPhrase(){
        if (queueDialogs.Count == 0){
            return;
        }
        Debug.Log(textVariables);
        if (isWriting==false){
            if (blinkingSprite != null)
            {
              StopCoroutine(blinkingSprite);  
            } 
            actualPhrase = queueDialogs.Dequeue();
            CheckVariable(actualPhrase);
            CheckTalker();
            if (isWriting){
                return;
            }
      //  CheckExpretion(actualPhrase);
            //actualPhrase = actualPhrase[1..^0];
            isWriting=true;
        //screenImage.sprite = expretion;
            StartCoroutine(ShowCharts(actualPhrase, texSpeed));
                
        }
    }
    public void AddPhrase(string newPhrase){
        Queue <string> modDialogs = new();
        modDialogs.Enqueue(newPhrase);
        foreach (string textBuffer in queueDialogs){
            modDialogs.Enqueue(textBuffer);
        }
        queueDialogs.Clear();
        queueDialogs = modDialogs;
        NextPhrase();
    }
    /*public void CheckExpretion(string actualPhrase){
            //Todo agregar comprobacion de que el primer caracter sea numerico
            Debug.Log(actualPhrase.ToCharArray()[0]);
            int expretionIndex = int.Parse(actualPhrase[0].ToString());
            expretion = texto.imageArray[expretionIndex];
        isWriting=true;
    }*/
    IEnumerator ShowCharts(string textToShow, float textSpeed){
        screenText.text = "";
        Debug.Log("corutina iniciada");
        isWritingPhrase=textToShow;
         int characterCount = 0;
        foreach (char character in textToShow.ToCharArray()){
            Debug.Log("corutina iniciada");
            screenText.text+=character;
            if (character != ' ' && characterCount % 2 == 0)
            {
                audioController.GetComponent<AudioSystem>().PlayCharSound();
            }

            characterCount++;
            yield return new WaitForSeconds(textSpeed);
        }
        isWriting=false;
        blinkingSprite = StartCoroutine(SpriteBlinking());

    }
    IEnumerator SpriteBlinking()
    {
        while(gameObject.GetComponent<Button>().interactable){
        string baseText = screenText.text;
        string spriteName = "<sprite=0>"; 
        screenText.text += spriteName;
        yield return new WaitForSeconds(0.5f);
        screenText.text = baseText;
        yield return new WaitForSeconds(0.5f);
    }
    }
    public void IsPressed(){
        /*if (isWriting==true){
            Debug.Log("skipeado");
            StopAllCoroutines();
            screenText.text = isWritingPhrase;
            isWriting=false;
            blinkingSprite = StartCoroutine(SpriteBlinking());
        }else{*/
            NextPhrase();
        //}
                    
    }
    public void CheckVariable(string actualPhrase){
        GetVariables();
        foreach (string variable in textVariables){
            if(actualPhrase.Contains("{"+variable+"}")){
                FieldInfo field = typeof(ClinicalCase).GetField(variable);
                if (field != null){
                    object value = field.GetValue(clinicalCase);
                    actualPhrase = actualPhrase.Replace("{"+variable+"}", value?.ToString() ?? "");
                    //TODO feedback pero ahora si feedback
                    if(variable=="feedback"){
                        if(correctAnswer==2){
                            actualPhrase = actualPhrase.Replace("{"+variable+"}", value?.ToString() ?? "");
                        }else{
                            actualPhrase = "El paciente murio";
                        }
                    }
                }
        }
        }
        if (actualPhrase.Contains("{dname}")){
            actualPhrase = actualPhrase.Replace("{dname}",saveFile.playerName);
        }
        if(actualPhrase.Contains("{choice}")){
            rawChoices = actualPhrase;
            var regext1 = new Regex(@"\{choice\}\{choicest1\s+b:\s*((?:""[^""]*""\s*)*)c:\s*""[^""]*""\}");
            Debug.Log("match estado" + regext1.Match(actualPhrase).Success);
            if (regext1.Match(actualPhrase).Success)
            {
             actualPhrase = regext1.Replace(actualPhrase, "");   
            }
            var regext2 = new Regex(@"\{choice\}\{choicest2\s+c:\s*((?:""[^""]*""\s*)*)\}");
            if (regext2.Match(actualPhrase).Success)
            {
             actualPhrase = regext2.Replace(actualPhrase, "");   
            }
            typeofChoice.SetActive(true);
            gameObject.GetComponent<Button>().interactable = false;
            choiceBox.SetActive(true);
        }
        if (actualPhrase.Contains("{main}"))
        {
            int timesToSkip = 0;
            foreach(string dialog in queueDialogs)
            {
                timesToSkip++;
                if(dialog.Contains("{m}"))
                {
                    //TODO refactorizar todo el sistema de examenes para integrar los limites de examenes
                    inspectionBox.GetComponent<ExamSystem>().ExamLoop();
                    touchingBox.GetComponent<ExamSystem>().ExamLoop();
                    listeningBox.GetComponent<ExamSystem>().ExamLoop();
                    var regex = new Regex(@"\{m\}");
                    if (regex.Match(dialog).Success)
                    {
                        actualPhrase = regex.Replace(dialog, "");
                    }
                    else
                    {
       
                         actualPhrase=dialog;   
                   
                    }
                    interrogationBox.GetComponent<InterrogationSystem>().InterrogationLoop();
                    SkipPhrase(timesToSkip);
                    break;
                }
            }
        }
        //chapuza historica
        if(actualPhrase.Contains("{m}")){
            actualPhrase = actualPhrase.Replace("{m}","");
        }
        if (actualPhrase.Contains("{a}"))
        {
            if(interrogationBox.GetComponent<InterrogationSystem>().GetInterrogationTime() == false)
            {
             actionMenu.SetActive(true);   
            }
            clinicalHistoryButton.SetActive(true);
           actualPhrase = actualPhrase.Replace("{a}","");
        }
        if (actualPhrase.Contains("{bg:"))
        {
            var regex = new Regex(@"\{bg:\s*""([^""]*)""\s*\}");
            var match = regex.Match(actualPhrase);
            string bgValue = String.Empty;
            if (match.Success)
            {
                bgValue = match.Groups[1].Value; 
                actualPhrase = regex.Replace(actualPhrase, ""); 
            }
            Debug.Log("bgValue " + bgValue);
            bgController.GetComponent<BackgroundSystem>().ChangeBg(bgValue);
        }
        if (actualPhrase.Contains("{ch:"))
        {
            var regex = new Regex(@"\{ch:\s*""([^""]*)""\s*\}");
            var match = regex.Match(actualPhrase);
            string chValue = String.Empty;
            if (match.Success)
            {
                chValue = match.Groups[1].Value; 
                actualPhrase = regex.Replace(actualPhrase, ""); 
            }
            Debug.Log("chValue " + chValue);
            bgController.GetComponent<BackgroundSystem>().ChangeCh(chValue);
        }
        
        
        this.actualPhrase=actualPhrase;
    }
    public void SkipPhrase(int timesToSkip = 1)
    {
        for (int i=1; i<=timesToSkip; i++)
        {
            queueDialogs.Dequeue();
        }
    }
    public void CheckTalker(){
        if (actualPhrase !=null){
            var regextNdname = new Regex(@"\{Ndname\}");
            Debug.Log("match estado Ndname: " + regextNdname.Match(actualPhrase).Success);
            if (regextNdname.Match(actualPhrase).Success)
            {
                charTalkNameText.text = saveFile.playerName;
                actualPhrase = regextNdname.Replace(actualPhrase, "");
            }

            var regextNpname = new Regex(@"\{Npname\}");
            Debug.Log("match estado Npname: " + regextNpname.Match(actualPhrase).Success);
            if (regextNpname.Match(actualPhrase).Success)
            {
                GetVariables();
                FieldInfo field = typeof(ClinicalCase).GetField("nombrePaciente");
                if (field != null)
                {
                    object value = field.GetValue(clinicalCase);
                    if (charTalkNameText != null)
                    {
                        Debug.Log("cambio de nombre de paciente");
                        charTalkNameText.text = value.ToString();
                    }
                }
                actualPhrase = regextNpname.Replace(actualPhrase, "");
            }
                }
        
    }
// TODO que el texto pueda salir a diferentes velocidades
}
