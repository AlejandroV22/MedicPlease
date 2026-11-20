using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameButton : MonoBehaviour
{
    public GameObject newGamePanel;
    public GameObject warningOverlay;
    public GameObject newNameOverlay;
    public TMP_InputField nameInput;
    public string sceneToLoad = "CaseSelection";

    // Botón "Nueva partida" del menú: solo abre el panel correspondiente
    public void NewGame()
    {
        newGamePanel.SetActive(true);

        bool hasSave = GameDataController.Instance != null && GameDataController.Instance.isSaveFileExist;
        warningOverlay.SetActive(hasSave);
        newNameOverlay.SetActive(!hasSave);
    }

    // Botón "Sí" de la advertencia: solo pasa al panel del nombre, sin borrar nada todavía
    public void ConfirmOverwrite()
    {
        warningOverlay.SetActive(false);
        newNameOverlay.SetActive(true);
    }

    // Botón "Confirmar" del panel del nombre: aquí se borra el progreso y se crea la partida
    public void ConfirmNewGame()
    {
        string playerName = nameInput != null ? nameInput.text.Trim() : "";
        if (string.IsNullOrEmpty(playerName)) return;

        GameDataController.StartNewGame();          // borra el archivo anterior y crea uno nuevo
        GameDataController.Instance.SetPlayerName(playerName);

        SceneManager.LoadScene(sceneToLoad);
    }
}