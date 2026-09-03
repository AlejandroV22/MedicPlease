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
    public Queue <string> queueDialogs = new();
    public Texto texto;
    // private Sprite expretion;
    [SerializeField] TextMeshProUGUI screenText;
    //[SerializeField] Image screenImage;
    private string actualPhrase;
    private bool isWriting = false;
    private string isWritingPhrase;
    public ClinicalCase clinicalCase;
    private string[] textVariables;
    public int correctAnswer=0;
    public GameObject typeofChoice;
    public GameObject choiceBox;
    public string rawChoices;
    private Coroutine blinkingSprite;

    public void Awake(){
        GetVariables();
        texto.GetTextToFile(clinicalCase.scriptPath);
        ShowText();
        GetComponent<Button>().onClick.AddListener(IsPressed);

    }
    public void GetVariables(){
        clinicalCase = GameManager.Instance.SelectedCase;
        transform.GetChild(1).GetComponent<Image>().sprite = clinicalCase.patientPortrait;
        Type tipo = clinicalCase.GetType();
        FieldInfo[] campos = tipo.GetFields();
        List<string> lista = new List<string> {};
        foreach(FieldInfo campo in campos){
            lista.Add(campo.Name);
        }
        textVariables = lista.ToArray();
        
    }
    public void ShowText(){
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
      //  CheckExpretion(actualPhrase);
            //actualPhrase = actualPhrase[1..^0];
            isWriting=true;
        //screenImage.sprite = expretion;
            StartCoroutine(ShowCharts(actualPhrase));
                
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
    IEnumerator ShowCharts(string textToShow){
        screenText.text = "";
        Debug.Log("corutina iniciada");
        isWritingPhrase=textToShow;
        foreach (char character in textToShow.ToCharArray()){
            Debug.Log("corutina iniciada");
            screenText.text+=character;
            yield return new WaitForSeconds(0.02f);
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
        if (isWriting==true){
            Debug.Log("skipeado");
            StopAllCoroutines();
            screenText.text = isWritingPhrase;
            isWriting=false;
            blinkingSprite = StartCoroutine(SpriteBlinking());
        }else{
            NextPhrase();
        }
                    
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
                    var regex = new Regex(@"\{m\}");
                    if (regex.Match(dialog).Success)
                    {
                        actualPhrase = regex.Replace(dialog, "");
                    }
                    else
                    {
                        actualPhrase=dialog;   
                    }
                    SkipPhrase(timesToSkip);
                    break;
                }
            }
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
// TODO que el texto pueda salir a diferentes velocidades
}
