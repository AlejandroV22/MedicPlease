using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiagnosisSystem : MonoBehaviour
{
    public ClinicalCase clinicalCase;
    public GameDataController gameDataController;
    private string[] questions;
    private bool[] isRelevantQuestions;
    public List<string> relevantDecisionMade;
    public GameObject inspectionButton;
    public GameObject listeningButton;
    public GameObject touchingButton;
    public GameObject mentalButton;
    public GameObject nervesButton;
    public GameObject sensitivityButton;
    public GameObject choiceBox;
    public GameObject extraExamButton;
    public bool[] relevantDiagnosis;
    public int totalRelevant = 0;
    public int totalRelevantBeforeExtraExams = 0;
    private string rawChoices;
    public bool isDiagnosisCorrect = false;
    private List<string> diagnosisWithCorrect = new List<string>();
    private List<string> diagnosisWithoutCorrect = new List<string>();

    void Start()
    {
        clinicalCase = GameManager.Instance.SelectedCase;
        questions = clinicalCase.questions;
        isRelevantQuestions = clinicalCase.relevantQuestions;
    }

    public void OptainDiagnosis()
    {
        diagnosisWithCorrect.Clear();
        diagnosisWithoutCorrect.Clear();
        string[] wrongs = clinicalCase.wrongDiagnosis;
        string correct = clinicalCase.correctDiagnosis;
        List<bool> tempRelevant = new List<bool>();
        for (int i = 0; i < wrongs.Length; i++)
        {
            diagnosisWithoutCorrect.Add(wrongs[i]);
            Debug.Log(i + " total: " + wrongs.Length);
            if (i != wrongs.Length - 1)
            {
                diagnosisWithCorrect.Add(wrongs[i]);
            }
        }
        diagnosisWithCorrect.Add(correct);
        for(int i = 0; i< diagnosisWithCorrect.Count(); i++){
            if(diagnosisWithCorrect[i] == correct){
                tempRelevant.Add(true);
            }else{
                tempRelevant.Add(false);
            }
        }
        relevantDiagnosis = tempRelevant.ToArray();
    }

    // ───────────────────────── Registro de decisiones ─────────────────────────

    /// <summary>
    /// Punto unico de registro: guarda la decision en el intento activo junto con su relevancia
    /// y, si la decision es un diagnostico, cierra el intento indicando si fue el correcto.
    /// Lo llama ChoiceSystem cada vez que el jugador elige una opcion.
    /// </summary>
    public void RegisterDecision(string decision, ExamCategory category = ExamCategory.None)
    {
        EnsureCase();
        gameDataController.RegisterDecision(decision, IsRelevantDecision(decision, category));

        if (IsDiagnosisOption(decision))
        {
            gameDataController.FinishAttempt(decision == clinicalCase.correctDiagnosis);
            if(decision == clinicalCase.correctDiagnosis){
                isDiagnosisCorrect = true;
            }else{
                isDiagnosisCorrect = false;
            }
        }
    }

    /// <summary>True si la decision es un examen o pregunta marcada como relevante en el caso.</summary>
    public bool IsRelevantDecision(string decision, ExamCategory category = ExamCategory.None)
    {
        EnsureCase();

        // Examenes: la relevancia ahora sale del ClinicalCase
        if (clinicalCase.IsExamRelevant(category, decision))
        {
            return true;
        }

        for (int i = 0; i < questions.Length; i++)
        {
            if (decision == questions[i] && i < isRelevantQuestions.Length && isRelevantQuestions[i])
            {
                return true;
            }
        }
        if(diagnosisWithCorrect !=null){
            for(int i = 0; i< diagnosisWithCorrect.Count(); i++){
            if (decision == diagnosisWithCorrect[i] && i < relevantDiagnosis.Length && relevantDiagnosis[i])
            {
                return true;
            }
            }
        }
        return false;

    }

    public bool IsDiagnosisOption(string option)
    {
        EnsureCase();
        return option == clinicalCase.correctDiagnosis || clinicalCase.wrongDiagnosis.Contains(option);
    }

    private void EnsureCase()
    {
        if (clinicalCase == null)
        {
            clinicalCase = GameManager.Instance.SelectedCase;
            questions = clinicalCase.questions;
            isRelevantQuestions = clinicalCase.relevantQuestions;
        }
    }

    // ───────────────────────── Consulta de decisiones relevantes ─────────────────────────

    /// <summary>Llena relevantDecisionMade con las decisiones relevantes del intento activo.</summary>
    public void GetRelevantDecisions()
    {
        EnsureCase();
        relevantDecisionMade.Clear();
        OptainDiagnosis();
        totalRelevant = clinicalCase.minimumCorrectExams;

        AttemptRecord attempt = gameDataController.CurrentAttempt;
        if (attempt == null) return;

        relevantDecisionMade.AddRange(
            attempt.decisions.Where(d => d.isRelevant).Select(d => d.decision));
    }

    /// <summary>True si en el intento activo el jugador ya eligió el examen extra necesario para el diagnóstico.</summary>
public bool DidNecessaryExtraExam()
{
    EnsureCase();
    AttemptRecord attempt = gameDataController.CurrentAttempt;
    if (attempt == null) return false;

    return attempt.decisions.Any(d =>
        clinicalCase.IsExamRelevant(ExamCategory.Extra, d.decision));
}

public void SelectDiagnosis()
{
    Start();
    GetRelevantDecisions();   // también llama a OptainDiagnosis() para llenar las listas

    List<string> source = DidNecessaryExtraExam() ? diagnosisWithCorrect : diagnosisWithoutCorrect;

    rawChoices = "{choice}{choicest2 c:";
    foreach (string diagnosis in source)
        rawChoices += "\"" + diagnosis + "\" ";
    rawChoices += "} ";

    Debug.Log(rawChoices);
    choiceBox.SetActive(true);
    choiceBox.GetComponent<ChoiceSystem>().ChoiceMenu(rawChoices);
}
}
