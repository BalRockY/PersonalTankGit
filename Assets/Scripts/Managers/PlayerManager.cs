using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager _instance;
    public static PlayerManager Instance
    {
        get
        {
            if (_instance is null)
                Debug.LogError("SpawnManager Manager is NULL");

            return _instance;
        }
    }

    // Variables
    public float hp;
    public float dmg;
    public float cash;
    public int kills;
    public int lvl;
    public int exp;

    // References
    public TankController tankCon;

    private void Awake()
    {
        _instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        tankCon = SpawnManager.Instance.theTank.GetComponent<TankController>();

    }

    void PlayerSetup()
    {
        hp = tankCon.hp;
        dmg = tankCon.dmg;
        kills = 0;
        lvl = 0;
        exp = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playerhit(float dmg)
    {
        // Take Damage
        hp -= dmg;

        // Check if dead
        if (hp <= 0) tankCon.Die(2);
    }

    public void playerkill()
    {
        // Add to killscore
        kills++;
    }

    public void GainEXP(int expEarned)
    {
        exp += expEarned;
        CheckLevelUp();
    }

    void CheckLevelUp()
    {
        if(exp >= lvl * 100)
        {
            lvl++;
        }
    }

    public void BuyWithCash(float amount)
    {
        cash -= amount;
    }
}
