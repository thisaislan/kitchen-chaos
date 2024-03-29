using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour
{
    private const string NUMBER_POPUP_TRIGGER = "NumberPopupTrigger";

    [SerializeField] private TextMeshProUGUI countdownText;

    [SerializeField] private Animator animator;
        
    void Start() =>
        GameManager.Instance.OnStateChanged += OnGameManagerStateChanged;

    private void OnDestroy() =>
        GameManager.Instance.OnStateChanged -= OnGameManagerStateChanged;

    private void Update()
    {
        var currentCountdownNumber = Mathf.CeilToInt(GameManager.Instance.CountdownToStartTimer.Value).ToString();

        if (!countdownText.text.Equals(currentCountdownNumber.ToString()) && GameManager.Instance.State.Value == GameManager.GameState.CountdownToStart)
        {
            countdownText.text = currentCountdownNumber.ToString();
            animator.SetTrigger(NUMBER_POPUP_TRIGGER);
            SoundManager.Instance.PlayCountdownSound();
        }
    }

    private void OnGameManagerStateChanged(object sender, System.EventArgs e) =>
        countdownText.gameObject.SetActive(GameManager.Instance.State.Value == GameManager.GameState.CountdownToStart);

}
