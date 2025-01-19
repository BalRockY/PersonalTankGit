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

    // Player Stats
    public float hp;
    public float MaxHP;
    public float MaxSpeed;
    public float dmg;
    public float cash;
    public int kills;
    public int lvl;
    public int exp;
    public int expReq;

    // Player states
    public bool driving = false;
    public bool walking = false;

    // Tank Stats
    public float turretRotationSpeed;
    public float firingSpeed;
    public float tankMass;

    // References
    public GameObject tankRef;
    private TankController tankConRef;
    private WalkingCharacterController walkingConRef;
    public GunController gunConRef;
    public Vector3 tankEnterVehicleRadius;
    public Vector3 exitVehiclePos;
    public GameObject walkingCharacterPrefab;
    private GameObject walkingCharacterInstantiatedObject;
    private AudioManager aManagerRef;
    

    // Boolians
    public bool insideVehicle;


    void Awake()
    {
        // Setup Manager
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        tankRef = GameObject.FindGameObjectWithTag("Tank");
        tankConRef = tankRef.GetComponent<TankController>();
        gunConRef = tankRef.GetComponentInChildren<GunController>();
        //Instantiate(snowGeneratorPrefab);
        //StartCoroutine(MoveSnow());
        PlayerSetup();
        insideVehicle = true;
        aManagerRef = AudioManager.Instance;
        
    }

    // State Handlers
    void StateChangeManager(GameState newState)
    {
        switch (newState)
        {
            case GameState.MainMenu:
                break;

            case GameState.Start:
                break;

            case GameState.Playing:
                break;

            case GameState.Paused:
                break;

            case GameState.RoundOver:
                break;

            case GameState.RestartGame:
                break;

            case GameState.RoundWon:
                break;

            default:
                break;
        }
    }

    private void Update()
    {
        EnterExitVehicle();
        
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
        MaxSpeed = tankConRef.maxSpeed;
    }

    // Player Hit By Enemy
    public void PlayerHit(float dmg) // Function is called in EnemyController
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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(tankEnterVehicleRadius, 1.2f);
    }
    void EnterExitVehicle()
    {
        // Get the position of the tank
        tankEnterVehicleRadius = GameObject.FindGameObjectWithTag("EnterVehicle").transform.position;

        Collider2D[] enterVehicleArea = Physics2D.OverlapCircleAll(tankEnterVehicleRadius, 1.2f);


        if (Input.GetKeyDown(KeyCode.E))
        {
            if(insideVehicle == true)
            {
                aManagerRef.aSourceAM4.PlayOneShot(aManagerRef.tankVaultClose3);
                exitVehiclePos = GameObject.FindGameObjectWithTag("ExitVehicle").transform.position;
                tankConRef.SetAudioListener(false); // Disable audio listener on tank while walking
                walkingCharacterInstantiatedObject = Instantiate(walkingCharacterPrefab, exitVehiclePos, Quaternion.identity);
                Debug.Log("Exited Vehicle");
                tankConRef.aSource.enabled = false;
                tankConRef.animator.enabled = false;
                tankConRef.enabled = false;
                gunConRef.enabled = false;
                insideVehicle = false;
                tankRef.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll; // freeze tank
                driving = false;
                walking = true;
            }
            else
            {
                bool playerDetected = false;
                foreach (Collider2D collider in enterVehicleArea)
                {
                    if (collider.CompareTag("Player"))
                    {
                        playerDetected = true;
                        break;
                    }
                }

                if (playerDetected)
                {
                    Destroy(walkingCharacterInstantiatedObject);
                    tankConRef.SetAudioListener(true);
                    Debug.Log("Entered Vehicle");
                    aManagerRef.aSourceAM4.PlayOneShot(aManagerRef.tankVaultOpen1);
                    tankConRef.enabled = true;
                    tankConRef.aSource.enabled = true;
                    tankConRef.animator.enabled = true;
                    gunConRef.enabled = true;
                    insideVehicle = true;
                    tankRef.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None; // unfreeze tank
                    driving = true;
                    walking = false;
                }
            }
        }
    }

    /*
    // Gain EXP 
    public void GainEXP(int expEarned)
    {
        exp += expEarned;

        if (exp >= expReq)
        {
            lvl++;
            Instantiate(LvlUIRef);
            expReq += lvl*100;
        }
    }
    */

    // Upgrades Enum
    public enum Upgrades
    {
        Vehicle_Speed,
        Vehicle_Manuvering,
        Weapon_FiringSpeed,
        Weapon_TurnSpeed,
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
                gunConRef.turretRotationSpeedFactor += 0.2f;
                break;

            // MaxHP
            case 4:
                MaxHP += 10;
                break;

            // One Time HP
            case 5:
                hp = MaxHP;
                break;

            // Damage
            case 6:
                tankConRef.dmg += 5;
                break;

            // Acceleration
            case 7:
                tankConRef.accelerationFactor += 2f;
                break;
        }
    }
}
