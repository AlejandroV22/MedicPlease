using UnityEngine;
using UnityEngine.SceneManagement;

public class VolverBoton : MonoBehaviour
{
    [Tooltip("Nombre exacto de la escena a cargar. Debe estar en Build Settings.")]
    [SerializeField] private string sceneName;

    public void LoadTargetScene()
    {
        SceneManager.LoadScene(sceneName);
    }

    
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}