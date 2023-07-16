using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{

    public enum Biding
    {
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        Interact,
        InteractAlternative,
        Pause
    }

    public static GameInput Instance { get; private set; }

    public event EventHandler OnInteractAction;
    public event EventHandler OnInteractAlternateAction;
    public event EventHandler OnPauseAction;

    private const string INPUT_BINDS = "input_binds";

    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        Instance = this;

        playerInputActions = new PlayerInputActions();

        if (PlayerPrefs.HasKey(INPUT_BINDS))
        {
            playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(INPUT_BINDS));
        }

        playerInputActions.Player.Enable();
    }

    private void Start()
    {
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

    public string GetBidingText(Biding biding)
    {
        switch (biding)
        {
            default:
            case Biding.MoveUp:
               return playerInputActions.Player.Move.bindings[1].ToDisplayString();
            case Biding.MoveDown:
                return playerInputActions.Player.Move.bindings[2].ToDisplayString();
            case Biding.MoveLeft:
                return playerInputActions.Player.Move.bindings[3].ToDisplayString();
            case Biding.MoveRight:
                return playerInputActions.Player.Move.bindings[4].ToDisplayString();
            case Biding.Interact:
                return playerInputActions.Player.Interact.bindings[0].ToDisplayString();
            case Biding.InteractAlternative:
                return playerInputActions.Player.InteractAlternate.bindings[0].ToDisplayString();
            case Biding.Pause:
                return playerInputActions.Player.Pause.bindings[0].ToDisplayString();
        }
    }

    public void RebindingBiding(Biding biding, Action onRebingindAction)
    {
        InputAction inputAction;
        int bidingIndex;

        switch (biding)
        {
            default:
            case Biding.MoveUp:
                inputAction = playerInputActions.Player.Move;
                bidingIndex = 1;
        break;
            case Biding.MoveDown:
                inputAction = playerInputActions.Player.Move;
                bidingIndex = 2;
        break;
            case Biding.MoveLeft:
                inputAction = playerInputActions.Player.Move;
                bidingIndex = 3;
        break;
            case Biding.MoveRight:
                inputAction = playerInputActions.Player.Move;
                bidingIndex = 4;
        break;
            case Biding.Interact:
                inputAction = playerInputActions.Player.Interact;
                bidingIndex = 0;
        break;
            case Biding.InteractAlternative:
                inputAction = playerInputActions.Player.InteractAlternate;
                bidingIndex = 0;
                break;
            case Biding.Pause:
                inputAction = playerInputActions.Player.Pause;
                bidingIndex = 0;
                break;
        }

        playerInputActions.Player.Disable();

        inputAction.PerformInteractiveRebinding(bidingIndex)
            .OnComplete(callback =>
            {
                callback.Dispose();
                playerInputActions.Player.Enable();
                onRebingindAction();

                PlayerPrefs.SetString(INPUT_BINDS, playerInputActions.SaveBindingOverridesAsJson());
                PlayerPrefs.Save();
            }).Start();
    }

}
