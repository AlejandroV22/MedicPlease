using UnityEngine;
using UnityEngine.SceneManagement;

public class CaseButton : MonoBehaviour
{
    public ClinicalCase clinicalCase;
    public string sceneToLoad; 

    public void SelectCase()
    {
        if(!clinicalCase == false)
        {
            GameManager.Instance.SelectClinicalCase(clinicalCase);
        }
        SceneManager.LoadScene(sceneToLoad);
    }
}