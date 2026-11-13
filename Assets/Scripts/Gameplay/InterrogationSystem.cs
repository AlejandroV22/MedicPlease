using UnityEngine;

public class InterrogationSystem : MonoBehaviour
{
    public GameObject dialogBox;
    public GameObject choiceBox;
    public GameObject ClinicalCaseButton;
    public Panel actionMenu;

    private const int MaxQuestions = 3;
    private bool interrogationTime;
    private bool interrogationOver;
    private int interrogationTries = 1;

    public bool GetInterrogationTime() => interrogationTime;
    public bool GetInterrogationOver() => interrogationOver;
    public void SetInterrogationTime(bool value) => interrogationTime = value;

    public void InterrogationChoice()
    {
        var dialog = dialogBox.GetComponent<DialogSystem>();

        if (interrogationTries > MaxQuestions)
        {
            interrogationOver = true;
            interrogationTime = false;
            dialog.AddPhrase("(El paciente no respondera mas preguntas)");
            dialog.AddPhrase("{a}¿Qué debería hacer?"); // chapuza historica parte 2
            return;
        }

        if (!ClinicalCaseButton.GetComponent<ClinicalHistoryButton>().isActiveFirstTime)
        {
            dialog.AddPhrase("(Debería revisar primero la historia del paciente)");
            dialog.AddPhrase("{a}¿Qué debería hacer?");
            return;
        }

        interrogationTime = true;

        var choice = choiceBox.GetComponent<ChoiceSystem>();
        choice.cancelable = false;              // no hay menú al que volver: no se cancela
        choiceBox.SetActive(true);
        choice.ChoiceMenu(ChoiceSystem.BuildRawChoices(dialog.clinicalCase.questions));

        dialog.AddPhrase("(¿Qué debería preguntarle al paciente?)");
    }

    public void InterrogationLoop()
    {
        if (!interrogationTime || interrogationTries > MaxQuestions)
        {
            interrogationTime = false;
            return;
        }

        interrogationTries++;
        InterrogationChoice();
        actionMenu.Close();                     // mientras se interroga, el menú de acciones se oculta
    }
}