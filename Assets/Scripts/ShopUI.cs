using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    private Button[] UpgradeButtons;
    private TMP_Text[] UpgradeButtonsTXT;
    private Button Btn1;
    private TMP_Text Btn1TXT;
    private Button Btn2;
    private TMP_Text Btn2TXT;
    private Button Btn3;
    private TMP_Text Btn3TXT;
    private TMP_Text announceText;
    private Button ExitUIBtn;
    private TMP_Text ExitUIBtnTXT;

    public List<int> CurUpgrades = new List<int>();

    private bool enoughMoney = true;

    [SerializeField]
    private float announceTime;

    private void Awake()
    {
        UpgradeButtons = new Button[3];
        UpgradeButtonsTXT = new TMP_Text[3];

        for (int i = 0; i < 3; i++)
        {
            UpgradeButtons[i] = this.gameObject.transform.GetChild(0).GetChild(i).GetComponent<Button>();
            UpgradeButtonsTXT[i] = UpgradeButtons[i].transform.GetChild(0).GetComponent<TMP_Text>();
        }

        // Announce Text Setup
        announceText = this.gameObject.transform.GetChild(0).GetChild(3).GetComponent<TMP_Text>();

        // Exit Button Setup
        ExitUIBtn = this.gameObject.transform.GetChild(0).GetChild(4).GetComponent<Button>();
        ExitUIBtnTXT = ExitUIBtn.transform.GetChild(0).GetComponent<TMP_Text>();
    }
    private void Start()
    {
        // Get Random Upgrade for each button in upgrade button array, adds name based on enum and a listener onClick for each button
        do
        {
            int curUpgrade = Random.Range(0, System.Enum.GetValues(typeof(PlayerManager.Upgrades)).Length);
            if (!CurUpgrades.Contains(curUpgrade))
            {
                CurUpgrades.Add(curUpgrade);
                UpgradeButtonsTXT[CurUpgrades.Count-1].text = ((PlayerManager.Upgrades)curUpgrade).ToString();
                UpgradeButtons[CurUpgrades.Count-1].onClick.AddListener(delegate { SelectedUpgrade(curUpgrade); });
            }
        }
        while (CurUpgrades.Count < 3);
            
        // Clear announce Text
        announceText.text = "";

        // Setup Exit Button
        ExitUIBtnTXT.text = "Cancel";
        ExitUIBtn.onClick.AddListener(delegate { StartCoroutine(ExitShop(2f)); });

        // Pauses game
        Time.timeScale = 0f;
    }

    // Exit Shop
    IEnumerator ExitShop(float bye)
    {
        foreach(Button btn in UpgradeButtons) btn.gameObject.SetActive(false);
        ExitUIBtn.gameObject.SetActive(false);
        Time.timeScale = 1f;
        announceText.color = Color.magenta;
        announceText.text = "Thank you, come again!";
        yield return new WaitForSeconds(bye);
        Destroy(this.gameObject);
    }

    // Announce
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
    void SelectedUpgrade(int whichUpgrade)
    {
        PlayerManager.Instance.Upgrade(whichUpgrade);
        StartCoroutine(ExitShop(2));        
    }

    
}
