using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    private Queue <string> queueDialogs = new();
    public Texto texto;
    private Sprite expretion;
    [SerializeField] TextMeshProUGUI screenText;
    [SerializeField] Image screenImage;
    public void Awake(){
        ShowText();
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
        string actualPhrase = queueDialogs.Dequeue();
        CheckExpretion(actualPhrase);
        actualPhrase = actualPhrase[1..^0];
        screenText.text = actualPhrase;
        screenImage.sprite = expretion;
        StartCoroutine(ShowCharts(actualPhrase));
    }
    public void CheckExpretion(string actualPhrase){
            //Todo agregar comprobacion de que el primer caracter sea numerico
            Debug.Log(actualPhrase.ToCharArray()[0]);
            int expretionIndex = int.Parse(actualPhrase[0].ToString());
            expretion = texto.imageArray[expretionIndex];
        
    }
    IEnumerator ShowCharts(string textToShow){
        screenText.text = "";
        foreach (char character in textToShow.ToCharArray()){
            screenText.text+=character;
            yield return new WaitForSeconds(0.02f);
        }

    }

}
