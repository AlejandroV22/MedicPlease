using UnityEngine;
using UnityEngine.SceneManagement;

public class CaseButton : MonoBehaviour
{
    public ClinicalCase clinicalCase;

    public void SelectCase()
    {
        GameManager.Instance.SelectedCase = clinicalCase;

        SceneManager.LoadScene("Consultorio");
    }
}