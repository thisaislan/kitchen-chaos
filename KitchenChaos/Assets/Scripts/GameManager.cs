using System;
using UnityEngine;
using static Player;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;
    public event EventHandler OnTogglePause;

    public enum GameState
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver
    }

    public GameState State { get; private set; } = GameState.WaitingToStart;

    // TODO - To start game automatically
    public float CountdownToStartTimer { get; private set; }  = 1f;

    public bool IsGamePaused { get; private set; }

    private float gamePalyingTimer;

    // TODO - To have more time
    private float gamePalyingTimerMax = 1000f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += OnGameInputPauseAction;
        GameInput.Instance.OnInteractAction += OnGameInputInteractAction;

        // TODO - To start game automatically
        State = GameState.WaitingToStart;
    }

    private void OnDestroy()
    {
        GameInput.Instance.OnPauseAction -= OnGameInputPauseAction;
        GameInput.Instance.OnInteractAction -= OnGameInputInteractAction;
    }

    private void OnGameInputPauseAction(object sender, EventArgs e) =>
        ToglePauseGame();

    private void OnGameInputInteractAction(object sender, EventArgs e)
    {
        if (State == GameState.WaitingToStart)
        {
            gamePalyingTimer = gamePalyingTimerMax;
            State = GameState.CountdownToStart;
            OnStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }


    private void Update()
    {
        switch (State)
        {
            case GameState.WaitingToStart:
                break;

            case GameState.CountdownToStart:

                CountdownToStartTimer -= Time.deltaTime;

                if (CountdownToStartTimer < 0f)
                {
                    State = GameState.GamePlaying;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }

                break;

            case GameState.GamePlaying:

                gamePalyingTimer -= Time.deltaTime;

                if (gamePalyingTimer < 0f)
                {
                    State = GameState.GameOver;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }

                break;

            case GameState.GameOver:
                break;
        }
    }

    public float GetGamePlayTimeNormalized() =>
        1 - (gamePalyingTimer / gamePalyingTimerMax);

    public void ToglePauseGame()
    {
        IsGamePaused = !IsGamePaused;

        Time.timeScale = IsGamePaused ? 0 : 1;

        OnTogglePause?.Invoke(this, EventArgs.Empty);
    }

}
