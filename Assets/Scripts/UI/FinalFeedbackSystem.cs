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

    [Tooltip("Nombres exactos de las decisiones que no puntúan (examen extra, diagnóstico). No cuentan en el resumen.")]
    [SerializeField] private List<string> unscoredDecisions = new List<string>();

    [Tooltip("Si está activado, muestra [Relevante]/[No relevante] en cada decisión y el total. Si está desactivado, solo muestra las decisiones y el número de aciertos.")]
    public bool showRelevance = true;

    [SerializeField] private TextMeshProUGUI summaryText;    // Ej: "Decisiones relevantes: 4 / 9" o "Aciertos: 4"
    [SerializeField] private TextMeshProUGUI resultText;     // "Enhorabuena" o "Sigue intentando"
    [SerializeField] private TextMeshProUGUI diagnosisText;  // Ej: "Diagnóstico seleccionado: incorrecto"

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

        // Solo cuentan las decisiones que puntúan
        List<DecisionRecord> scored = interrogation.Concat(examination)
            .Where(r => IsScored(r.decision))
            .ToList();

        int total = scored.Count;
        int relevant = scored.Count(r => r.isRelevant);

        bool reachedThreshold = relevant >= relevantThreshold;
        bool success = reachedThreshold && diagnosisCorrect;

        string relevantNumber = success
            ? "<color=#2ECC71>" + relevant + "</color>"
            : relevant.ToString();

        if (summaryText != null)
        {
            summaryText.text = showRelevance
                ? "Decisiones relevantes: " + relevantNumber + " / " + total
                : "Aciertos: " + relevantNumber;
        }

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
            // Sin relevancia, o decisión sin puntaje: solo el nombre
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
}