using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Define and create instance of manager
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance is null)
                Debug.LogError("Game Manager is NULL");

            return _instance;
        }
    }
    
    // Game Variables
    public GameState State;
    public static event System.Action<GameState> OnGameStateChanged;

    // Render Distance
    [SerializeField]
    public int renderDistance = 20;


    private void Awake()
    {
        // Setup Manager
        _instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        UpdateGameState(GameState.Start);
        Physics.IgnoreLayerCollision(8, 3);
        Physics.IgnoreLayerCollision(8, 8);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UpdateGameState(GameState.RoundOver);
        }
    }

    // State Handlers
    public void UpdateGameState(GameState newState)
    {
        State = newState;

        switch (newState)
        {
            case GameState.MainMenu:
                HandleMainMenu();
                break;

            case GameState.Start:
                HandleStartGame();
                break;

            case GameState.Playing:
                HandlePlayingGame();
                break;

            case GameState.Paused:
                HandlePauseGame();
                break;

            case GameState.RoundOver:
                HandleGameOver();
                break;
            case GameState.RestartGame:
                HandleRestartGame();
                break;
            case GameState.RoundWon:
                HandleRoundWon();
                break;

            default:
                break;
        }

        OnGameStateChanged?.Invoke(newState);
    }
    void HandleStartGame()
    {

    }

    void HandleMainMenu()
    {

    }
    void HandlePlayingGame()
    {
        Time.timeScale = 1f;
    }
    void HandlePauseGame()
    {
        Time.timeScale = 0f;
    }
    void HandleGameOver()
    {

    }
    void HandleRestartGame()
    {

    }
    void HandleRoundWon()
    {
        
    }
}

// Gamestates Enum
public enum GameState
{
    MainMenu,
    Start,
    Playing,
    Paused,
    RoundOver,
    RestartGame,
    RoundWon
}
