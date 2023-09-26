using System;
using UnityEngine;

public class PauseMultplayerUI : MonoBehaviour
{
    [SerializeField] private GameObject pauseMultplayerContainer;

    void Start()
    {
        GameManager.Instance.OnGamePause += OnGameManagerGamePause;
        GameManager.Instance.OnGameUnpause += OnGameManagerGameUnpause;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGamePause -= OnGameManagerGamePause;
        GameManager.Instance.OnGameUnpause -= OnGameManagerGameUnpause;
    }

    private void OnGameManagerGamePause(object sender, EventArgs e) =>
        pauseMultplayerContainer.SetActive(true);

    private void OnGameManagerGameUnpause(object sender, EventArgs e) =>
        pauseMultplayerContainer.SetActive(false);
    
}
