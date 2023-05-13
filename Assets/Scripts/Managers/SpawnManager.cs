using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

public class SpawnManager : MonoBehaviour
{
    private GameObject enemy;
    private GameObject wall1x2;
    private GameObject wall1x1;
    private NavMeshSurface surface;
    private GameObject moneyPickup;
    public GameObject theTank;
    private GameObject level;
    public GameObject theShop;

    private GameObject[] zombiesInstantiated;
    private GameObject[] obstaclesInstantiated;
    private GameObject shopInstantiaded;
    private GameObject levelInstantiated;
    private GameObject[] splatAnimsInstantiated;
    private GameObject[] pickUpsInstantiated;

    private TankController tankController;
    private CameraController camControl;


    [SerializeField]
    private float enemySpawnInterval;
    [SerializeField]
    private float moneySpawnInterval;
    [SerializeField]
    private float spawnScaler = 0.99995f;
    [SerializeField]
    private int moneySpawnFactor;
    [SerializeField]
    private int wallSpawn1x1;
    [SerializeField]
    private int wallSpawn1x2;

    private float enemySpawnIntervalLoader;

    private static SpawnManager _instance;
    public static SpawnManager Instance
    {
        get
        {
            if (_instance is null)
                Debug.LogError("SpawnManager Manager is NULL");

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
                SpawnLevel();
                SpawnTank();
                SpawnObstacles();
                surface.BuildNavMesh();
                StartCoroutine(ZombieSpawner());
                //StartCoroutine(MoneySpawn());
                SpawnShop();
                break;

            case GameState.Playing:
                break;

            case GameState.Paused:
                break;

            case GameState.GameOver:
                StopCoroutine(ZombieSpawner());
                KillZombies();
                KillTank();
                KillObstacles();
                KillShop();
                KillLevel();
                KillSplat();
                KillPickups();
                break;
            case GameState.RestartGame:
                enemySpawnInterval = enemySpawnIntervalLoader;
                SpawnLevel();
                SpawnTank();
                SpawnObstacles();
                surface.BuildNavMesh();
                StartCoroutine(ZombieSpawner());
                //StartCoroutine(MoneySpawn());
                SpawnShop();
                break;

            default:
                break;
        }
    }

    private void Awake()
    {
        _instance = this;
        gameManager.OnGameStateChanged += UIManagerStateChange;

        theTank = Resources.Load<GameObject>("Tank");
        surface = GameObject.Find("NavMesh").GetComponent<NavMeshSurface>();
        wall1x2 = Resources.Load<GameObject>("Wall1x2");
        wall1x1 = Resources.Load<GameObject>("Wall1x1");
        enemy = Resources.Load<GameObject>("Enemy");
        moneyPickup = Resources.Load<GameObject>("MoneyPickUp");
        level = Resources.Load<GameObject>("Level");
        theShop = Resources.Load<GameObject>("TheShop");
        camControl = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();

    }

    void Start()
    {
        enemySpawnIntervalLoader = enemySpawnInterval;
    }
    void SpawnLevel()
    {
        Instantiate(level);
    }
    public void SpawnTank()
    {
        Instantiate(theTank);
        camControl.FindTank();
    }
    
    void SpawnObstacles()
    {
        SpawnWalls();
    }

    void SpawnShop()
    {
        Vector3 shopSpawnLoc = new Vector3(Random.Range(-45, 45), Random.Range(-45, 45), 0);
        Instantiate(theShop, shopSpawnLoc, Quaternion.identity);
    }

    

    public void SpawnWalls()
    {
        //Method to draw the ray in scene for debug purpose
        
        for (int i = 0; i< wallSpawn1x2; i++)
        {
            
            var position = new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), 0.4f);
            /*Gizmos.color = Color.red;
            Collider2D hit = Physics2D.OverlapBox(position, new Vector2(1, 2), 0f);
            Gizmos.DrawCube(new Vector3(hit.transform.position.x, hit.transform.position.y, 0f), new Vector3(1f,2f,0f));*/

            if (position != GameObject.FindGameObjectWithTag("Tank").transform.position)
            {
                Instantiate(wall1x2, position, Quaternion.identity);
            }
            
        }
        for (int i = 0; i < wallSpawn1x1; i++)
        {
            var position = new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), 0.4f);
            if (position != GameObject.FindGameObjectWithTag("Tank").transform.position)
            {
                Instantiate(wall1x1, position, Quaternion.identity);
            }
        }

    }
    /*public IEnumerator MoneySpawn()
    {
        while (true)
        {
            Vector3 moneySpawnLoc = new Vector3(Random.Range(-48, 48), Random.Range(-48, 48), 0);
            if (moneySpawnLoc != GameObject.FindGameObjectWithTag("Wall").transform.position)
            {
                Instantiate(moneyPickup, moneySpawnLoc, Quaternion.identity);
            }

            yield return new WaitForSeconds(moneySpawnInterval);
        }
    }*/
            public void MoneySpawn(Vector3 deathLocation)
    {
        int randomNumber;
        randomNumber = Random.Range(1, 10);
        if(randomNumber > moneySpawnFactor)
        Instantiate(moneyPickup, deathLocation, Quaternion.identity);
    }
    public IEnumerator ZombieSpawner()
    {
        tankController = GameObject.FindGameObjectWithTag("Tank").GetComponent<TankController>();
        while (tankController.dead == false)
        {
            if (tankController == null)
            {
                break;
            }
            Vector3 enemySpawnLoc = new Vector3(Random.Range(-48, 48), Random.Range(-48, 48), 0);
            /*if(enemySpawnLoc != GameObject.FindGameObjectWithTag("Wall").transform.position && enemySpawnLoc != GameObject.FindGameObjectWithTag("Tank").transform.position)
            {*/
                Instantiate(enemy, enemySpawnLoc, Quaternion.identity);
            //}
            yield return new WaitForSeconds(enemySpawnInterval);
        }
    }

    public void KillZombies()
    {
        zombiesInstantiated = GameObject.FindGameObjectsWithTag("Enemy");
        for(int i = 0; i< zombiesInstantiated.Length; i++)
        {
            Destroy(zombiesInstantiated[i]);
        }
    }
    public void KillTank()
    {
        Destroy(GameObject.FindGameObjectWithTag("Tank"));
    }

    void KillObstacles()
    {
        obstaclesInstantiated = GameObject.FindGameObjectsWithTag("Wall");
        for(int i = 0; i < obstaclesInstantiated.Length; i++)
        {
            Destroy(obstaclesInstantiated[i]);
        }
    }
    void KillLevel()
    {
        levelInstantiated = GameObject.FindGameObjectWithTag("Level");
        Destroy(levelInstantiated);

    }
    void KillShop()
    {
        shopInstantiaded = GameObject.FindGameObjectWithTag("Shop");

            Destroy(shopInstantiaded);
    }
    void KillPickups()
    {
        pickUpsInstantiated = GameObject.FindGameObjectsWithTag("Money");
        for (int i = 0; i < pickUpsInstantiated.Length; i++)
        {
            Destroy(pickUpsInstantiated[i]);
        }
    }

    void KillSplat()
    {
        splatAnimsInstantiated = GameObject.FindGameObjectsWithTag("SplatAnim");
        for (int i = 0; i < splatAnimsInstantiated.Length; i++)
        {
            Destroy(splatAnimsInstantiated[i]);
        }
    }
    void Update()
    {

    }
    private void FixedUpdate()
    {
        enemySpawnInterval = enemySpawnInterval * spawnScaler;
    }
}
