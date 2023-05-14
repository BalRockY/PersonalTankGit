using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    // Define and create instance of manager
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance is null)
                Debug.LogError("UI Manager is NULL");

            return _instance;
        }
    }    

    // HUD Element Variables
    private TMP_Text killsText, moneyText, hpText;
    private GameObject hud;

    // End Screen
    private GameObject endScreen;

    // Menus
    public List<GameObject> menus;

    // Buttons
    private Button restartGame;

    private void Awake()
    {
        // Setup Manager
        _instance = this;
        GameManager.OnGameStateChanged += StateChangeManager;
        endScreen = GameObject.Find("UI/EndScreen");
        hud = GameObject.Find("UI/HUD");
        
        // Setup HUD Element References
        killsText = hud.gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        moneyText = hud.gameObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>();
        hpText = hud.gameObject.transform.GetChild(2).gameObject.GetComponent<TMP_Text>();

        // Setup Menus List
        menus = new List<GameObject>();
        menus.Add(endScreen);
        menus.Add(hud);

        // Set EndScreen Restart Button Reference
        restartGame = GameObject.Find("EndScreen").transform.GetChild(0).GetComponent<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {
        restartGame.onClick.AddListener(RestartGame);
    }

    // Update is called once per frame
    void Update()
    {
        killsText.text = "Kills: " + PlayerManager.Instance.kills.ToString("0");
        moneyText.text = "Cash: " + PlayerManager.Instance.cash.ToString("0");
        hpText.text = "HP: " + PlayerManager.Instance.hp.ToString("0");
    }

    // State Handlers
    private void StateChangeManager(GameState newState)
    {
        switch (newState)
        {
            case GameState.MainMenu:
                break;

            case GameState.Start:
                HUD();
                break;

            case GameState.Playing:

                break;

            case GameState.Paused:
                break;

            case GameState.GameOver:
                EndScreen();
                break;

            case GameState.RestartGame:
                RestartUI();
                break;


            default:
                break;
        }
    }

    // Activate HUD
    void HUD()
    {
        ClearMenus();
        hud.SetActive(true);
    }

    // Restart Round UI
    void RestartUI()
    {
        ClearMenus();
        HUD();
    }
    
    // Show EndScreen
    void EndScreen()
    {
        ClearMenus();
        endScreen.SetActive(true);
    }

    // Restart Round Button Function
    void RestartGame()
    {
        GameManager.Instance.UpdateGameState(GameState.RestartGame);
    }

    // Clear All Menus
    void ClearMenus()
    {
        foreach (GameObject menuObj in menus)
        {
            menuObj.SetActive(false);
        }
    }



}
