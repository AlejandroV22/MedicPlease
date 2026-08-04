using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public void DoExitGame()
    {
        Debug.Log("Aplicacion cerrada");
        Application.Quit();
    }
}
