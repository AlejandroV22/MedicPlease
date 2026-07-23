using TMPro;
using UnityEngine;

public class ClinicalCaseViewer : MonoBehaviour
{
    [Header("Caso clínico")]
    public ClinicalCase clinicalCase;

    [Header("UI")]

    public TMP_Text patientNameText;

    public TMP_Text ageText;

    public TMP_Text sexText;

    public TMP_Text historyText;

    public TMP_Text symptomsText;

    void Start()
    {
        ShowClinicalCase();
    }

    void ShowClinicalCase()
    {
        patientNameText.text = clinicalCase.patientName;

        ageText.text = clinicalCase.age.ToString();

        sexText.text = clinicalCase.sex;

        historyText.text = clinicalCase.medicalHistory;

        symptomsText.text = clinicalCase.symptoms;
    }
    public void LoadClinicalCase(ClinicalCase newCase)
    {
        clinicalCase = newCase;

        ShowClinicalCase();
    }
}