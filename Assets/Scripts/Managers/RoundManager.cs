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

    //Manager variables
    private AudioSource AMaSource3;

    // Player Variables
    public GameObject theTank;
    private TankController tankController;
    private CameraController camControl;
    private GameObject[] mines;

    // Level & Navigation Variables
    private GameObject level;
    private GameObject levelInstantiated;
    private NavMeshSurface surface;
    public GameObject caravanImage;
    public GameObject theShop;

    //Environmental variables
    [SerializeField] private GameObject rainParcticles = null;
    private Transform mainCamTransform;
    [SerializeField] private float rainMoveInterval;
    public bool isRaining;
    [SerializeField] private GameObject theSun;
    [SerializeField] private float sunSpeed;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject rainImpactPrefab;
    [SerializeField] private float rainImpactInterval;
    [SerializeField] private Vector3 theSunStartPos;

    // Obstacle Variables
    private GameObject wall_1x2;
    private GameObject wall_1x1;
    [SerializeField] private GameObject industrialBuilding_1;
    private GameObject[] obstaclesInstantiated;
    [SerializeField] private int wallSpawn1x1;
    [SerializeField] private int wallSpawn1x2;
    [SerializeField] private int building_industr_spawnCount;
    private GameObject gateLeft;
    private GameObject gateRight;
    [SerializeField] private float wallElevation;
    [SerializeField] private GameObject streetLamp;
    [SerializeField] private float lampElevation;
    [SerializeField] private int streetLampSpawnCount;
    public WallPositionData metalWallSpawnPositions;

    // Pick Up Variables
    private GameObject[] pickUpsInstantiated;
    private GameObject moneyPickup;
    [SerializeField] private float moneySpawnInterval;
    [SerializeField] private int moneySpawnFactor;

    // Enemy Variables
    private GameObject[] zombiesInstantiated;
    [SerializeField] private GameObject enemy;

    // Level triggers
    private bool roundComplete = false;
    public bool triggerEndRound = false;


    // Enemy spawn variables
    [SerializeField] private float spawnScaler = 0.99995f;
    private float enemySpawnIntervalLoader;
    [SerializeField] private float enemySpawnInterval;
    public int enemyClusterCount;
    public int maxEnemyCount;
    public int currentEnemiesSpawnedCount;

    // VFX Variables
    private GameObject[] splatAnimsInstantiated;



    void Awake()
    {
        // Setup Manager
        _instance = this;
        GameManager.OnGameStateChanged += StateChangeManager;

        // Load Resources
        //enemy = Resources.Load<GameObject>("Enemy");
        level = Resources.Load<GameObject>("Level");
        theTank = Resources.Load<GameObject>("Tank");        
        wall_1x2 = Resources.Load<GameObject>("Wall1x2");
        wall_1x1 = Resources.Load<GameObject>("Wall1x1");
        moneyPickup = Resources.Load<GameObject>("MoneyPickUp");
        

        // Find NavMesh
        //surface = GameObject.Find("NavMesh").GetComponent<NavMeshSurface>();

        // Find Camera Controller
        camControl = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        mainCamTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;

        AMaSource3 = AudioManager.Instance.aSourceAM3;
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

        Sunwalk();

        //  HandleOpenGates()
         
    }

    void HandleOpenGates()
    {
        // Open gates once
        /*if (roundComplete == false)
        {
            GateOpener();
        }

        // Win round once
        if (triggerEndRound == true)
        {
            HandleRoundWon();
            triggerEndRound = false;
        }*/
    }

    // State Handlers
    void StateChangeManager(GameState newState)
    {
        switch (newState)
        {
            case GameState.MainMenu:

                break;

            case GameState.Start:
                Debug.Log("Starting spawn enumerator");
                StartCoroutine(StartRound());
                break;

            case GameState.Playing:
                break;

            case GameState.Paused:
                break;

            case GameState.RoundOver:
                StopCoroutine(ZombieSpawner());
                KillZombies();
                KillTank();
                KillObstacles();
                KillLevel();
                KillSplat();
                KillPickups();
                KillMines();
                break;
            case GameState.RestartGame:
                enemySpawnInterval = enemySpawnIntervalLoader;
                roundComplete = false;
                StartCoroutine(StartRound());
                break;
            case GameState.RoundWon:
                HandleRoundWon();
                break;

            default:
                break;
        }
    }

    // Start Round State
    IEnumerator StartRound()
    {
        
        // Spawn & Setup Tank
        Instantiate(theTank);
        camControl.FindTank();
        theTank = GameObject.FindGameObjectWithTag("Tank");


        
        isRaining = true;
        if(isRaining)
        {
            StartRain();
        }
        Instantiate(theSun);
        theSun.transform.position = theSunStartPos;



        // Build Navigation Mesh
        //surface.BuildNavMesh();

        // ADD ROUND START COUNT DOWN
        
        
        SpawnObstacles();
        // Start Spawn Enemies
        StartCoroutine(ZombieSpawner());

        // Reference complete level-area
        //caravanImage = GameObject.Find("CaravanPicture");


        yield return new WaitForEndOfFrame();

        
    }
    void SpawnArenaLevel()
    {
        // Spawn & Setup Level
        Instantiate(level);
        gateLeft = GameObject.Find("GateLeft");
        gateRight = GameObject.Find("GateRight");
        
    }

    void HandleRoundWon()
    {
        StartCoroutine(ChangeLevel(5));
    }

    IEnumerator ChangeLevel(float seconds)
    {
        GameManager.Instance.UpdateGameState(GameState.RoundOver);
        yield return new WaitForSeconds(seconds);
        GameManager.Instance.UpdateGameState(GameState.RestartGame);
        
    }

    public void Sunwalk()
    {
        
        Vector3 targetPos = new Vector3(230f, theSun.transform.position.y, theSun.transform.position.z);

        theSun.transform.position = Vector3.MoveTowards(theSun.transform.position, targetPos, sunSpeed * Time.deltaTime);

    }
    public void StartRain()
    {
        Vector3 particlePos = new Vector3(theTank.transform.position.x, theTank.transform.position.y, -45f);
        
        GameObject particles = Instantiate(rainParcticles, particlePos, rainParcticles.transform.rotation);
        rainParcticles = particles;
        rainParcticles.transform.position = particlePos;

        AMaSource3.clip = AudioManager.Instance.rain1;
        AMaSource3.Play();
        AMaSource3.loop = true;

        StartCoroutine(RainMover(rainMoveInterval));
        StartCoroutine(RainImpact(rainImpactInterval));
    }


    IEnumerator RainImpact(float interval)
    {
        
        StartCoroutine(RainImpactSpawner());
        yield return new WaitForSeconds(interval);
        if(isRaining)
        {
            StartCoroutine(RainImpact(rainImpactInterval));
        }
    }

    IEnumerator RainImpactSpawner()
    {
        
        Camera mainCamera = Camera.main;
        // Manually set range. The numbers represent Unity units.
        float spawnX = mainCamera.transform.position.x + Random.Range(-8f, 8f); 
        float spawnY = mainCamera.transform.position.y + Random.Range(-6f, 6f);

        Vector3 spawnPosition = new Vector3(spawnX, spawnY, rainImpactPrefab.transform.position.z);

        GameObject rainImpactObject = Instantiate(rainImpactPrefab, spawnPosition, Quaternion.identity);
        yield return new WaitForSeconds(0.4f);
        Destroy(rainImpactObject);
    }

    Vector2 CalculateCameraBounds(Camera camera)
    {
        float cameraHeight = 2f * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad) * camera.transform.position.z;
        float cameraWidth = cameraHeight * camera.aspect;

        return new Vector2(cameraWidth / 2f, cameraHeight / 2f);
    }

