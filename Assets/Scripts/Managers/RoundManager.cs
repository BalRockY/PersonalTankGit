using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RoundManager : MonoBehaviour
{
    // Define and create instance of manager
    private static RoundManager _instance;
    public static RoundManager Instance
    {
        get
        {
            if (_instance is null)
                Debug.LogError("SpawnManager Manager is NULL");

            return _instance;
        }
    }

    // Player Variables
    public GameObject theTank;
    private TankController tankController;
    private CameraController camControl;

    // Level & Navigation Variables
    private GameObject level;
    private GameObject levelInstantiated;
    private NavMeshSurface surface;

    // Obstacle Variables
    private GameObject wall_1x2;
    private GameObject wall_1x1;
    private GameObject[] obstaclesInstantiated;
    [SerializeField]
    private int wallSpawn1x1;
    [SerializeField]
    private int wallSpawn1x2;

    // Pick Up Variables
    private GameObject[] pickUpsInstantiated;

    private GameObject moneyPickup;
    [SerializeField]
    private float moneySpawnInterval;
    [SerializeField]
    private int moneySpawnFactor;

    // Enemy Variables
    private GameObject[] zombiesInstantiated;
    private GameObject enemy;

    [SerializeField]
    private float spawnScaler = 0.99995f;
    private float enemySpawnIntervalLoader;
    [SerializeField]
    private float enemySpawnInterval;

    // VFX Variables
    private GameObject[] splatAnimsInstantiated;

    void Awake()
    {
        // Setup Manager
        _instance = this;
        GameManager.OnGameStateChanged += StateChangeManager;

        // Load Resources
        enemy = Resources.Load<GameObject>("Enemy");
        level = Resources.Load<GameObject>("Level");
        theTank = Resources.Load<GameObject>("Tank");        
        wall_1x2 = Resources.Load<GameObject>("Wall1x2");
        wall_1x1 = Resources.Load<GameObject>("Wall1x1");
        moneyPickup = Resources.Load<GameObject>("MoneyPickUp");

        // Find NavMesh
        surface = GameObject.Find("NavMesh").GetComponent<NavMeshSurface>();

        // Find Camera Controller
        camControl = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();

    }

    // Start is called before the first frame update
    void Start()
    {
        enemySpawnIntervalLoader = enemySpawnInterval;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Set Spawn Interval
        enemySpawnInterval = enemySpawnInterval * spawnScaler;
    }

    // State Handlers
    void StateChangeManager(GameState newState)
    {
        switch (newState)
        {
            case GameState.MainMenu:

                break;

            case GameState.Start:
                StartCoroutine(StartRound());
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
                KillLevel();
                KillSplat();
                KillPickups();
                break;
            case GameState.RestartGame:
                enemySpawnInterval = enemySpawnIntervalLoader;
                StartCoroutine(StartRound());
                break;

            default:
                break;
        }
    }

    // Start Round State
    IEnumerator StartRound()
    {
        // Spawn & Setup Level
        Instantiate(level);
        SpawnObstacles();

        // Spawn & Setup Tank
        Instantiate(theTank);
        camControl.FindTank();

        // Build Navigation Mesh
        surface.BuildNavMesh();

        // ADD ROUND START COUNT DOWN

        // Start Spawn Enemies
        StartCoroutine(ZombieSpawner());

        return null;
    }
    
    // Spawn Functions
    void SpawnObstacles()
    {
        SpawnWalls();
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
                Instantiate(wall_1x2, position, Quaternion.identity);
            }
            
        }
        for (int i = 0; i < wallSpawn1x1; i++)
        {
            var position = new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), 0.4f);
            if (position != GameObject.FindGameObjectWithTag("Tank").transform.position)
            {
                Instantiate(wall_1x1, position, Quaternion.identity);
            }
        }

    }
    public IEnumerator SpawnPickUp()
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
    }
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


    // Clear Functions
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
   
}
