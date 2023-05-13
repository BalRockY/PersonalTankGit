using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour
{
    private static gameManager _instance;
    public static gameManager Instance
    {
        get
        {
            if (_instance is null)
                Debug.LogError("Game Manager is NULL");

            return _instance;
        }
    }
    

    public GameState State;
    public static event System.Action<GameState> OnGameStateChanged;

    [SerializeField]
    public int renderDistance = 20;

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

            case GameState.GameOver:
                HandleGameOver();
                break;
            case GameState.RestartGame:
                HandleRestartGame();
                break;

            default:
                break;
        }

        OnGameStateChanged?.Invoke(newState);
    }

    private CameraController camControl;
    public void HandleStartGame()
    {
    }
    public void HandleMainMenu()
    {

    }
    public void HandlePlayingGame()
    {
        Time.timeScale = 1f;
    }

    public void HandlePauseGame()
    {
        Time.timeScale = 0f;
    }
    public void HandleGameOver()
    {

    }
    public void HandleShop()
    {
        
    }
    void HandleRestartGame()
    {

    }


    private void Awake()
    {
        _instance = this;
        
    }

    private void Start()
    {
        UpdateGameState(GameState.Start);
        Physics.IgnoreLayerCollision(8, 3);
        Physics.IgnoreLayerCollision(8, 8);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            UpdateGameState(GameState.GameOver);
        }
    }


}
public enum GameState
{
    MainMenu,
    Start,
    Playing,
    Paused,
    GameOver,
    RestartGame,
    Shop
}
