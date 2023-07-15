using System;
using UnityEngine;

public class GameInput : MonoBehaviour
{

    public static GameInput Instance { get; private set; }

    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    public event EventHandler OnPauseAction;

    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Interact.performed += OnPlayerInteractPerformed;
        playerInputActions.Player.InteractAlternate.performed += OnPlayerInteractAlternatePerformed;
        playerInputActions.Player.Pause.performed += OnPlayerPause;
    }

    private void OnDestroy()
    {
        playerInputActions.Player.Interact.started -= OnPlayerInteractPerformed;
        playerInputActions.Player.InteractAlternate.performed -= OnPlayerInteractAlternatePerformed;
        playerInputActions.Player.Pause.performed -= OnPlayerPause;

        playerInputActions.Dispose();
    }

    private void OnPlayerPause(UnityEngine.InputSystem.InputAction.CallbackContext obj) =>
        OnPauseAction?.Invoke(this, EventArgs.Empty);


    private void OnPlayerInteractPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj) =>
        OnInteractAction?.Invoke(this, EventArgs.Empty);

    private void OnPlayerInteractAlternatePerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj) =>
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);

    public Vector2 GetMovementVectorNormalized() =>
        playerInputActions.Player.Move.ReadValue<Vector2>().normalized;

}
