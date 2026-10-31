using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class ChoiceSystem : MonoBehaviour
{
    [Header("UI de opciones (botones)")]
    [Tooltip("Objeto con VerticalLayoutGroup donde se crean los botones")]
    public Transform buttonContainer;
    [Tooltip("Prefab de boton con un TMP_Text como hijo")]
    public Button optionButtonPrefab;

    [Header("Datos")]
    public List<string> choices = new List<string>();
    public string rawChoices;
    public GameObject gameDataController;
    public List<string> decisionsMade = new List<string>();

    private GameObject dialogBox;
    private bool isInteractable;
    private string correctAnswer;
    private bool menuGenerated = false;
    private List<string> badChoices = new List<string>();
    private readonly List<Button> spawnedButtons = new List<Button>();
    private int searchTimes = 0;

    private void Start()
    {
        dialogBox = GameObject.Find("DialogBox");
        gameObject.SetActive(false);
    }

    public void GetStart()
    {
        Start();
    }

    private void Update()
    {
        rawChoices = dialogBox.GetComponent<DialogSystem>().rawChoices;
        isInteractable = dialogBox.GetComponent<Button>().interactable;
        if (!isInteractable)
        {
            if (!menuGenerated)
            {
                ChoiceMenu(rawChoices);
                menuGenerated = true;
            }
        }
        else
        {
            menuGenerated = false;
        }
    }

    private void OnDisable()
    {
        menuGenerated = false;
    }

    // ---------- Manejo de botones ----------

    private void ClearButtons()
    {
        foreach (Button b in spawnedButtons)
        {
            if (b != null) Destroy(b.gameObject);
        }
        spawnedButtons.Clear();
    }

    private void BuildButtons(List<string> options, System.Action<string> onSelected)
    {
        ClearButtons();
        foreach (string option in options)
        {
            Button btn = Instantiate(optionButtonPrefab, buttonContainer);
            btn.GetComponentInChildren<TMP_Text>().text = option;
            string captured = option;
            btn.onClick.AddListener(() => onSelected(captured));
            spawnedButtons.Add(btn);
        }

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)buttonContainer);
    }

    // ---------- Logica del menu ----------

    public void ChoiceMenu(string rawChoices)
    {
        ClearButtons();

        // TODO hacer que vayan a distintos caminos dependiendo de la decision
        if (rawChoices.Contains("choicest1"))
        {
            var regexBadChoices = new Regex(@"\{choice\}\{choicest1 b:(.*?) c:");
            var matchBadChoices = regexBadChoices.Match(rawChoices);
            badChoices = Regex.Matches(matchBadChoices.Groups[1].Value, @"""([^""]+)""")
                        .Select(m => m.Groups[1].Value)
                        .ToList();

            var regexCorrectAnswer = new Regex(@"\{choice\}\{choicest1 b:.*? c:""([^""]+)""\}");
            var matchCorrectAnswer = regexCorrectAnswer.Match(rawChoices);
            correctAnswer = matchCorrectAnswer.Groups[1].Value;

            RandomChoices(correctAnswer);
            BuildButtons(choices, ConfirmChoice);
        }
        else if (rawChoices.Contains("choicest2"))
        {
            var regexChoices = new Regex(@"\{choice\}\{choicest2\s+c:\s*(.*?)\}");
            var matchChoices = regexChoices.Match(rawChoices);
            choices = Regex.Matches(matchChoices.Groups[1].Value, @"""([^""]*)""")
                        .Select(m => m.Groups[1].Value)
                        .ToList();
            BuildButtons(choices, changePath);
        }

        dialogBox.GetComponent<Button>().interactable = false;
        menuGenerated = true;
    }

    public void RandomChoices(string rchoise)
    {
        choices.Clear();
        choices.Add(rchoise);
        foreach (string choice in badChoices)
        {
            choices.Add(choice);
        }
        choices = choices.OrderBy(x => UnityEngine.Random.value).ToList();
    }

    public void ConfirmChoice(string selected)
    {
        if (selected == correctAnswer)
        {
            dialogBox.GetComponent<DialogSystem>().correctAnswer += 1;
            Debug.Log("respuesta correcta");
        }
        dialogBox.GetComponent<Button>().interactable = true;
        gameObject.SetActive(false);
        dialogBox.GetComponent<DialogSystem>().NextPhrase();
    }

    public void changePath(string selectedOption)
    {
        decisionsMade.Add(selectedOption);
        gameDataController.GetComponent<GameDataController>().SaveData(decisionsMadeReceived: decisionsMade);
        searchTimes = 0;

        SearchPath(selectedOption);
    }

    public void SearchPath(string selectedOption)
    {
        searchTimes++; // TODO agregar un identificador de caminos y un identificador de donde termina el texto
        Texto fullText = dialogBox.GetComponent<DialogSystem>().texto;
        Queue<string> queueDialogs = dialogBox.GetComponent<DialogSystem>().queueDialogs;
        int timesToSkip = 0;
        bool dialogFound = false;

        foreach (string dialog in queueDialogs)
        {
            timesToSkip++;
            if (dialog.Contains("{" + selectedOption + "}"))
            {
                dialogBox.GetComponent<DialogSystem>().SkipPhrase(timesToSkip);
                var regex = new Regex(@"\{" + Regex.Escape(selectedOption) + @"\}");
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

        if (dialogFound == false)
        {
            foreach (string textBuffer in fullText.textArray)
            {
                queueDialogs.Enqueue(textBuffer);
            }
            dialogBox.GetComponent<DialogSystem>().queueDialogs = queueDialogs;
            if (searchTimes < 100)
            {
                SearchPath(selectedOption);
            }
            dialogBox.GetComponent<Button>().interactable = true;
            gameObject.SetActive(false);
            dialogBox.GetComponent<DialogSystem>().NextPhrase();
        }
    }
}