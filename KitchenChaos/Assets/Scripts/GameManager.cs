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
        GamePalying,
        GameOver
    }

    public GameState State { get; private set; } = GameState.WaitingToStart;

    public float CountdownToStartTimer { get; private set; }  = 3f;

    public bool IsGamePaused { get; private set; }

    private float waitingToStartTimer = 1f;
    private float gamePalyingTimer;

    private float gamePalyingTimerMax = 10f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameInput.Instance.OnPauseAction += OnGameInputPauseAction;
    }

    private void OnDestroy()
    {
        GameInput.Instance.OnPauseAction -= OnGameInputPauseAction;
    }

    private void OnGameInputPauseAction(object sender, EventArgs e) =>
        ToglePauseGame();

    private void Update()
    {
        switch (State)
        {
            case GameState.WaitingToStart:

                waitingToStartTimer -= Time.deltaTime;

                if (waitingToStartTimer < 0f)
                {
                    gamePalyingTimer = gamePalyingTimerMax;

                    State = GameState.CountdownToStart;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }

                break;

            case GameState.CountdownToStart:

                CountdownToStartTimer -= Time.deltaTime;

                if (CountdownToStartTimer < 0f)
                {
                    State = GameState.GamePalying;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }

                break;

            case GameState.GamePalying:

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
