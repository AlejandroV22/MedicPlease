using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FinalFeedbackSystem : MonoBehaviour
{
    public static bool IsShowing { get; private set; }

    [SerializeField] private GameObject feedbackPanel;
    [SerializeField] private TextMeshProUGUI interrogationText;
    [SerializeField] private TextMeshProUGUI examinationText;
    [SerializeField] private GameDataController gameDataController;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform contentRect;

    [Tooltip("Número mínimo de decisiones relevantes para desbloquear el diagnóstico.")]
    [SerializeField] private int relevantThreshold = 3;

    [Tooltip("Nombres exactos de decisiones que NO puntúan (por ejemplo, el diagnóstico si se registra como decisión). " +
             "Los exámenes normales, los extra y las preguntas de interrogatorio sí puntúan y no deben estar aquí.")]
    [SerializeField] private List<string> unscoredDecisions = new List<string>();

    [Tooltip("Si está activado, muestra [Relevante]/[No relevante] en cada decisión y el total. Si está desactivado, solo muestra las decisiones y el número de aciertos.")]
    public bool showRelevance = true;

    [SerializeField] private TextMeshProUGUI summaryText;    // Ej: "Decisiones relevantes: 4 / 9" o "Aciertos: 4"
    [SerializeField] private TextMeshProUGUI resultText;     // "Enhorabuena" o "Sigue intentando"
    [SerializeField] private TextMeshProUGUI diagnosisText;  // Ej: "Diagnóstico seleccionado: incorrecto"

    [Header("Puntaje y rango")]
    [Tooltip("Si está activado, un diagnóstico incorrecto da rango F sin importar el puntaje.")]
    [SerializeField] private bool requireCorrectDiagnosis = true;
    [Tooltip("Puntaje mínimo (0 a 100) para cada rango. Se evalúan de mayor a menor.")]
    [SerializeField] private int minScoreA = 90;
    [SerializeField] private int minScoreB = 75;
    [SerializeField] private int minScoreC = 60;
    [SerializeField] private int minScoreD = 45;
    [SerializeField] private TextMeshProUGUI scoreText;      // Ej: "Puntaje: 75 / 100"
    [SerializeField] private TextMeshProUGUI gradeText;      // Ej: "Rango: B"

    private void Awake()
    {
        feedbackPanel.SetActive(false);
        IsShowing = false;
    }

    // diagnosisCorrect lo envía DialogSystem
    public void ShowFinalPanel(bool diagnosisCorrect)
    {
        ClinicalCase clinicalCase = GameManager.Instance.SelectedCase;
        CaseRecord caseRecord = gameDataController.GetCaseRecord(clinicalCase.name);
        AttemptRecord attempt = caseRecord?.LastAttempt;

        List<DecisionRecord> interrogation = new List<DecisionRecord>();
        List<DecisionRecord> examination = new List<DecisionRecord>();   // incluye exámenes normales y extra
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

        // Puntúan interrogatorio, exámenes normales y exámenes extra.
        List<DecisionRecord> scored = interrogation.Concat(examination)
            .Where(r => IsScored(r.decision))
            .ToList();

        int total = scored.Count;
        int relevant = scored.Count(r => r.isRelevant);

        bool reachedThreshold = relevant >= relevantThreshold;
        bool success = reachedThreshold && diagnosisCorrect;

        // Puntaje: porcentaje de decisiones relevantes entre las que puntúan
        int score = total > 0 ? Mathf.RoundToInt(100f * relevant / total) : 0;
        string grade = CalculateGrade(score, diagnosisCorrect);

        string relevantNumber = success
            ? "<color=#2ECC71>" + relevant + "</color>"
            : relevant.ToString();

        if (summaryText != null)
        {
            summaryText.text = showRelevance
                ? "Decisiones relevantes: " + relevantNumber + " / " + total
                : "Aciertos: " + relevantNumber;
        }

        if (scoreText != null)
            scoreText.text = "Puntaje: " + score + " / 100";

        if (gradeText != null)
            gradeText.text = "Rango: " + grade;

        if (resultText != null)
            resultText.text = success ? "Enhorabuena" : "Sigue intentando";

        if (diagnosisText != null)
            diagnosisText.text = "Diagnóstico seleccionado: " + (diagnosisCorrect ? "correcto" : "incorrecto");

        feedbackPanel.SetActive(true);
        feedbackPanel.transform.SetAsLastSibling();
        IsShowing = true;

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
        scrollRect.verticalNormalizedPosition = 1f;
    }

    // Convierte el puntaje en rango. El diagnóstico incorrecto puede forzar F.
    private string CalculateGrade(int score, bool diagnosisCorrect)
    {
        if (requireCorrectDiagnosis && !diagnosisCorrect) return "F";
        if (score >= minScoreA) return "A";
        if (score >= minScoreB) return "B";
        if (score >= minScoreC) return "C";
        if (score >= minScoreD) return "D";
        return "F";
    }

    private bool IsScored(string decision)
    {
        foreach (string unscored in unscoredDecisions)
        {
            if (string.Equals(unscored?.Trim(), decision?.Trim(), StringComparison.OrdinalIgnoreCase))
                return false;
        }
        return true;
    }

    private string BuildText(List<DecisionRecord> records)
    {
        if (records.Count == 0)
            return "No se registraron decisiones.";

        StringBuilder sb = new StringBuilder();
        foreach (DecisionRecord record in records)
        {
            if (!showRelevance || !IsScored(record.decision))
            {
                sb.AppendLine(record.decision);
                continue;
            }

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

    public static void ResetState() => IsShowing = false;
}