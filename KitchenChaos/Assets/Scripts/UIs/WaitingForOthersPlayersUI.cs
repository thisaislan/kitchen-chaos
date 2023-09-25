using UnityEngine;
using static GameManager;

public class WaitingForOthersPlayersUI : MonoBehaviour
{

    [SerializeField] GameObject waitingForOthersPlayersUIConteiner;
        
    void Start()
    {
        GameManager.Instance.OnLocalPlayerReadyChanged += OnGameManagerLocalPlayerReadyChanged;
        GameManager.Instance.OnStateChanged += OnGameManagerStateChanged;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnStateChanged -= OnGameManagerStateChanged;
        GameManager.Instance.OnLocalPlayerReadyChanged -= OnGameManagerLocalPlayerReadyChanged;
    }

    private void OnGameManagerLocalPlayerReadyChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsLocalPlayerReady && GameManager.Instance.State.Value == GameManager.GameState.WaitingToStart)
        {
            waitingForOthersPlayersUIConteiner.SetActive(true);
        }
    }

    private void OnGameManagerStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.State.Value == GameManager.GameState.CountdownToStart)
        {
            waitingForOthersPlayersUIConteiner.SetActive(false);
        }
    }
}
