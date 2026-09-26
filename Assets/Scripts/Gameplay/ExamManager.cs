using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ExamManager : MonoBehaviour
{
    public static ExamManager Instance { get; private set; }
 // Chapuza historica parte 6 es 18 porque cada que pasaba un examen sumaba 3 intentos en vez de uno xd
    [SerializeField] private int maxExamTries = 18;

    public int examinationTries;
    private bool examTime;
    public GameObject examMenu;
    public int ExaminationTries => examinationTries;
    public bool ExamTime => examTime;
    public int MaxExamTries => maxExamTries;
    public GameObject interrogationSystem;
    public GameObject dialogSystem;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            
            Destroy(gameObject);
            return;
        }

        Instance = this;

    }

    public bool CanStartExam()
    {
        return examinationTries <= maxExamTries;
    }


    public void StartExam()
    {
        examTime = true;
    }


    public void RegisterTry()
    {
        if (examTime && examinationTries <= maxExamTries)
        {
            examinationTries++;
            Debug.Log("examTimeValue " + examTime);
        }
        else
        {
            examTime = false;
        }
    }

   
    public void StopExam()
    {
        examTime = false;
    }

    public void ResetTries()
    {
        examinationTries = 0;
        examTime = false;
    }
    public void IsInterrogationOver(){
        if(interrogationSystem.GetComponent<InterrogationSystem>().GetInterrogationOver() == true){
            Debug.Log("duro dos");
            examMenu.SetActive(true);
            examMenu.transform.GetChild(0).gameObject.SetActive(true);
        }else{
            Debug.Log("horas haciendolo bien rico");
            dialogSystem.GetComponent<DialogSystem>().AddPhrase("{Ndname}(Necesito interrogar primero)");
        }
    }
}