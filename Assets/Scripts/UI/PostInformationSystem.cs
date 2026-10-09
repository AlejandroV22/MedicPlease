using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using System.Linq;

public class PostInformationSystem : MonoBehaviour
{
    public GameObject gameDataController;
    public GameData saveData;
    public ClinicalCase clinicalCase;
    public Texto guion;
    public TextMeshProUGUI interrogationDecisionsText;
    public TextMeshProUGUI examinationDecisionsText;

    private List<string> interrogationQuestions = new List<string>();
    private List<string> examinationQuestions = new List<string>();
    private Dictionary<string, string[]> interrogationQuestionsAnswers = new Dictionary<string, string[]>();
    private Dictionary<string, string[]> examinationQuestionsAnswers = new Dictionary<string, string[]>();

    private void Start()
    {
        clinicalCase = GameManager.Instance.SelectedCase;
        saveData = gameDataController.GetComponent<GameDataController>().LoadData();
        guion.GetTextToFile(clinicalCase.scriptPath);
        GetLastDecisions();
    }

    public void GetLastDecisions()
    {
        interrogationQuestions.Clear();
        examinationQuestions.Clear();
        interrogationQuestionsAnswers.Clear();
        examinationQuestionsAnswers.Clear();

        if (clinicalCase == null || saveData == null) return;

        foreach (string decision in saveData.decisionsMade)
        {
            if (clinicalCase.questions.Contains(decision))
            {
                if (!interrogationQuestions.Contains(decision))
                    interrogationQuestions.Add(decision);
            }
            else
            {
                if (!examinationQuestions.Contains(decision))
                    examinationQuestions.Add(decision);
            }
        }

        FillAnswers(interrogationQuestions, interrogationQuestionsAnswers);
        FillAnswers(examinationQuestions, examinationQuestionsAnswers);

        interrogationDecisionsText.text = BuildText(interrogationQuestions, interrogationQuestionsAnswers);
        examinationDecisionsText.text = BuildText(examinationQuestions, examinationQuestionsAnswers);
    }

    private void FillAnswers(List<string> questions, Dictionary<string, string[]> target)
    {
        foreach (string question in questions)
        {
            for (int i = 0; i < guion.textArray.Length; i++)
            {
                if (guion.textArray[i].Contains("{" + question + "}"))
                {
                    target[question] = GetAnswerInterrogation(i, question);
                    break; // ya la encontramos
                }
            }
        }
    }

    private string BuildText(List<string> questions, Dictionary<string, string[]> answers)
    {
        StringBuilder sb = new StringBuilder();
        foreach (string question in questions)
        {
            sb.AppendLine("<b>" + question + "</b>");

            if (answers.TryGetValue(question, out string[] lines))
            {
                foreach (string line in lines)
                    sb.AppendLine(line);
            }
            sb.AppendLine(); // línea en blanco entre preguntas
        }
        return sb.ToString().TrimEnd();
    }

    public string[] GetAnswerInterrogation(int index, string question)
    {
        List<string> answer = new List<string>();
        string pattern = @"\{" + Regex.Escape(question) + @"\}|\{Ndname\}|\{Npname\}";

        for (int i = index; i < guion.textArray.Length; i++)
        {
            // Si llegamos a {main}, terminamos (esa línea no se incluye)
            if (guion.textArray[i].Contains("{main}")) break;

            string clean = Regex.Replace(guion.textArray[i], pattern, "").Trim();
            if (!string.IsNullOrEmpty(clean))
                answer.Add(clean);
        }

        return answer.ToArray();
    }
}