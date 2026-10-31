using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;

public class DiagnosisSystem : MonoBehaviour
{
    public ClinicalCase clinicalCase;
    public GameDataController gameDataController;
    private GameData gameData;
    private List<string> decisionMade;
    private bool[] isRelevantExams;
    private string[] exams;
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
    public int totalRelevant = 0;
    public int totalRelevantBeforeExtraExams = 0;
    private string rawChoices;
    private List<string> diagnosisWithCorrect = new List<string>();
    private List<string> diagnosisWithoutCorrect = new List<string>();
    void Start()
    {
        clinicalCase = GameManager.Instance.SelectedCase;
        questions = clinicalCase.questions;
        isRelevantQuestions = clinicalCase.relevantQuestions;
        GetExams();

    }
    private void GetExams()
    {
        string[] inspectionExams = inspectionButton.GetComponent<ExamSystem>().exams;
        string[] listeningExams = listeningButton.GetComponent<ExamSystem>().exams;
        string[] touchingExams = touchingButton.GetComponent<ExamSystem>().exams;
        string[] mentalExams = mentalButton.GetComponent<ExamSystem>().exams;
        string[] nervesExams = nervesButton.GetComponent<ExamSystem>().exams;
        string[] sensitivityExams = sensitivityButton.GetComponent<ExamSystem>().exams;
        string[] extraExams = extraExamButton.GetComponent<ExamSystem>().exams;
        bool[] inspectionRelevant = inspectionButton.GetComponent<ExamSystem>().relevantExams;
        bool[] listeningRelevant = listeningButton.GetComponent<ExamSystem>().relevantExams;
        bool[] touchingRelevant = touchingButton.GetComponent<ExamSystem>().relevantExams;
        bool[] mentalRelevant = mentalButton.GetComponent<ExamSystem>().relevantExams;
        bool[] nervesRelevant = nervesButton.GetComponent<ExamSystem>().relevantExams;
        bool[] sensitivityRelevant = sensitivityButton.GetComponent<ExamSystem>().relevantExams;
        bool[] extraRelevant = extraExamButton.GetComponent<ExamSystem>().relevantExams;
        exams = new[]{
            inspectionExams,
            listeningExams,
            touchingExams,
            mentalExams,
            nervesExams,
            sensitivityExams,
            extraExams
        }.SelectMany(arr => arr).ToArray();
        isRelevantExams = new[]{
            inspectionRelevant,
            listeningRelevant,
            touchingRelevant,
            mentalRelevant,
            nervesRelevant,
            sensitivityRelevant,
            extraRelevant
        }.SelectMany(arr => arr).ToArray();
    }
    public void OptainDiagnosis()
    {
        diagnosisWithCorrect.Clear();
        diagnosisWithoutCorrect.Clear();
        string[] wrongs = clinicalCase.wrongDiagnosis;
        string correct = clinicalCase.correctDiagnosis;
        for (int i = 0; i < wrongs.Length; i++)
        {
            diagnosisWithoutCorrect.Add(wrongs[i]);

            if (i != wrongs.Length - 1)
            {
                diagnosisWithCorrect.Add(wrongs[i]);
            }
        }
        diagnosisWithCorrect.Add(correct);
    }
    public void GetRelevantDecisions()
    {
        relevantDecisionMade.Clear();
        OptainDiagnosis();
        gameData = gameDataController.LoadData();
        decisionMade = gameData.decisionsMade;
        totalRelevant = clinicalCase.minimumCorrectExams;//isRelevantExams.Count(x => x) + isRelevantQuestions.Count(x => x)
        foreach (string decision in decisionMade)
        {
            for (int i = 0; i < exams.Length; i++)
            {
                if (decision == exams[i])
                {
                    if (isRelevantExams[i] == true)
                    {
                        relevantDecisionMade.Add(decision);
                    }
                }
            }
            for (int i = 0; i < questions.Length; i++)
            {
                if (decision == questions[i])
                {
                    if (isRelevantQuestions[i] == true)
                    {
                        relevantDecisionMade.Add(decision);
                    }
                }
            }
        }
    }
    public void SelectDiagnosis()
    {
        Start();
        GetRelevantDecisions();
        gameDataController.SaveData(relevantDecisionMadeReceived: relevantDecisionMade);
        rawChoices = "{choice}{choicest2 c:";
        if (relevantDecisionMade.Count() == totalRelevantBeforeExtraExams + 1)
        {
            foreach (string diagnosis in diagnosisWithCorrect)
            {
                rawChoices = @rawChoices + "\"" + diagnosis + "\"" + " ";
            }
            rawChoices = rawChoices + "} ";
            Debug.Log(rawChoices);
            choiceBox.SetActive(true);
            choiceBox.GetComponent<ChoiceSystem>().ChoiceMenu(rawChoices);
        }
        else
        {
            foreach (string diagnosis in diagnosisWithoutCorrect)
            {
                rawChoices = @rawChoices + "\"" + diagnosis + "\"" + " ";
            }
            rawChoices = rawChoices + "} ";
            Debug.Log(rawChoices);
            choiceBox.SetActive(true);
            choiceBox.GetComponent<ChoiceSystem>().ChoiceMenu(rawChoices);

        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
