using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using System;

public class ExamSystem : MonoBehaviour
{
    public GameObject choiceBox;
    private TMP_Dropdown choiceOptions;
    public string[] exams;
    public bool[] revelantExams;
    public GameObject dialogSystem;

    public GameObject actionMenu;

    public void ShowExam()
    {
        if (ExamManager.Instance.CanStartExam())
        {
            ExamManager.Instance.StartExam();

            string rawChoices = "{choice}{choicest2 c:";
            foreach (string exam in exams)
            {
                rawChoices = rawChoices + "\"" + exam + "\"" + " ";
            }
            rawChoices = rawChoices + "} ";

            choiceBox.SetActive(true);
            choiceBox.GetComponent<ChoiceSystem>().ChoiceMenu(rawChoices);
        }
        else
        {
            ExamManager.Instance.StopExam();
            dialogSystem.GetComponent<DialogSystem>().AddPhrase("(No puedo realizarle mas examenes al paciente)");
            //chapuza historica parte 3
            dialogSystem.GetComponent<DialogSystem>().AddPhrase("{nLine}");
        }
    }

    public void ExamLoop()
    {
        if (ExamManager.Instance.ExamTime && ExamManager.Instance.CanStartExam())
        {
            ExamManager.Instance.RegisterTry();
            actionMenu.SetActive(true);
        }
        else
        {
            ExamManager.Instance.StopExam();
        }
    }
    public void IsExaminationOver(string examinationType){
        if (ExamManager.Instance.CanStartExam()){
            dialogSystem.GetComponent<DialogSystem>().AddPhrase("¿Qué debería "+ examinationType +"?");
            ShowExam();
        }else{
            ShowExam();
        }
    
    }
    public void ExtraExamLoop()
    {
        if (ExamManager.Instance.extraExamTime && ExamManager.Instance.CanStartExtraExam())
        {
            ExamManager.Instance.RegisterTry();
            actionMenu.SetActive(true);
        }
        else
        {
            ExamManager.Instance.StopExtraExam();
        }
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

            string rawChoices = "{choice}{choicest2 c:";
            foreach (string exam in exams)
            {
                rawChoices = rawChoices + "\"" + exam + "\"" + " ";
            }
            rawChoices = rawChoices + "} ";

            choiceBox.SetActive(true);
            choiceBox.GetComponent<ChoiceSystem>().ChoiceMenu(rawChoices);
        }
        else
        {
            ExamManager.Instance.StopExtraExam();
            dialogSystem.GetComponent<DialogSystem>().AddPhrase("(creo que ya tengo todo lo que necesito)");
            //chapuza historica parte 3
            dialogSystem.GetComponent<DialogSystem>().AddPhrase("{nLine}");
        }
    }
    public void ExamSelected()
    {
        choiceBox.GetComponent<ChoiceSystem>().GetStart();
        //TODO agregar a clinical case los resultados de los examenes.
    }
}