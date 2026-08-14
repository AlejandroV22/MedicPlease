using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    public Image patientPhoto;
    

  private void Start()
    {
        LoadClinicalCase(GameManager.Instance.SelectedCase);
    }

    void ShowClinicalCase()
    {
        patientNameText.text = clinicalCase.patientName;

        ageText.text = clinicalCase.age.ToString();

        sexText.text = clinicalCase.sex;

        historyText.text = clinicalCase.consultReason;

        symptomsText.text = clinicalCase.symptoms;
        
        patientPhoto.sprite = clinicalCase.patientPortrait;
    }
    public void LoadClinicalCase(ClinicalCase newCase)
    {
        clinicalCase = newCase;

        ShowClinicalCase();
    }
}