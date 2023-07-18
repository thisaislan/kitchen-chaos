using TMPro;
using UnityEngine;

public class TutotialUI : MonoBehaviour
{

    [SerializeField] GameObject tutorialConteiner;

    [SerializeField] private TextMeshProUGUI upKeyText;
    [SerializeField] private TextMeshProUGUI downKeyText;
    [SerializeField] private TextMeshProUGUI leftKeyText;
    [SerializeField] private TextMeshProUGUI rightKeyText;
    [SerializeField] private TextMeshProUGUI interactKeyText;
    [SerializeField] private TextMeshProUGUI interactAlternativeKeyText;
    [SerializeField] private TextMeshProUGUI pauseKeyText;
    [SerializeField] private TextMeshProUGUI interactGamepadKeyText;
    [SerializeField] private TextMeshProUGUI interactAlternativeGamepadKeyText;
    [SerializeField] private TextMeshProUGUI pauseGamepadKeyText;


    private void Start()
    {
        GameInput.Instance.OnRebinding += OnGameInputRebinding;
        GameManager.Instance.OnStateChanged += OnGameManagerStateChanged;

        tutorialConteiner.SetActive(true);
        UpdateVisuals();
    }


    private void OnDestroy()
    {
        GameInput.Instance.OnRebinding -= OnGameInputRebinding;
        GameManager.Instance.OnStateChanged -= OnGameManagerStateChanged;
    }

    private void OnGameInputRebinding(object sender, System.EventArgs e) =>
        UpdateVisuals();

    private void OnGameManagerStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.State == GameManager.GameState.CountdownToStart)
        {
            tutorialConteiner.SetActive(false);
        }
    }


    private void UpdateVisuals()
    {
        upKeyText.text = GameInput.Instance.GetBidingText(GameInput.Biding.MoveUp);
        downKeyText.text = GameInput.Instance.GetBidingText(GameInput.Biding.MoveDown);
        leftKeyText.text = GameInput.Instance.GetBidingText(GameInput.Biding.MoveLeft);
        rightKeyText.text = GameInput.Instance.GetBidingText(GameInput.Biding.MoveRight);
        interactKeyText.text = GameInput.Instance.GetBidingText(GameInput.Biding.Interact);
        interactAlternativeKeyText.text = GameInput.Instance.GetBidingText(GameInput.Biding.InteractAlternative);
        pauseKeyText.text = GameInput.Instance.GetBidingText(GameInput.Biding.Pause);
        interactGamepadKeyText.text = GameInput.Instance.GetBidingText(GameInput.Biding.GamepadInteract);
        interactAlternativeGamepadKeyText.text = GameInput.Instance.GetBidingText(GameInput.Biding.GamepadInteractAlternative);
        pauseGamepadKeyText.text = GameInput.Instance.GetBidingText(GameInput.Biding.GamepadPause);
    }

}
