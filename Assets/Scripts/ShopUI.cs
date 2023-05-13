using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    private Button upgradeShotCount_Button;
    private TMP_Text upgradeShotCount_ButtonText;
    private Button upgradeFiringRate_Button;
    private TMP_Text upgradeFiringRate_ButtonText;
    private Button upgradeReloadTime_Button;
    private TMP_Text upgradeReloadTime_ButtonText;
    private GunController gunController;
    private TMP_Text announceText;
    private Button exitShop_Button;
    private TMP_Text exitShop_ButtonText;

    private bool enoughMoney = true;

    [SerializeField]
    private float announceTime;

    private void Awake()
    {
        upgradeShotCount_Button = this.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Button>();
        upgradeShotCount_ButtonText = upgradeShotCount_Button.transform.GetChild(0).GetComponent<TMP_Text>();
        upgradeFiringRate_Button = this.gameObject.transform.GetChild(0).GetChild(1).GetComponent<Button>();
        upgradeFiringRate_ButtonText = upgradeFiringRate_Button.transform.GetChild(0).GetComponent<TMP_Text>();
        upgradeReloadTime_Button = this.gameObject.transform.GetChild(0).GetChild(2).GetComponent<Button>();
        upgradeReloadTime_ButtonText = upgradeReloadTime_Button.transform.GetChild(0).GetComponent<TMP_Text>();
        gunController = GameObject.Find("Gun").GetComponent<GunController>();
        announceText = this.gameObject.transform.GetChild(0).GetChild(3).GetComponent<TMP_Text>();
        exitShop_Button = this.gameObject.transform.GetChild(0).GetChild(4).GetComponent<Button>();
        exitShop_ButtonText = exitShop_Button.transform.GetChild(0).GetComponent<TMP_Text>();
    }
    private void Start()
    {
        announceText.text = "";
        upgradeShotCount_ButtonText.text = "One extra shot: 200 bucks!";
        upgradeShotCount_Button.onClick.AddListener(delegate { BuyTheDamnThing("shotcount"); });
        upgradeFiringRate_ButtonText.text = "Faster firing rate: 200 bucks!";
        upgradeFiringRate_Button.onClick.AddListener(delegate { BuyTheDamnThing("firingrate"); });
        upgradeReloadTime_ButtonText.text = "Faster reload time: 200 bucks!";
        upgradeReloadTime_Button.onClick.AddListener(delegate { BuyTheDamnThing("reloadtime"); });

        exitShop_ButtonText.text = "Exit shop";
        exitShop_Button.onClick.AddListener(delegate { StartCoroutine(ExitShop(2f)); });

        Time.timeScale = 0f;
    }
    IEnumerator ExitShop(float bye)
    {
        upgradeFiringRate_Button.gameObject.SetActive(false);
        upgradeShotCount_Button.gameObject.SetActive(false);
        upgradeReloadTime_Button.gameObject.SetActive(false);
        exitShop_Button.gameObject.SetActive(false);
        Time.timeScale = 1f;
        announceText.color = Color.magenta;
        announceText.text = "Thank you, come again!";
        yield return new WaitForSeconds(bye);
        Destroy(this.gameObject);
    }
    IEnumerator Announce(string announcement, float wait)
    {
        if(enoughMoney == true)
        {
            announceText.color = Color.green;
        }
        else if(enoughMoney == false)
        {
            announceText.color = Color.red;
        }
        announceText.text = announcement;
        yield return new WaitForSecondsRealtime(wait);
        Debug.Log("Went here in the code");
        //announceText.text = "";
        /*if (enoughMoney == false)
        {
            Time.timeScale = 1f;
            Destroy(this.gameObject);
        }*/
    }
    void BuyTheDamnThing(string whichUpgrade)
    {
        if (PlayerManager.Instance.cash >= 200)
        {
            enoughMoney = true;
        }
        else
            enoughMoney = false;
            
        switch (whichUpgrade)
        {
            case "shotcount":
                if (enoughMoney == true)
                {
                    gunController.shotVolleyCount += 1;
                    PlayerManager.Instance.BuyWithCash(200);
                    StartCoroutine(Announce("Bought a better magazine.", announceTime));
                    StartCoroutine(ExitShop(1));
                }
                else if(enoughMoney == false)
                {
                    StartCoroutine(Announce("Not enough money for better magazine", announceTime));
                }
                break;

            case "firingrate":
                if (enoughMoney == true)
                {
                    gunController.volleyFiringSpeed *= 0.85f;
                    PlayerManager.Instance.BuyWithCash(200);
                    StartCoroutine(Announce("Bought improved firing rate.", announceTime));
                    StartCoroutine(ExitShop(1));
                }
                else if (enoughMoney == false)
                {
                    StartCoroutine(Announce("Not enough money for faster firing", announceTime));
                }
                break;

            case "reloadtime":
                if (enoughMoney == true)
                {
                    gunController.reloadSpeed *= 0.75f;
                    PlayerManager.Instance.BuyWithCash(200);
                    StartCoroutine(Announce("Bought faster reloading", announceTime));
                    StartCoroutine(ExitShop(1));
                }
                else if (enoughMoney == false)
                {
                    StartCoroutine(Announce("Not enough money for faster reload", announceTime));
                }
                break;

        }
            
        
    }

    
}
