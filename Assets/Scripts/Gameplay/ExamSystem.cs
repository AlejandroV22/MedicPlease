using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
public class ExamSystem : MonoBehaviour
{
    private GameObject choiceBox;
    private TMP_Dropdown choiceOptions;
    public string[] exams;
    private Button choiceButton;
    private GameObject dialogSystem;
    public void ShowExam(){
        choiceBox = GameObject.Find("ChoiceBox");
        choiceOptions=choiceBox.GetComponentInChildren<TMP_Dropdown>();
        choiceButton = GameObject.Find("ChoiceButton").GetComponent<Button>();
        dialogSystem = GameObject.Find("DialogBoxImage");
        choiceOptions.ClearOptions();
        choiceOptions.AddOptions(exams.ToList());
        choiceButton.onClick.RemoveAllListeners();
        choiceButton.onClick.AddListener(ExamSelected);
    }
    public void ExamSelected(){
        choiceBox.GetComponent<ChoiceSystem>().GetStart();
        //TODO agregar a clinical case los resultados de los examenes
    }
}
