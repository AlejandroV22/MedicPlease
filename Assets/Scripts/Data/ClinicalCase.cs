using UnityEngine;

[CreateAssetMenu(fileName = "NuevoCasoClinico", menuName = "Clinical Cases/Caso Clínico")]
public class ClinicalCase : ScriptableObject
{
    [Header("Información del paciente")]
    public string patientName;
    public int age;
    public string sex;

    [TextArea(5,10)]
    public string medicalHistory;

    [TextArea(3,8)]
    public string symptoms;

    [Header("Respuestas correctas")]
    public string correctDiagnosis;
    public string correctTreatment;

    [Header("Retroalimentación")]

    [TextArea(5,10)]
    public string feedback;
    
}