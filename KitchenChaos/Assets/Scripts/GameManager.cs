using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static Player;

public class GameManager : NetworkBehaviour
{

    public static GameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;
    public event EventHandler OnLocalGamePaused;
    public event EventHandler OnLocalGameUnpaused;
    public event EventHandler OnGamePause;
    public event EventHandler OnGameUnpause;
    public event EventHandler OnLocalPlayerReadyChanged;

    public enum GameState
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver
    }

    public NetworkVariable<GameState> State { get; private set; } = new NetworkVariable<GameState>(GameState.WaitingToStart);

    public NetworkVariable<float> CountdownToStartTimer { get; private set; } = new NetworkVariable<float>(3f);

    public NetworkVariable<bool> IsGamePaused { get; private set; } = new NetworkVariable<bool>(false);

    private NetworkVariable<float> gamePalyingTimer = new NetworkVariable<float>(0f);

    private Dictionary<ulong, bool> playerReadyDictionary = new Dictionary<ulong, bool>();
    private Dictionary<ulong, bool> playerPauseDictionary = new Dictionary<ulong, bool>();

    public bool IsLocalPlayerReady { get; private set; }

    public bool IsLocalGamePaused { get; private set; }

    private float gamePalyingTimerMax = 90f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += OnGameInputPauseAction;
        GameInput.Instance.OnInteractAction += OnGameInputInteractAction;
    }

    public override void OnNetworkSpawn()
    {
        State.OnValueChanged += OnStateChange;
        IsGamePaused.OnValueChanged += OnGamePauseChange;
        base.OnNetworkSpawn();
    }

    public override void OnDestroy()
    {
        State.OnValueChanged -= OnStateChange;
        IsGamePaused.OnValueChanged -= OnGamePauseChange;

        GameInput.Instance.OnPauseAction -= OnGameInputPauseAction;
        GameInput.Instance.OnInteractAction -= OnGameInputInteractAction;

        base.OnDestroy();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                return;
            }
        }

        gamePalyingTimer.Value = gamePalyingTimerMax;
        State.Value = GameState.CountdownToStart;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ToglePauseGameServerRpc(bool toggle, ServerRpcParams serverRpcParams = default)
    {
        playerPauseDictionary[serverRpcParams.Receive.SenderClientId] = toggle;

        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (playerPauseDictionary.ContainsKey(clientId) && playerPauseDictionary[clientId])
            {
                IsGamePaused.Value = true;
                return;
            }
        }

        IsGamePaused.Value = false;
    }

    private void OnStateChange(GameState previousValue, GameState newValue) =>
        OnStateChanged?.Invoke(this, EventArgs.Empty);

    private void OnGamePauseChange(bool previousValue, bool newValue)
    {
        Time.timeScale = newValue ? 0 : 1;

        if (newValue)
        {
            OnGamePause?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            OnGameUnpause?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnGameInputPauseAction(object sender, EventArgs e) =>
        PauseGame();

    private void OnGameInputInteractAction(object sender, EventArgs e)
    {
        if (IsLocalPlayerReady == false)
        {
            IsLocalPlayerReady = true;
            OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
            SetPlayerReadyServerRpc();
        }
    }

    private void Update()
    {
        if (!IsServer) { return; }

        switch (State.Value)
        {
            case GameState.WaitingToStart:
                break;

            case GameState.CountdownToStart:

                CountdownToStartTimer.Value -= Time.deltaTime;

                if (CountdownToStartTimer.Value < 0f)
                {
                    State.Value = GameState.GamePlaying;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }

                break;

            case GameState.GamePlaying:

                gamePalyingTimer.Value -= Time.deltaTime;

                if (gamePalyingTimer.Value < 0f)
                {
                    State.Value = GameState.GameOver;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }

                break;

            case GameState.GameOver:
                break;
        }
    }

    public float GetGamePlayTimeNormalized() =>
        1 - (gamePalyingTimer.Value / gamePalyingTimerMax);

    public void UnpauseGame()
    {
        IsLocalGamePaused = false;

        OnLocalGameUnpaused?.Invoke(this, EventArgs.Empty);

        ToglePauseGameServerRpc(false);
    }

    public void PauseGame()
    {
        IsLocalGamePaused = true;

        OnLocalGamePaused?.Invoke(this, EventArgs.Empty);

        ToglePauseGameServerRpc(true);
    }

}
