using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BotonVolverConsultorio : MonoBehaviour
{
    [SerializeField] private GameObject choiceBox;
    [SerializeField] private GameObject warningPanel;
    [SerializeField] private string sceneToLoad = "CaseSelection";

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        if (warningPanel != null)
            warningPanel.SetActive(false);
    }

    private void Update()
    {
        // Deshabilitado mientras ChoiceBox esté activo
        button.interactable = choiceBox == null || !choiceBox.activeInHierarchy;
    }

    // Botón de volver: muestra la advertencia
    public void OpenWarning()
    {
        if (warningPanel != null)
            warningPanel.SetActive(true);
    }
    public void ConfirmExit()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void CancelExit()
    {
        if (warningPanel != null)
            warningPanel.SetActive(false);
    }
}