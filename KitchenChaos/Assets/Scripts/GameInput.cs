using System;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public event EventHandler OnInteractAction;

    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Interact.performed += PlayerInteractPerformed;
    }

    private void PlayerInteractPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj) =>
        OnInteractAction?.Invoke(this, EventArgs.Empty);

    public Vector2 GetMovementVectorNormalized() =>
        playerInputActions.Player.Move.ReadValue<Vector2>().normalized;
    
}
