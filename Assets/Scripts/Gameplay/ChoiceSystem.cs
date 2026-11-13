using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceSystem : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Objeto con VerticalLayoutGroup donde se crean los botones")]
    public Transform buttonContainer;
    [Tooltip("Prefab de boton con un TMP_Text como hijo")]
    public Button optionButtonPrefab;
    [Tooltip("Botón Cerrar de este ChoiceBox")]
    public GameObject cancelButton;

    [Header("Referencias")]
    public GameObject dialogBox;

    [Header("Estado")]
    public ExamCategory currentCategory;
    [Tooltip("Solo las elecciones de examen se pueden cancelar")]
    public bool cancelable;

    public List<string> choices = new List<string>();

    private const int MaxSearchPasses = 100;
    private string correctAnswer;
    private List<string> badChoices = new List<string>();
    private readonly List<Button> spawnedButtons = new List<Button>();

    private DialogSystem Dialog => dialogBox.GetComponent<DialogSystem>();
    private Button DialogButton => dialogBox.GetComponent<Button>();

    private void Awake()
    {
        Debug.Assert(dialogBox != null, "dialogBox no asignado en ChoiceSystem", this);
    }

    // ---------- Texto de elección ----------

    public static string BuildRawChoices(IEnumerable<string> options)
    {
        var sb = new System.Text.StringBuilder("{choice}{choicest2 c:");
        foreach (string option in options)
            sb.Append('"').Append(option).Append("\" ");
        sb.Append("} ");
        return sb.ToString();
    }

    // ---------- Menú ----------

    public void ChoiceMenu(string rawChoices)
    {
        ClearButtons();
        if (cancelButton != null) cancelButton.SetActive(cancelable);

        if (rawChoices.Contains("choicest1"))
        {
            var matchBad = Regex.Match(rawChoices, @"\{choice\}\{choicest1 b:(.*?) c:");
            badChoices = Regex.Matches(matchBad.Groups[1].Value, "\"([^\"]+)\"")
                              .Select(m => m.Groups[1].Value)
                              .ToList();

            var matchCorrect = Regex.Match(rawChoices, @"\{choice\}\{choicest1 b:.*? c:""([^""]+)""\}");
            correctAnswer = matchCorrect.Groups[1].Value;

            RandomChoices(correctAnswer);
            BuildButtons(choices, ConfirmChoice);
        }
        else if (rawChoices.Contains("choicest2"))
        {
            var matchChoices = Regex.Match(rawChoices, @"\{choice\}\{choicest2\s+c:\s*(.*?)\}");
            choices = Regex.Matches(matchChoices.Groups[1].Value, "\"([^\"]*)\"")
                           .Select(m => m.Groups[1].Value)
                           .ToList();
            BuildButtons(choices, changePath);
        }

        DialogButton.interactable = false;
    }

    public void RandomChoices(string rightAnswer)
    {
        choices = new List<string> { rightAnswer };
        choices.AddRange(badChoices);
        choices = choices.OrderBy(_ => UnityEngine.Random.value).ToList();
    }

    // ---------- Botones ----------

    void ClearButtons()
    {
        foreach (Button b in spawnedButtons)
            if (b != null) Destroy(b.gameObject);
        spawnedButtons.Clear();
    }

    void BuildButtons(List<string> options, System.Action<string> onSelected)
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

    // ---------- Respuestas ----------

    // Preguntas de quiz del guion (choicest1)
    public void ConfirmChoice(string selected)
    {
        if (selected == correctAnswer)
        {
            Dialog.correctAnswer += 1;
            Debug.Log("respuesta correcta");
        }
        FinishChoice();
    }

    // Exámenes, preguntas de interrogatorio (choicest2)
    public void changePath(string selectedOption)
    {
        if (Dialog.GetIsWriting()) Dialog.SkipWriting();

        Dialog.diagnosisController.GetComponent<DiagnosisSystem>().RegisterDecision(selectedOption);
        SearchPath(selectedOption);
    }

    public void SearchPath(string selectedOption)
    {
        string tag = "{" + selectedOption + "}";
        Queue<string> queue = Dialog.queueDialogs;

        for (int pass = 0; pass < MaxSearchPasses; pass++)
        {
            int found = -1;
            string line = null;
            int index = 0;

            foreach (string candidate in queue)
            {
                index++;
                if (candidate.Contains(tag))
                {
                    found = index;
                    line = candidate;
                    break;
                }
            }

            if (found > 0)
            {
                Dialog.SkipPhrase(found);
                Dialog.AddPhrase(line.Replace(tag, ""));
                FinishChoice();
                return;
            }

            // No está más adelante: vuelve a encolar el guion completo y busca otra vez
            foreach (string textBuffer in Dialog.texto.textArray)
                queue.Enqueue(textBuffer);
        }

        Debug.LogWarning($"No se encontró el tag {tag} en el guion");
        FinishChoice();
    }

    // Cierra la elección y devuelve el control al diálogo
    void FinishChoice()
    {
        DialogButton.interactable = true;
        gameObject.SetActive(false);
        Dialog.NextPhrase();
    }

    // Botón Cerrar. Solo cancela exámenes: el menú queda abierto para volver a elegir.
    public void CancelChoice()
    {
        if (!cancelable) return;

        ExamManager.Instance.StopExam();
        ExamManager.Instance.StopExtraExam();
        gameObject.SetActive(false);
    }
}