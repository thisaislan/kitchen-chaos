using System;
using TMPro;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{

    public static OptionsUI Instance { get; private set; }

    [SerializeField] GameObject optionsConteiner;

    [SerializeField] private Button soundEffectsMenuButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button closeButton;

    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button interactButton;
    [SerializeField] private Button interactAlternativeButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button gamepadInteractButton;
    [SerializeField] private Button gamepadInteractAlternativeButton;
    [SerializeField] private Button gamepadPauseButton;

    [SerializeField] private TextMeshProUGUI moveUpText;
    [SerializeField] private TextMeshProUGUI moveDownText;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI interactAlternativeText;
    [SerializeField] private TextMeshProUGUI pauseText;
    [SerializeField] private TextMeshProUGUI gamepadInteractText;
    [SerializeField] private TextMeshProUGUI gamepadInteractAlternativeText;
    [SerializeField] private TextMeshProUGUI gamepadPauseText;

    [SerializeField] private TextMeshProUGUI soundEffectsMenuButtonText;
    [SerializeField] private TextMeshProUGUI musicButtonText;

    [SerializeField] private GameObject pressToRebindContainer;

    private Action onCloseAction;

    private void Awake()
    {
        Instance = this;

        soundEffectsMenuButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.ChangeVolume();
            UpdateVisuals();
        });

        musicButton.onClick.AddListener(() =>
        {
            MusicManager.Instance.ChangeVolume();
            UpdateVisuals();
        });

        closeButton.onClick.AddListener(() => {
            Hide();
            onCloseAction.Invoke();
        });
          
        moveUpButton.onClick.AddListener(() => RebindingBiding(GameInput.Biding.MoveUp));

        moveDownButton.onClick.AddListener(() => RebindingBiding(GameInput.Biding.MoveDown));

        moveLeftButton.onClick.AddListener(() => RebindingBiding(GameInput.Biding.MoveLeft));

        moveRightButton.onClick.AddListener(() => RebindingBiding(GameInput.Biding.MoveRight));

        interactButton.onClick.AddListener(() => RebindingBiding(GameInput.Biding.Interact));

        interactAlternativeButton.onClick.AddListener(() => RebindingBiding(GameInput.Biding.InteractAlternative));

        pauseButton.onClick.AddListener(() => RebindingBiding(GameInput.Biding.Pause));

        gamepadInteractButton.onClick.AddListener(() => RebindingBiding(GameInput.Biding.GamepadInteract));

        gamepadInteractAlternativeButton.onClick.AddListener(() => RebindingBiding(GameInput.Biding.GamepadInteractAlternative));

        gamepadPauseButton.onClick.AddListener(() => RebindingBiding(GameInput.Biding.GamepadPause));
    }

    private void Start()
    {
        UpdateVisuals();

        GameManager.Instance.OnTogglePause += OnGameManagerTogglePause;
    }

    private void RebindingBiding(GameInput.Biding biding)
    {
        ShowPressToRebind();
        GameInput.Instance.RebindingBiding(biding, () => {
            UpdateVisuals();
            HidePressToRebind();
        });
    }

    private void OnGameManagerTogglePause(object sender, System.EventArgs e)
    {
        if (!GameManager.Instance.IsGamePaused) { Hide(); }
    }

    private void UpdateVisuals()
    {
        soundEffectsMenuButtonText.text = $"Sound Effects: {Mathf.Round(SoundManager.Instance.volume * 10f)}";
        musicButtonText.text = $"Music: {Mathf.Round(MusicManager.Instance.volume * 10f)}";

        moveUpText.text = GameInput.Instance.GetBidingText(GameInput.Biding.MoveUp);
        moveDownText.text = GameInput.Instance.GetBidingText(GameInput.Biding.MoveDown);
        moveLeftText.text = GameInput.Instance.GetBidingText(GameInput.Biding.MoveLeft);
        moveRightText.text = GameInput.Instance.GetBidingText(GameInput.Biding.MoveRight);
        interactText.text = GameInput.Instance.GetBidingText(GameInput.Biding.Interact);
        interactAlternativeText.text = GameInput.Instance.GetBidingText(GameInput.Biding.InteractAlternative);
        pauseText.text = GameInput.Instance.GetBidingText(GameInput.Biding.Pause);
        gamepadInteractText.text = GameInput.Instance.GetBidingText(GameInput.Biding.GamepadInteract);
        gamepadInteractAlternativeText.text = GameInput.Instance.GetBidingText(GameInput.Biding.GamepadInteractAlternative);
        gamepadPauseText.text = GameInput.Instance.GetBidingText(GameInput.Biding.GamepadPause);

    }

    public void Show(Action onCloseAction)
    {
        this.onCloseAction = onCloseAction;

        optionsConteiner.SetActive(true);

        soundEffectsMenuButton.Select();
    }

    public void Hide() =>
        optionsConteiner.SetActive(false);

    public void ShowPressToRebind()
    {
        pressToRebindContainer.SetActive(true);

        moveUpButton.Select();
    }

    public void HidePressToRebind() =>
        pressToRebindContainer.SetActive(false);

}
