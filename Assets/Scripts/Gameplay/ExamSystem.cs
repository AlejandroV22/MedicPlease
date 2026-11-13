using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class ExamSystem : MonoBehaviour
{
    public GameObject choiceBox;
    private TMP_Dropdown choiceOptions;
    public string[] exams;
    public ExamCategory category;       // tuyo
    public GameObject dialogSystem;
    public GameObject diagnosisController;
    public GameObject actionMenu;
    public Panel extraMenu;             // remota

    public void ShowExam()
    {
        if (ExamManager.Instance.CanStartExam())
        {
            ExamManager.Instance.StartExam();

            // Los examenes de este boton salen de la lista de su categoria en el ClinicalCase.
            // Si la categoria es None o la lista esta vacia, se usan los exams del inspector.
            if (category != ExamCategory.None)
            {
                string[] namesFromCase = GameManager.Instance.SelectedCase.GetExamNames(category);
                if (namesFromCase.Length > 0)
                {
                    exams = namesFromCase;
                }
            }

            string rawChoices = "{choice}{choicest2 c:";
            foreach (string exam in exams)
            {
                rawChoices = rawChoices + "\"" + exam + "\"" + " ";
            }
            rawChoices = rawChoices + "} ";

            choiceBox.SetActive(true);
            choiceBox.GetComponent<ChoiceSystem>().currentCategory = category;
            choiceBox.GetComponent<ChoiceSystem>().ChoiceMenu(rawChoices);
        }
    }

    public void IsExaminationOver(string examinationType)
    {
        if (ExamManager.Instance.CanStartExam())
        {
            dialogSystem.GetComponent<DialogSystem>().AddPhrase("¿Qué debería " + examinationType + "?");
        }
        ShowExam();
    }

    public bool ExamLoop()
    {
        var mgr = ExamManager.Instance;
        if (!mgr.ExamTime) return false;       // la respuesta no era de un examen (interrogatorio, lab...)

        mgr.RegisterTry();
        if (!mgr.CanStartExam()) return true;  // era el último examen permitido

        actionMenu.SetActive(true);//examinationMenu.Open() Luego
        return false;
    }

    public bool ExtraExamLoop()
    {
        var mgr = ExamManager.Instance;
        if (!mgr.extraExamTime) return false;

        mgr.RegisterExtraTry();               // ya apaga extraExamTime
        if (!mgr.CanStartExtraExam()) return true;

        extraMenu.Open();                     // antes: actionMenu.SetActive(true)
        return false;
    }

    public void IsExtraExaminationOver(string examinationType){
        if (ExamManager.Instance.CanStartExtraExam()){
            dialogSystem.GetComponent<DialogSystem>().AddPhrase("¿Qué debería "+ examinationType +"?");
            ShowExtraExam();
        }else{
            ShowExtraExam();
        }

    }

    public void ShowExtraExam()
    {
        if (ExamManager.Instance.CanStartExtraExam())
        {
            ExamManager.Instance.StartExtraExam();
            ClinicalCase clinicalCase = GameManager.Instance.SelectedCase;
            diagnosisController.GetComponent<DiagnosisSystem>().GetRelevantDecisions();
            if(diagnosisController.GetComponent<DiagnosisSystem>().relevantDecisionMade.Count() >= clinicalCase.minimumCorrectExams){
                exams = clinicalCase.correctExtraExams;
                diagnosisController.GetComponent<DiagnosisSystem>().totalRelevantBeforeExtraExams = diagnosisController.GetComponent<DiagnosisSystem>().relevantDecisionMade.Count();
            }else{
                exams = clinicalCase.wrongExtraExams;
            }

            string rawChoices = "{choice}{choicest2 c:";
            foreach (string exam in exams)
            {
                rawChoices = rawChoices + "\"" + exam + "\"" + " ";
            }
            rawChoices = rawChoices + "} ";

            choiceBox.SetActive(true);
            choiceBox.GetComponent<ChoiceSystem>().currentCategory = ExamCategory.Extra;
            choiceBox.GetComponent<ChoiceSystem>().ChoiceMenu(rawChoices);
        }
    }
    //Que hace ExamSelected? Por qué llama a GetStart si GetStart lo único que hacía era ocultar la caja de diálogo lo cual en la lógica actual es un problema? 
    public void ExamSelected()
    {
        choiceBox.GetComponent<ChoiceSystem>().GetStart();
        //TODO agregar a clinical case los resultados de los examenes.
    }
}
