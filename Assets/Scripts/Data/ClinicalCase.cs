using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExamRelevance
{
    public string examName;
    public bool isRelevant;
}

[CreateAssetMenu(fileName = "NuevoCasoClinico", menuName = "Clinical Cases/Caso Clínico")]
public class ClinicalCase : ScriptableObject
{
    [Header("Información del paciente")]
    public string nombrePaciente;
    public int edad;
    public string sexo;
    public string ocupacion;
    public string lugarProcedencia;
    public Sprite retratoPaciente;

    [Header("Historia Clinica")]
    public string motivoConsulta;
    public string enfermedadActual;
    [TextArea(3,8)]
    [Header("Preguntas de interrogacion")]
    public string[] questions;
    public bool[] relevantQuestions;

    [Header("Respuestas correctas")]
    public string correctDiagnosis;
    public string correctTreatment;
    public int minimumCorrectExams;
    [Header("Diagnosticos incorrectos")]
    public string[] wrongDiagnosis;

    [Header("Relevancia de exámenes por categoría")]
    public ExamRelevance[] inspectionRelevances;
    public ExamRelevance[] listeningRelevances;
    public ExamRelevance[] touchingRelevances;
    public ExamRelevance[] mentalRelevances;
    public ExamRelevance[] nervesRelevances;
    public ExamRelevance[] sensitivityRelevances;

    [Header("Exámenes del diagnostico")]
    public string[] correctExtraExams;
    public bool[] extraExamsNecessaryForDiagnosis;
    [Header("Exámenes distractores")]
    public string[] wrongExtraExams;
    [Header("Retroalimentación")]

    [TextArea(5,10)]
    public string feedback;
    public TextAsset scriptPath;

    /// <summary>True si el examen está marcado como relevante en alguna categoría de este caso.</summary>
    public bool IsExamRelevant(string examName)
    {
        if (IsRelevantIn(inspectionRelevances, examName)) return true;
        if (IsRelevantIn(listeningRelevances, examName)) return true;
        if (IsRelevantIn(touchingRelevances, examName)) return true;
        if (IsRelevantIn(mentalRelevances, examName)) return true;
        if (IsRelevantIn(nervesRelevances, examName)) return true;
        if (IsRelevantIn(sensitivityRelevances, examName)) return true;

        // Exámenes extra: se mantiene el sistema de arrays paralelos
        if (correctExtraExams != null && extraExamsNecessaryForDiagnosis != null)
        {
            for (int i = 0; i < correctExtraExams.Length && i < extraExamsNecessaryForDiagnosis.Length; i++)
            {
                if (correctExtraExams[i] == examName)
                    return extraExamsNecessaryForDiagnosis[i];
            }
        }
        return false;
    }

    private bool IsRelevantIn(ExamRelevance[] category, string examName)
    {
        if (category == null) return false;
        foreach (ExamRelevance entry in category)
        {
            if (entry.examName == examName && entry.isRelevant)
                return true;
        }
        return false;
    }
}
