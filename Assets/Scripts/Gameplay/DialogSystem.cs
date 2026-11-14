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
    [NonSerialized] public Queue<string> queueDialogs = new();
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
    public int correctAnswer = 0; // ya no se usa para la logica de diagnostico, esta solo porque si no se rompe choice system
    public GameObject choiceBox;
    public GameObject interrogationButton;
    public string rawChoices;
    public GameData saveFile = new GameData();
    public GameDataController saveFileController;
    private Coroutine blinkingSprite;
    public GameObject inspectionButton;
    public GameObject listeningButton;
    public GameObject touchingButton;
    public GameObject extraExamButton;
    public Panel actionMenu;
    public GameObject bgController;
    public GameObject audioController;
    public float textSpeed = 0.05f;
    public GameObject clinicalHistoryButton;
    public Panel extraMenu;
    public GameObject diagnosisController;
    public FinalFeedbackSystem finalFeedback; 

    public void Awake()
    {
        Debug.Assert(actionMenu != null, "actionMenu no asignado en DialogSystem", this);
        Debug.Assert(extraMenu != null, "extraMenu no asignado en DialogSystem", this);
        FinalFeedbackSystem.ResetState();
        GetVariables();
        saveFileController.StartAttempt(clinicalCase.name);
        texto.GetTextToFile(clinicalCase.scriptPath);
        ShowText();
        GetComponent<Button>().onClick.AddListener(IsPressed);
        choiceBox.transform.GetChild(1).gameObject.SetActive(true);

    }
    public bool GetIsWriting(){
        return isWriting;
    }
    public void GetVariables()
    {
        saveFile = saveFileController.Data;
        clinicalCase = GameManager.Instance.SelectedCase;
        transform.GetChild(1).GetComponent<Image>().sprite = clinicalCase.retratoPaciente;
        Type tipo = clinicalCase.GetType();
        FieldInfo[] campos = tipo.GetFields();
        List<string> lista = new List<string> { };
        foreach (FieldInfo campo in campos)
        {
            lista.Add(campo.Name);
        }
        textVariables = lista.ToArray();

    }
    public void ShowText()
    {
        if (charTalkNameText != null)
        {
            charTalkNameText.text = "??????";
        }
        queueDialogs.Clear();
        foreach (string textBuffer in texto.textArray)
        {
            queueDialogs.Enqueue(textBuffer);
        }
        NextPhrase();
    }
    public void NextPhrase()
    {
        if (FinalFeedbackSystem.IsShowing) return;
        if (queueDialogs.Count == 0)
        {
            return;
        }
//        Debug.Log(textVariables);
        if (isWriting == false)
        {
            if (blinkingSprite != null)
            {
                StopCoroutine(blinkingSprite);
            }
            actualPhrase = queueDialogs.Dequeue();
            CheckVariable(actualPhrase);
            CheckTalker();
            if (isWriting)
            {
                return;
            }
            //  CheckExpretion(actualPhrase);
            //actualPhrase = actualPhrase[1..^0];
            isWriting = true;
            //screenImage.sprite = expretion;
            StartCoroutine(ShowCharts(actualPhrase, textSpeed));

        }
    }
    public void AddPhrase(string newPhrase)
    {
        Queue<string> modDialogs = new();
        modDialogs.Enqueue(newPhrase);
        foreach (string textBuffer in queueDialogs)
        {
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
    IEnumerator ShowCharts(string textToShow, float textSpeed)
    {
        screenText.text = "";
//        Debug.Log("corutina iniciada");
        isWritingPhrase = textToShow;
        int characterCount = 0;
        foreach (char character in textToShow.ToCharArray())
        {
//            Debug.Log("corutina iniciada");
            screenText.text += character;
            if (character != ' ' && characterCount % 2 == 0)
            {
                audioController.GetComponent<AudioSystem>().PlayCharSound();
            }

            characterCount++;
            yield return new WaitForSeconds(textSpeed);
        }
        isWriting = false;
        blinkingSprite = StartCoroutine(SpriteBlinking());

    }
    IEnumerator SpriteBlinking()
    {
        while (gameObject.GetComponent<Button>().interactable)
        {
            string baseText = screenText.text;
            string spriteName = " <sprite=0>";
            screenText.text += spriteName;
            yield return new WaitForSeconds(0.5f);
            screenText.text = baseText;
            yield return new WaitForSeconds(0.5f);
        }
    }
    public void IsPressed()
    {
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
    public void SkipWriting(){
        if (isWriting==true){
//            Debug.Log("skipeado");
            StopAllCoroutines();
            screenText.text = isWritingPhrase;
            isWriting=false;
            blinkingSprite = StartCoroutine(SpriteBlinking());
        }
    }
    public void CheckVariable(string actualPhrase)
    {
        GetVariables();
        foreach (string variable in textVariables)
        {
            if (actualPhrase.Contains("{" + variable + "}"))
            {
                FieldInfo field = typeof(ClinicalCase).GetField(variable);
                if (field != null)
                {
                    object value = field.GetValue(clinicalCase);
                    actualPhrase = actualPhrase.Replace("{" + variable + "}", value?.ToString() ?? "");
                    //TODO feedback pero ahora si feedback
                    /*if (variable == "feedback")
                    {
                        if (correctAnswer == 2)
                        {
                            actualPhrase = actualPhrase.Replace("{" + variable + "}", value?.ToString() ?? "");
                        }
                        else
                        {
                            actualPhrase = "El paciente murio";
                        }
                    }*/
                }
            }
        }
        if (actualPhrase.Contains("{dname}"))
        {
            actualPhrase = actualPhrase.Replace("{dname}", saveFile.playerName);
        }
        if (actualPhrase.Contains("{choice}"))
        {
            rawChoices = actualPhrase;
            var regext1 = new Regex(@"\{choice\}\{choicest1\s+b:\s*((?:""[^""]*""\s*)*)c:\s*""[^""]*""\}");
            if (regext1.Match(actualPhrase).Success)
                actualPhrase = regext1.Replace(actualPhrase, "");

            var regext2 = new Regex(@"\{choice\}\{choicest2\s+c:\s*((?:""[^""]*""\s*)*)\}");
            if (regext2.Match(actualPhrase).Success)
                actualPhrase = regext2.Replace(actualPhrase, "");

            gameObject.GetComponent<Button>().interactable = false;

            var choice = choiceBox.GetComponent<ChoiceSystem>();
            choice.cancelable = false;              // las preguntas del guion no se cancelan
            choiceBox.SetActive(true);
            choice.ChoiceMenu(rawChoices);          // antes lo hacía ChoiceSystem.Update
        }
        if (actualPhrase.Contains("{main}"))
        {
            // Busca el primer {m} pendiente y cuántas frases hay que saltar hasta él
            int skip = 0;
            string mLine = null;
            foreach (string dialog in queueDialogs)
            {
                skip++;
                if (dialog.Contains("{m}")) { mLine = dialog; break; }
            }

            if (mLine != null)
            {
                // Si el último paso fue un examen, ExamLoop lo registra y reabre su menú.
                // Devuelven true si ese era el último examen permitido.
                bool examsOver = inspectionButton.GetComponent<ExamSystem>().ExamLoop();
                bool extraOver = extraExamButton.GetComponent<ExamSystem>().ExtraExamLoop();

                // Saltar ANTES del interrogatorio: InterrogationLoop puede encolar frases al frente
                SkipPhrase(skip);
                interrogationButton.GetComponent<InterrogationSystem>().InterrogationLoop();

                if (examsOver || extraOver)
                {
                    // Se descarta el "¿Qué debería hacer?" y se pasa directo a la siguiente línea del guion
                    actionMenu.Close();
                    extraMenu.Close();
                    actualPhrase = "{Ndname}" + (examsOver
                        ? "(No puedo realizarle mas examenes al paciente)"
                        : "(creo que ya tengo todo lo que necesito)");
                    PushFront("{nLine}");
                }
                else
                {
                    actualPhrase = mLine.Replace("{m}", "");
                }
            }
        }
        //chapuza historica
        if (actualPhrase.Contains("{m}"))
        {
            actualPhrase = actualPhrase.Replace("{m}", "");
        }
        if (actualPhrase.Contains("{a}"))
        {
            if (interrogationButton.GetComponent<InterrogationSystem>().GetInterrogationTime() == false)
            {
                actionMenu.Open(); //actionMenu.SetActive(true); antes
            }
            clinicalHistoryButton.SetActive(true);
            actualPhrase = actualPhrase.Replace("{a}", "");
        }
        if (actualPhrase.Contains("{extra}"))
        {
            if (interrogationButton.GetComponent<InterrogationSystem>().GetInterrogationTime() == false)
            {
                extraMenu.Open();//extraMenu.SetActive(true); Antes
            }
            clinicalHistoryButton.SetActive(true);
            actualPhrase = actualPhrase.Replace("{extra}", "");
        }
        if (actualPhrase.Contains("{end}"))
        {
            saveFileController.GetComponent<SaveUploader>().Upload();
            queueDialogs.Clear();
            actualPhrase = "";
            finalFeedback.ShowFinalPanel(diagnosisController.GetComponent<DiagnosisSystem>().isDiagnosisCorrect);
        }
        if(actualPhrase.Contains("{diag}")){
            choiceBox.transform.GetChild(1).gameObject.SetActive(false);
            diagnosisController.GetComponent<DiagnosisSystem>().SelectDiagnosis();
            actualPhrase = actualPhrase.Replace("{diag}", "");
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
//            Debug.Log("bgValue " + bgValue);
            bgController.GetComponent<BackgroundSystem>().ChangeBg(bgValue);
        }//https://www.twitch.tv/doctorfefe
        //https://www.twitch.tv/retrohipplayer
if (actualPhrase.Contains("{ch:"))
{
    var regex = new Regex(@"\{ch:\s*""([^""]*)""\s*\}");
    var match = regex.Match(actualPhrase);

    var regexVisibility = new Regex(@"\{ch:\s*""([^""]*)""\s+([01])\s*\}");
    var matchVisibility = regexVisibility.Match(actualPhrase);

    string chValue = String.Empty;
    bool chVisibility = true;

    if (match.Success)
    {
        chValue = match.Groups[1].Value;
        actualPhrase = regex.Replace(actualPhrase, "");
    }

    if (matchVisibility.Success)
    {
        chValue = matchVisibility.Groups[1].Value; // si también quieres el valor
        chVisibility = matchVisibility.Groups[2].Value == "1";
        actualPhrase = regexVisibility.Replace(actualPhrase, "");
    }
            bgController.GetComponent<BackgroundSystem>().ChangeCh(chValue, chVisibility);
        }
        if (actualPhrase.Contains("{nLine}"))
        {
            NextLine();
        }


        this.actualPhrase = actualPhrase;
    }
    public void SkipPhrase(int timesToSkip = 1)
    {
        for (int i = 1; i <= timesToSkip; i++)
        {
            queueDialogs.Dequeue();
        }
    }
    public void CheckTalker()
    {
        if (actualPhrase != null)
        {
            var regextNdname = new Regex(@"\{Ndname\}");
//            Debug.Log("match estado Ndname: " + regextNdname.Match(actualPhrase).Success);
            if (regextNdname.Match(actualPhrase).Success)
            {
                charTalkNameText.text = saveFile.playerName;
                actualPhrase = regextNdname.Replace(actualPhrase, "");
            }

            var regextNpname = new Regex(@"\{Npname\}");
//            Debug.Log("match estado Npname: " + regextNpname.Match(actualPhrase).Success);
            if (regextNpname.Match(actualPhrase).Success)
            {
                GetVariables();
                FieldInfo field = typeof(ClinicalCase).GetField("nombrePaciente");
                if (field != null)
                {
                    object value = field.GetValue(clinicalCase);
                    if (charTalkNameText != null)
                    {
//                        Debug.Log("cambio de nombre de paciente");
                        charTalkNameText.text = value.ToString();
                    }
                }
                actualPhrase = regextNpname.Replace(actualPhrase, "");
            }
            var regextNoname = new Regex(@"\{Noname\}");

            if (regextNoname.Match(actualPhrase).Success)
            {
                charTalkNameText.text = "??????";
                actualPhrase = regextNoname.Replace(actualPhrase, "");
            }
        }

    }
    public void NextLine()
    {
        texto.ChangeLine(clinicalCase.scriptPath);
        ShowText();
    }
    // Pone una frase al inicio de la cola SIN llamar a NextPhrase (a diferencia de AddPhrase)
    private void PushFront(string phrase)
    {
        var q = new Queue<string>();
        q.Enqueue(phrase);
        foreach (string p in queueDialogs) q.Enqueue(p);
        queueDialogs = q;
    }
    // TODO que el texto pueda salir a diferentes velocidades
}
