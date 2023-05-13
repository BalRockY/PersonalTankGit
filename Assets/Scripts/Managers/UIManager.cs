using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
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
    private void UIManagerStateChange(GameState newState)
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
    private TMP_Text killsText;
    private TMP_Text moneyText;
    private TMP_Text hpText;
    private GameObject endScreen;
    private GameObject hud;
    public List<GameObject> menus;
    private Button restartGame;

    public float kills = 0;
    public float cash = 0;
    public float hp;
    private void Awake()

    {
        _instance = this;
        gameManager.OnGameStateChanged += UIManagerStateChange;
        endScreen = GameObject.Find("UI/EndScreen");
        hud = GameObject.Find("UI/HUD");
        

        killsText = hud.gameObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        moneyText = hud.gameObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>();
        hpText = hud.gameObject.transform.GetChild(2).gameObject.GetComponent<TMP_Text>();

        menus = new List<GameObject>();
        menus.Add(endScreen);
        menus.Add(hud);

        restartGame = GameObject.Find("EndScreen").transform.GetChild(0).GetComponent<Button>();
    }
    void Start()
    {
        restartGame.onClick.AddListener(RestartGame);
    }
    void RestartGame()
    {
        gameManager.Instance.UpdateGameState(GameState.RestartGame);
    }
    void RestartUI()
    {
        ClearMenus();
        kills = 0;
        cash = 0;
        hp = 0;
        HUD();
    }
    void ClearMenus()
    {
        foreach (GameObject menuObj in menus)
        {
            menuObj.SetActive(false);
        }
    }
    void EndScreen()
    {
        ClearMenus();
        endScreen.SetActive(true);
    }
    void HUD()
    {
        ClearMenus();
        hud.SetActive(true);
    }


    public void SwitchMenu(string menu)
    {
        foreach(GameObject menuObj in menus)
        {
            menuObj.SetActive(false);
        }
        switch (menu)
        {
            case "endScreen":
                endScreen.SetActive(true);
                break;
            case "hud":
                hud.SetActive(true);
                break;
                    
            default:
                Debug.Log("No such menu exists");
                break;
        }
    }
    
    public void BuyWithCash(float amount)
    {
        cash -= amount;
    }
    // Update is called once per frame
    void Update()
    {
        killsText.text = "Kills: " + kills.ToString("0");
        moneyText.text = "Cash: " + cash.ToString("0");
        hpText.text = "HP: " + hp.ToString("0");
    }
}
