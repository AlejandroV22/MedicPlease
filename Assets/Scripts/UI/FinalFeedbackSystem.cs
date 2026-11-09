using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using System.Linq;
public class FinalFeedbackSystem : MonoBehaviour
{
    public static bool IsShowing { get; private set; }

    [SerializeField] private GameObject feedbackPanel;      
    [SerializeField] private TextMeshProUGUI interrogationText;
    [SerializeField] private TextMeshProUGUI examinationText;
    [SerializeField] private GameDataController gameDataController;

    private void Awake()
    {
        feedbackPanel.SetActive(false);
        IsShowing = false;
    }

    public void ShowFinalPanel()
    {
        ClinicalCase clinicalCase = GameManager.Instance.SelectedCase;
        CaseRecord caseRecord = gameDataController.GetCaseRecord(clinicalCase.name);
        AttemptRecord attempt = caseRecord?.LastAttempt;

        List<DecisionRecord> interrogation = new List<DecisionRecord>();
        List<DecisionRecord> examination = new List<DecisionRecord>();
        HashSet<string> seen = new HashSet<string>();

        if (attempt != null)
        {
            foreach (DecisionRecord record in attempt.decisions)
            {
                
                if (!seen.Add(record.decision)) continue;

                if (clinicalCase.questions.Contains(record.decision))
                    interrogation.Add(record);
                else
                    examination.Add(record);
            }
        }

        interrogationText.text = BuildText(interrogation);
        examinationText.text = BuildText(examination);

        feedbackPanel.SetActive(true);
        feedbackPanel.transform.SetAsLastSibling(); 
        IsShowing = true;
    }

    private string BuildText(List<DecisionRecord> records)
    {
        if (records.Count == 0)
            return "No se registraron decisiones.";

        StringBuilder sb = new StringBuilder();
        foreach (DecisionRecord record in records)
        {
            string tag = record.isRelevant
                ? "<color=#2ECC71>[Relevante]</color>"
                : "<color=#E74C3C>[No relevante]</color>";

            sb.AppendLine(record.decision + " " + tag);
        }
        return sb.ToString().TrimEnd();
    }

    public void Close()
    {
        feedbackPanel.SetActive(false);
        IsShowing = false;
    }
}