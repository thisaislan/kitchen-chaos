using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI countdownText;
        
    void Start() =>
        GameManager.Instance.OnStateChanged += OnGameManagerStateChanged;

    private void OnDestroy() =>
        GameManager.Instance.OnStateChanged -= OnGameManagerStateChanged;

    private void Update() =>
        countdownText.text = Mathf.Ceil(GameManager.Instance.CountdownToStartTimer).ToString();

    private void OnGameManagerStateChanged(object sender, System.EventArgs e) =>
        countdownText.gameObject.SetActive(GameManager.Instance.State == GameManager.GameState.CountdownToStart);

}
