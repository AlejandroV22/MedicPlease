using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CaseButton : MonoBehaviour
{
    public ClinicalCase clinicalCase;
    public string sceneToLoad;

    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        if (button == null) return;

        // Sin ClinicalCase asignado, el botón queda bloqueado
        Debug.Log("CaseButton " + (clinicalCase != null ? clinicalCase.name : "sin caso")
        + " | desbloqueado: " + IsUnlocked()
        + " | casos guardados: " + GameDataController.Instance.Data.caseHistory.Count);
        button.interactable = clinicalCase != null && IsUnlocked();
    }

    private bool IsUnlocked()
    {
        GameDataController dataController = FindAnyObjectByType<GameDataController>();
        if (dataController == null || clinicalCase == null) return false;

        return CaseProgress.IsCaseUnlocked(dataController.Data, clinicalCase.name);
    }

    public void SelectCase()
    {
        if (clinicalCase != null)
        {
            if (!IsUnlocked()) return;

            GameManager.Instance.SelectClinicalCase(clinicalCase);
        }

        SceneManager.LoadScene(sceneToLoad);
    }
}