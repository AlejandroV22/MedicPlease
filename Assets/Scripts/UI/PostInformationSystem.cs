using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PostInformationSystem : MonoBehaviour
{
    public GameObject gameDataController;
    public GameData saveData;
    public ClinicalCase clinicalCase;

    public TextMeshProUGUI interrogationDecisionsText;
    public TextMeshProUGUI examinationDecisionsText;

    private List<string> interrogationQuestions = new List<string>();
    private List<string> examinationQuestions = new List<string>();

    private void Awake()
    {
        clinicalCase = GameManager.Instance.SelectedCase;
        saveData = gameDataController.GetComponent<GameDataController>().LoadData();
        GetLastDecisions();
    }

    public void GetLastDecisions()
    {
        interrogationQuestions.Clear();
        examinationQuestions.Clear();

        if (clinicalCase == null || saveData == null) return;

        foreach (string decision in saveData.decisionsMade)
        {
            if (clinicalCase.questions.Contains(decision))
            {
                if (!interrogationQuestions.Contains(decision))
                    interrogationQuestions.Add(decision);
            }
            else
            {
                if (!examinationQuestions.Contains(decision))
                    examinationQuestions.Add(decision);
            }
        }

        interrogationDecisionsText.text = string.Join("\n", interrogationQuestions);
        examinationDecisionsText.text = string.Join("\n", examinationQuestions);
    }
}