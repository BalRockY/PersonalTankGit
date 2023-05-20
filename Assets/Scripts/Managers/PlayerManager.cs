using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // Define and create instance of manager
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

    // Round Stats
    public float hp;
    public float MaxHP;
    public float dmg;
    public float cash;
    public int kills;
    public int lvl;
    public int exp;
    public int expReq;

    // References
    public GameObject tankRef;
    public TankController tankConRef;
    public GunController gunConRef;
    public GameObject LvlUIRef;

    void Awake()
    {
        // Setup Manager
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        tankRef = RoundManager.Instance.theTank;
        tankConRef = tankRef.GetComponent<TankController>();
        gunConRef = tankRef.GetComponentInChildren<GunController>();
        LvlUIRef = Resources.Load<GameObject>("ShopUI");
        PlayerSetup();
    }

    // Player Setup
    public void PlayerSetup()
    {
        MaxHP = tankConRef.hp;
        hp = MaxHP;
        dmg = tankConRef.dmg;
        kills = 0;
        lvl = 0;
        exp = 0;
        expReq = 100;
    }

    // Player Hit By Enemy
    public void PlayerHit(float dmg)
    {
        // Take Damage
        hp -= dmg;

        // Check if dead
        if (hp <= 0) StartCoroutine(tankRef.GetComponent<TankController>().Die(2f));
    }

    // Player Killed Enemy
    public void PlayerKill()
    {
        // Add to killscore
        kills++;
    }

    // Gain EXP 
    public void GainEXP(int expEarned)
    {
        exp += expEarned;

        if (exp >= expReq)
        {
            lvl++;
            Instantiate(LvlUIRef);
            expReq *= 3;
        }
    }

    // Upgrades Enum
    public enum Upgrades
    {
        Vehicle_Speed,
        Vehicle_Manuvering,
        Weapon_Speed,
        Weapon_TurnSpeed,
        ShotCount,
        MaxHP,
        OneTimeHP,
        Damage,
        Acceleration
    }

    // Upgrade!!
    public void Upgrade(int i)
    {
        switch(i)
        {
            // Vehicle Speed
            case 0:
                tankConRef.maxSpeed += 0.5f;
                break;

            // Vehicle Turn Speed
            case 1:
                tankConRef.turnFactor+= 0.5f;
                break;

            // Weapon Speed
            case 2:
                gunConRef.volleyFiringSpeed -= 0.1f;
                
                break;

            // Weapon Turn Speed
            case 3:
                gunConRef.speed += 0.2f;
                break;

            // ShotCount
            case 4:
                gunConRef.shotVolleyCount += 1;
                break;

            // MaxHP
            case 5:
                MaxHP += 10;
                break;

            // One Time HP
            case 6:
                hp = MaxHP;
                break;

            // Damage
            case 7:
                tankConRef.dmg += (tankConRef.dmg/2);
                break;

            // Acceleration
            case 8:
                tankConRef.accelerationFactor += 2f;
                break;
        }
    }
}
