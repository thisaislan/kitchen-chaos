using System;
using UnityEngine;
using static Player;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;

    public enum GameState
    {
        WaitingToStart,
        CountdownToStart,
        GamePalying,
        GameOver
    }

    public GameState State { get; private set; } = GameState.WaitingToStart;

    public float CountdownToStartTimer { get; private set; }  = 3f;

    private float waitingToStartTimer = 1f;
    private float gamePalyingTimer = 10f;



    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        switch (State)
        {
            case GameState.WaitingToStart:

                waitingToStartTimer -= Time.deltaTime;

                if (waitingToStartTimer < 0f)
                {
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

}
