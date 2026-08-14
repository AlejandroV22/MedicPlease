using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NuevoCasoClinico", menuName = "Clinical Cases/Caso Clínico")]
public class ClinicalCase : ScriptableObject
{
    [Header("Información del paciente")]
    public string patientName;
    public int age;
    public string sex;
    public Sprite patientPortrait;

    [Header("Historia Clinica")]
    public string consultReason;
    public string personalHistory;
    public string familyHistory;
    [TextArea(3,8)]
    public string symptoms;

    [Header("Respuestas correctas")]
    public string correctDiagnosis;
    public string correctTreatment;

    [Header("Retroalimentación")]

    [TextArea(5,10)]
    public string feedback;
    public TextAsset scriptPath;
    
}