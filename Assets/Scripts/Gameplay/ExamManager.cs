using UnityEngine;

public class ExamManager : MonoBehaviour
{
    public static ExamManager Instance { get; private set; }

    // Total de exámenes permitidos. Se compara con "<" (con 6, se pueden hacer 6 exámenes).
    [SerializeField] private int maxExamTries = 6;
    // Total de exámenes extra permitidos.
    public int maxExtraExamTries = 1;

    public int examinationTries;
    public int extraExaminationTries;
    public bool extraExamTime;
    private bool examTime;

    public int ExaminationTries => examinationTries;
    public bool ExamTime => examTime;
    public int MaxExamTries => maxExamTries;

    public Panel actionMenu;
    public Panel examinationMenu;
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

    public bool CanStartExam() => examinationTries < maxExamTries;
    public bool CanStartExtraExam() => extraExaminationTries < maxExtraExamTries;

    public void StartExam() => examTime = true;
    public void StopExam() => examTime = false;

    public void RegisterTry()
    {
        if (examTime) examinationTries++;
        examTime = false;
    }

    public void StartExtraExam() => extraExamTime = true;
    public void StopExtraExam() => extraExamTime = false;

    public void RegisterExtraTry()
    {
        if (extraExamTime) extraExaminationTries++;
        extraExamTime = false;
    }

    public void ResetTries()
    {
        examinationTries = 0;
        extraExaminationTries = 0;
        examTime = false;
        extraExamTime = false;
    }

    public void IsInterrogationOver()
    {
        var dialog = dialogSystem.GetComponent<DialogSystem>();
        if (interrogationSystem.GetComponent<InterrogationSystem>().GetInterrogationOver())
        {
            examinationMenu.Open();
        }
        else
        {
            actionMenu.Close();
            dialog.AddPhrase("{Ndname}(Necesito interrogar primero)");
            dialog.AddPhrase("{a}(¿Qué debería hacer?)");
        }
    }
}