IEnumerator RainMover(float interval)
    {

        Vector3 particlePos = new Vector3(theTank.transform.position.x, theTank.transform.position.y, -45f);
        rainParcticles.transform.position = particlePos;
        yield return new WaitForSeconds(interval);
        if(isRaining)
        {
            StartCoroutine(RainMover(rainMoveInterval));
        }
        
    }
    // Spawn Functions
    void SpawnObstacles()
    {
        SpawnIndustrialBuildings();
        SpawnWalls();
        SpawnShop();
        SpawnLamps();
    }
    void SpawnShop()
    {
        float transformX = Random.Range(-45f, 45f);
        float transformY = Random.Range(-45f, 45f);
        Vector3 position = new Vector3(transformX, transformY, 0f);
        Instantiate(theShop, position, transform.rotation);
    }
    void SpawnLamps()
    {
        for (int i = 0; i < streetLampSpawnCount; i++)
        {

            var position = new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), lampElevation);
            /*Gizmos.color = Color.red;
            Collider2D hit = Physics2D.OverlapBox(position, new Vector2(1, 2), 0f);
            Gizmos.DrawCube(new Vector3(hit.transform.position.x, hit.transform.position.y, 0f), new Vector3(1f,2f,0f));*/

            if (position != GameObject.FindGameObjectWithTag("Tank").transform.position) // Virker ikke, da tankens transform.position er et lille punkt, midt i tanken. Walls kan stadig spawne oven i tanken.
            {
                Instantiate(streetLamp, position, Quaternion.identity);
            }

        }
    }
    void SpawnIndustrialBuildings()
    {
        for (int i = 0; i < building_industr_spawnCount; i++)
        {

            var position = new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), wallElevation);
            /*Gizmos.color = Color.red;
            Collider2D hit = Physics2D.OverlapBox(position, new Vector2(1, 2), 0f);
            Gizmos.DrawCube(new Vector3(hit.transform.position.x, hit.transform.position.y, 0f), new Vector3(1f,2f,0f));*/

            if (position != GameObject.FindGameObjectWithTag("Tank").transform.position) // Virker ikke, da tankens transform.position er et lille punkt, midt i tanken. Walls kan stadig spawne oven i tanken.
            {
                Instantiate(industrialBuilding_1, position, Quaternion.identity);
            }

        }
    }
    public void SpawnWalls()
    {
        //Method to draw the ray in scene for debug purpose
        
        // Random location spawner:
        for (int i = 0; i< wallSpawn1x2; i++)
        {
            
            var position = new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), wallElevation);
            /*Gizmos.color = Color.red;
            Collider2D hit = Physics2D.OverlapBox(position, new Vector2(1, 2), 0f);
            Gizmos.DrawCube(new Vector3(hit.transform.position.x, hit.transform.position.y, 0f), new Vector3(1f,2f,0f));*/

            if (position != GameObject.FindGameObjectWithTag("Tank").transform.position) // Virker ikke, da tankens transform.position er et lille punkt, midt i tanken. Walls kan stadig spawne oven i tanken.
            {
                Instantiate(wall_1x2, position, Quaternion.identity);
            }
            
        }
        for (int i = 0; i < wallSpawn1x1; i++)
        {
            var position = new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), wallElevation);
            if (position != GameObject.FindGameObjectWithTag("Tank").transform.position)
            {
                Instantiate(wall_1x1, position, Quaternion.identity);
            }
        }

        if (metalWallSpawnPositions == null || wall_1x1 == null)
        {
            Debug.LogError("PositionData or objectPrefab is not assigned!");
            return;
        }

        foreach (Vector3 position in metalWallSpawnPositions.positions)
        {
            Instantiate(wall_1x1, position, Quaternion.identity);
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
        currentEnemiesSpawnedCount = 0;
        tankController = GameObject.FindGameObjectWithTag("Tank").GetComponent<TankController>();
        while (currentEnemiesSpawnedCount < maxEnemyCount)
        {
            if (tankController == null)
            {
                break;
            }
            
            Vector3 enemySpawnLoc;
            do
            {
                enemySpawnLoc = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), 0);
            } while (IsSpawnLocationInsideFrustum(enemySpawnLoc, mainCamera, Mathf.Abs(mainCamera.transform.position.z)));
            for (int i = 0; i < enemyClusterCount; i++)
            {
                Instantiate(enemy, enemySpawnLoc, Quaternion.identity);
                currentEnemiesSpawnedCount++;
            }

            yield return new WaitForSeconds(enemySpawnInterval);
        }
    }
    bool IsSpawnLocationInsideFrustum(Vector3 spawnLocation, Camera camera, float cameraDistance)
    {
        Vector3 viewportPoint = camera.WorldToViewportPoint(spawnLocation);

        // Check if the spawn location is inside the camera's frustum
        return viewportPoint.x >= 0 && viewportPoint.x <= 1 &&
               viewportPoint.y >= 0 && viewportPoint.y <= 1 &&
               viewportPoint.z >= 0 && viewportPoint.z <= cameraDistance;
    }

    void GateOpener()
    {
        
        zombiesInstantiated = GameObject.FindGameObjectsWithTag("Enemy");
        int zombieCount = zombiesInstantiated.Length;
        if (currentEnemiesSpawnedCount >= maxEnemyCount && zombieCount == 0)
        {
            gateLeft.transform.position = new Vector3(-8f, gateLeft.transform.position.y, 0);
            gateRight.transform.position = new Vector3(8f, gateRight.transform.position.y, 0);
            roundComplete = true;
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

    void KillMines()
    {
        mines = GameObject.FindGameObjectsWithTag("Mine");
        for (int i = 0; i < mines.Length; i++)
        {
            Destroy(mines[i]);
        }
    }



}
