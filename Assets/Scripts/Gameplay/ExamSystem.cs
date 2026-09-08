using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
public class ExamSystem : MonoBehaviour
{
    public GameObject choiceBox;
    private TMP_Dropdown choiceOptions;
    public string[] exams;
    public GameObject dialogSystem;
    public void ShowExam(){
        string rawChoices = "{choice}{choicest2 c:";
            foreach(string exam in exams)
            {
                rawChoices = @rawChoices + "\"" + exam + "\"" + " "; 
            }
            rawChoices = rawChoices + "} ";
            
            choiceBox.SetActive(true);
            choiceBox.GetComponent<ChoiceSystem>().ChoiceMenu(rawChoices);
                       
    }
    public void ExamSelected(){
        choiceBox.GetComponent<ChoiceSystem>().GetStart();
        //TODO agregar a clinical case los resultados de los examenes.
    }
}
