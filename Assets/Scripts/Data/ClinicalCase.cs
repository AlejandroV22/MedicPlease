using System.Collections.Generic;
using UnityEngine;

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

    [Header("Respuestas correctas")]
    public string correctDiagnosis;
    public string correctTreatment;

    [Header("Retroalimentación")]

    [TextArea(5,10)]
    public string feedback;
    public TextAsset scriptPath;
    
}