using UnityEngine;


public class ExamManager : MonoBehaviour
{
    public static ExamManager Instance { get; private set; }
 // Chapuza historica parte 6 es 18 porque cada que pasaba un examen sumaba 3 intentos en vez de uno xd
    [SerializeField] private int maxExamTries = 18;

    public int examinationTries;
    private bool examTime;

    public int ExaminationTries => examinationTries;
    public bool ExamTime => examTime;
    public int MaxExamTries => maxExamTries;

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
}