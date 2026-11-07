using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputController : MonoBehaviour
{
    public DialogSystem dialogSystem;
    public InputActionReference skipText;
    void Start()
    {
        dialogSystem = GetComponent<DialogSystem>();
    }
    private void OnEnable()
    {
        skipText.action.Enable();
        skipText.action.started += SkipText;
    }
    private void OnDisable()
    {
        skipText.action.started -= SkipText;
        skipText.action.Disable();
    }
    private void SkipText(InputAction.CallbackContext obj){
        dialogSystem.SkipWriting();
    }
    // Update is called once per frame

}
