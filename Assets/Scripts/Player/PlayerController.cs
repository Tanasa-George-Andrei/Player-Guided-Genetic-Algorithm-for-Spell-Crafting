using UnityEngine.InputSystem;
using UnityEngine;

public struct InputWrapper
{
    public Vector2 inputMovement;
    public Vector2 mouseDelta;
    public bool sprinting;
    public bool jumping;
    public bool jumped;
    public bool action1Start;
    public bool action2Start;
    public bool action3Start;
    public bool action4Start;
    public bool specialAction;
    public bool spellCreationStart;
}


[RequireComponent(typeof(PlayerDirector))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    private PlayerDirector director;
    private InputWrapper inputWrapper;

    public void OnMovement(InputAction.CallbackContext value)
    {
        inputWrapper.inputMovement = value.ReadValue<Vector2>();
        director.ProcessInput(inputWrapper);
    }
    public void OnLook(InputAction.CallbackContext value)
    {
        inputWrapper.mouseDelta = value.ReadValue<Vector2>();
        director.ProcessInput(inputWrapper);
    }
    public void OnJump(InputAction.CallbackContext value)
    {
        inputWrapper.jumping = value.performed;
        inputWrapper.jumped = value.started;
        director.ProcessInput(inputWrapper);
    }
    public void OnSprint(InputAction.CallbackContext value)
    {
        inputWrapper.sprinting = value.performed;
        director.ProcessInput(inputWrapper);
    }
    public void OnSpecialAction(InputAction.CallbackContext value)
    {
        inputWrapper.specialAction = value.started || value.performed;
        director.ProcessInput(inputWrapper);
    }
    public void OnAction1(InputAction.CallbackContext value)
    {
        inputWrapper.action1Start = value.started /*|| value.performed*/;
        director.ProcessInput(inputWrapper);
    }
    public void OnAction2(InputAction.CallbackContext value)
    {
        inputWrapper.action2Start = value.started /*|| value.performed*/;
        director.ProcessInput(inputWrapper);
    }
    public void OnAction3(InputAction.CallbackContext value)
    {
        inputWrapper.action3Start = value.started /*|| value.performed*/;
        director.ProcessInput(inputWrapper);
    }
    public void OnAction4(InputAction.CallbackContext value)
    {
        inputWrapper.action4Start = value.started /*|| value.performed*/;
        director.ProcessInput(inputWrapper);
    }
    public void StartSpellCreation(InputAction.CallbackContext value)
    {
        inputWrapper.spellCreationStart = value.started;
        director.ProcessInput(inputWrapper);
    }

    public void OnPause(InputAction.CallbackContext value)
    {
        PlayerUIManager.Instance.Pause();
    }

    private void Start()
    {
        director = gameObject.GetComponent<PlayerDirector>();
    }
}