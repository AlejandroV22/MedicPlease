using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class ExamManager : MonoBehaviour
{
    public static ExamManager Instance { get; private set; }
 // Chapuza historica parte 6 es 18 porque cada que pasaba un examen sumaba 3 intentos en vez de uno xd (esto se arreglo por arte de magia)
    [SerializeField] private int maxExamTries = 5;

    public int examinationTries;
    private bool examTime;
    public GameObject examMenu;
    public int ExaminationTries => examinationTries;
    public bool ExamTime => examTime;
    public int MaxExamTries => maxExamTries;
    public GameObject interrogationSystem;
    public GameObject dialogSystem;
    public GameObject actionMenu;
    public int maxExtraExamTries = 2;
    public bool extraExamTime;
    public int extraExaminationTries;

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
    public bool CanStartExtraExam()
    {
        return extraExaminationTries <= maxExtraExamTries;
    }
    public void StartExtraExam(){
        extraExamTime = true;
    }
       public void StopExtraExam(){
        examTime = false;
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
        if (extraExamTime && extraExaminationTries <= maxExtraExamTries)
        {
            extraExaminationTries++;
            Debug.Log("extraExamTimeValue " + extraExamTime);
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
            actionMenu.SetActive(false);
            dialogSystem.GetComponent<DialogSystem>().AddPhrase("{Ndname}(Necesito interrogar primero)");
            dialogSystem.GetComponent<DialogSystem>().AddPhrase("{a}(¿Qué debería hacer?)");
        }
    }

}