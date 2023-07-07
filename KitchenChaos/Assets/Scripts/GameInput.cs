using System;
using UnityEngine;

public class GameInput : MonoBehaviour
{

    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;

    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Interact.performed += OnPlayerInteractPerformed;
        playerInputActions.Player.InteractAlternate.performed += OnPlayerInteractAlternatePerformed;
    }

    private void OnDestroy()
    {
        playerInputActions.Player.Interact.started -= OnPlayerInteractPerformed;
        playerInputActions.Player.InteractAlternate.performed -= OnPlayerInteractAlternatePerformed;
    }

    private void OnPlayerInteractPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj) =>
        OnInteractAction?.Invoke(this, EventArgs.Empty);

    private void OnPlayerInteractAlternatePerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj) =>
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);

    public Vector2 GetMovementVectorNormalized() =>
        playerInputActions.Player.Move.ReadValue<Vector2>().normalized;

}
