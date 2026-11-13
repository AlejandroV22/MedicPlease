using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExamSystem : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject choiceBox;
    public GameObject dialogSystem;
    public GameObject diagnosisController;
    [Tooltip("Solo en el ExamSystem que DialogSystem llama en {main} (inspección)")]
    public Panel examinationMenu;
    [Tooltip("Solo en el ExamSystem del botón de examen extra")]
    public Panel extraMenu;

    [Header("Opciones")]
    public ExamCategory category;
    public string[] exams;

    private void Awake()
    {
        Debug.Assert(choiceBox != null, "choiceBox no asignado en ExamSystem", this);
        Debug.Assert(dialogSystem != null, "dialogSystem no asignado en ExamSystem", this);
    }

    // ---------- Botones de examen ----------

    public void IsExaminationOver(string examinationType)
    {
        var dialog = dialogSystem.GetComponent<DialogSystem>();
        if (!ExamManager.Instance.CanStartExam())
        {
            dialog.AddPhrase("(No puedo realizarle mas examenes al paciente)");
            dialog.AddPhrase("{nLine}");
            return;
        }
        dialog.AddPhrase("¿Qué debería " + examinationType + "?");
        ShowExam();
    }

    public void ShowExam()
    {
        if (!ExamManager.Instance.CanStartExam()) return;

        ExamManager.Instance.StartExam();
        OpenChoice(ResolveExams(), category);
    }

    string[] ResolveExams()
    {
        if (category != ExamCategory.None)
        {
            string[] fromCase = GameManager.Instance.SelectedCase.GetExamNames(category);
            if (fromCase.Length > 0) return fromCase;
        }
        return exams;
    }

    // ---------- Examen extra ----------

    public void IsExtraExaminationOver(string examinationType)
    {
        var dialog = dialogSystem.GetComponent<DialogSystem>();
        if (!ExamManager.Instance.CanStartExtraExam())
        {
            dialog.AddPhrase("(creo que ya tengo todo lo que necesito)");
            dialog.AddPhrase("{nLine}");
            return;
        }
        dialog.AddPhrase("¿Qué debería " + examinationType + "?");
        ShowExtraExam();
    }

    public void ShowExtraExam()
    {
        var mgr = ExamManager.Instance;
        if (!mgr.CanStartExtraExam()) return;

        mgr.StartExtraExam();

        var clinicalCase = GameManager.Instance.SelectedCase;
        var diagnosis = diagnosisController.GetComponent<DiagnosisSystem>();
        diagnosis.GetRelevantDecisions();

        string[] options;
        if (diagnosis.relevantDecisionMade.Count() >= clinicalCase.minimumCorrectExams)
        {
            options = clinicalCase.correctExtraExams;
            diagnosis.totalRelevantBeforeExtraExams = diagnosis.relevantDecisionMade.Count();
        }
        else
        {
            options = clinicalCase.wrongExtraExams;
        }

        OpenChoice(options, ExamCategory.Extra);
    }

    // ---------- Llamadas desde DialogSystem en {main} ----------


    public void ExamLoop()
    {
        var mgr = ExamManager.Instance;
        if (!mgr.ExamTime) return;

        mgr.RegisterTry();
        examinationMenu.Open();
    }

    public void ExtraExamLoop()
    {
        var mgr = ExamManager.Instance;
        if (!mgr.extraExamTime) return;

        mgr.RegisterExtraTry();
        extraMenu.Open();
    }

    // ---------- Auxiliares ----------

    void OpenChoice(IEnumerable<string> options, ExamCategory cat)
    {
        var choice = choiceBox.GetComponent<ChoiceSystem>();
        choice.currentCategory = cat;
        choice.cancelable = true;               // los exámenes sí se pueden cancelar
        choiceBox.SetActive(true);
        choice.ChoiceMenu(ChoiceSystem.BuildRawChoices(options));
    }
}