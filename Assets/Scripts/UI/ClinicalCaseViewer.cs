using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClinicalCaseViewer : MonoBehaviour
{
    [Header("Caso clínico")]
    public ClinicalCase clinicalCase;

    [Header("UI")]
    public TMP_Text nombrePacienteTexto;
    public TMP_Text edadTexto;
    public TMP_Text sexoTexto;
    public TMP_Text lugarProcedenciaTexto;
    public TMP_Text motivoConsultaTexto;
    public TMP_Text enfermedadActualTexto;
    public Image pacienteFoto;

    private void Start()
    {
        LoadClinicalCase(GameManager.Instance.SelectedCase);
    }

    private void ShowClinicalCase()
    {
        nombrePacienteTexto.text = "Nombre: " + clinicalCase.nombrePaciente;

        edadTexto.text = "Edad: " + clinicalCase.edad.ToString();

        sexoTexto.text = "Sexo: " + clinicalCase.sexo;

        motivoConsultaTexto.text = clinicalCase.motivoConsulta;
        
        lugarProcedenciaTexto.text = "Lugar de procedencia: " + clinicalCase.lugarProcedencia;

        enfermedadActualTexto.text = clinicalCase.enfermedadActual;

        pacienteFoto.sprite = clinicalCase.retratoPaciente;
    }

    public void LoadClinicalCase(ClinicalCase newCase)
    {
        clinicalCase = newCase;

        ShowClinicalCase();
    }
}