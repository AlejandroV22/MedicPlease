using System.Collections.Generic;
using UnityEngine;

public enum ExamCategory
{
    None,
    Inspection,
    Listening,
    Touching,
    Mental,
    Nerves,
    Sensitivity,
    Extra
}

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

    /// <summary>
    /// True si el examen es relevante en la categoría indicada.
    /// Con ExamCategory.None (decisiones que no vienen de un botón de examen) busca en todas.
    /// </summary>
    public bool IsExamRelevant(ExamCategory category, string examName)
    {
        switch (category)
        {
            case ExamCategory.Inspection:   return IsRelevantIn(inspectionRelevances, examName);
            case ExamCategory.Listening:    return IsRelevantIn(listeningRelevances, examName);
            case ExamCategory.Touching:     return IsRelevantIn(touchingRelevances, examName);
            case ExamCategory.Mental:       return IsRelevantIn(mentalRelevances, examName);
            case ExamCategory.Nerves:       return IsRelevantIn(nervesRelevances, examName);
            case ExamCategory.Sensitivity:  return IsRelevantIn(sensitivityRelevances, examName);
            case ExamCategory.Extra:        return IsExtraExamRelevant(examName);
        }

        return IsRelevantIn(inspectionRelevances, examName)
            || IsRelevantIn(listeningRelevances, examName)
            || IsRelevantIn(touchingRelevances, examName)
            || IsRelevantIn(mentalRelevances, examName)
            || IsRelevantIn(nervesRelevances, examName)
            || IsRelevantIn(sensitivityRelevances, examName)
            || IsExtraExamRelevant(examName);
    }

    /// <summary>Nombres de los examenes de una categoria, en el orden en que estan en su lista.</summary>
    public string[] GetExamNames(ExamCategory category)
    {
        ExamRelevance[] list = null;
        switch (category)
        {
            case ExamCategory.Inspection:   list = inspectionRelevances; break;
            case ExamCategory.Listening:    list = listeningRelevances; break;
            case ExamCategory.Touching:     list = touchingRelevances; break;
            case ExamCategory.Mental:       list = mentalRelevances; break;
            case ExamCategory.Nerves:       list = nervesRelevances; break;
            case ExamCategory.Sensitivity:  list = sensitivityRelevances; break;
        }

        List<string> names = new List<string>();
        if (list != null)
        {
            foreach (ExamRelevance entry in list)
            {
                if (!string.IsNullOrEmpty(entry.examName))
                    names.Add(entry.examName);
            }
        }
        return names.ToArray();
    }

    private bool IsExtraExamRelevant(string examName)
    {
        // Exámenes extra: se mantiene el sistema de arrays paralelos
        if (correctExtraExams == null || extraExamsNecessaryForDiagnosis == null) return false;
        for (int i = 0; i < correctExtraExams.Length && i < extraExamsNecessaryForDiagnosis.Length; i++)
        {
            if (correctExtraExams[i] == examName)
                return extraExamsNecessaryForDiagnosis[i];
        }
        return false;
    }

    private bool IsRelevantIn(ExamRelevance[] list, string examName)
    {
        if (list == null) return false;
        foreach (ExamRelevance entry in list)
        {
            if (entry.examName == examName && entry.isRelevant)
                return true;
        }
        return false;
    }
}
