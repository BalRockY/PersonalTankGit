using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    // Navigation Variables
    private Vector3 target;
    NavMeshAgent agent;
    private GameObject player;
    [SerializeField] private float rotationSpeed;
    private float distanceToPlayer;
    [SerializeField] private float currentAgentSpeed;
    public float attractionWeight = 1.0f;
    public float repulsionWeight = 1.0f;
    public float repulsionRadius = 2.0f;


    public GameObject selectedUI;

    // Tank Variables
    private TankController tankScript;
    private Transform tank;




    // Sprite Variables
    private SpriteRenderer sprite;
    private GameObject splatAnimGO;


    // Audio Variables
    private AudioSource aSource;
    [SerializeField] private AudioSource aSource2;
    private AudioClip[] bulletHitFleshSounds;

    private bool killZombieHasRun = false;

    private new BoxCollider2D collider2D;


    [SerializeField]
    private float attckDmg;
    [SerializeField]
    public float hp;
    [SerializeField]
    float agentSpeed = 1.5f;
    [SerializeField]
    float minKillSpeed = 3f;
    [SerializeField]
    float minStunSpeed = 1.5f;
    [SerializeField]
    float upClosePushSpeed = 0.1f;
    [SerializeField]
    float upCloseDistance = 1.0f;
    [SerializeField]
    float vehichleDmg = 8f;


    public bool isStunned = false;
    public bool isCloseToPlayer = false;
    public bool isTouchingPlayer = false;

    public int exp = 15;


    private void Awake()
    {
        // Setup Player Reference
        player = GameObject.FindGameObjectWithTag("Tank");
        tankScript = player.GetComponent<TankController>();
        tank = player.transform;

        // Setup Navigation
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        // Setup Collision
        collider2D = this.GetComponent<BoxCollider2D>();

        // Setup Sprite References        
        splatAnimGO = Resources.Load<GameObject>("VFX/SplatAnimation");
        sprite = this.gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>();
        
        // Setup Aduio References
        aSource = this.gameObject.GetComponent<AudioSource>();
        bulletHitFleshSounds = AudioManager.Instance.bulletHitFleshClips;
}
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        //CalculateMovement();
        SetTargetPosition();
        SetAgentPosition();
        SetAgentDirection();
        SetAgentSpeed();

        TakeDamgeWhileStunned();
        if (hp <= 0 && killZombieHasRun == false)
        {
            KillZombie();
        }

        // Render/Unrender Sprite based on distance to player
        var tankDistance = Mathf.Sqrt(Mathf.Pow(tank.transform.position.x - transform.position.x, 2) + Mathf.Pow(tank.transform.position.y - transform.position.y, 2));
        if (tankDistance < GameManager.Instance.renderDistance && killZombieHasRun == false) sprite.enabled = true;
        else sprite.enabled = false;
    }

    // Activate Selection UI
    public void Select()
    {
        selectedUI.SetActive(!selectedUI.activeSelf); //Selectionbox - unused for now
    }


    // Damage Functions
    void TakeDamgeWhileStunned()
    {
        if (isStunned)
        {
            hp -= (vehichleDmg * Time.deltaTime);
        }
    }
    void TakeDmg(float incomingdmg)
    {
        hp -= incomingdmg;
        int randomnumber = Random.Range(0, 2);
        aSource2.PlayOneShot(bulletHitFleshSounds[randomnumber]);
        

        /*if(isStunned == true && hp <= 0 && killZombieHasRun == false) // this was to play splat sound only when run over, but it feels weird not having it when killed normally
{                                                               // so until better death sounds are found for other death scenarios, this code is disabled.
    //AUDIO MANAGER IS PLAYING THE SOUND 
    AudioManager.Instance.aSourceAM.volume = 0.04f;
    AudioManager.Instance.aSourceAM.PlayOneShot(AudioManager.Instance.splatSound);
    KillZombie();
}
else if (hp <= 0 && killZombieHasRun == false)
{
    KillZombie();
}*/
    }
    void KillZombie()
    {
        //Splatsound
        aSource.volume = 0.1f;
        aSource.PlayOneShot(AudioManager.Instance.splatSound);

        killZombieHasRun = true;
        attckDmg = 0;
        collider2D.isTrigger = true; //these are so that the the bullet hit sound get time to play before gameobject is destroyed, but enemy is still technically dead.
        sprite.enabled = false;
        PlayerManager.Instance.PlayerKill();
        //PlayerManager.Instance.GainEXP(exp);
        Instantiate(splatAnimGO, this.transform.position, Quaternion.identity);
        //RoundManager.Instance.MoneySpawn(this.transform.position); //Removed Money pickup for now.
        Destroy(this.gameObject/*, AudioManager.Instance.splatSound.length*/); // Wait for longest bullethitflesh clip, which is bulletHitFlesh1
    }

    // Collision Triggers
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("agent hit: " + collision.gameObject);
        if (collision.gameObject.tag == "Tank")
        {
            var chassisHitDmg = tankScript.speed * tankScript.speedAsDmgMultiplier;
            
            hp -= chassisHitDmg;
            if (/*tankScript.speed <= minKillSpeed && */tankScript.speed > minStunSpeed)
            {
                isStunned = true;
                //StartCoroutine(StunWait(4));
            }
            /*else if (tankScript.speed > minKillSpeed && killZombieHasRun == false)
            {
                KillZombie();
            }*/
        }
        else if(collision.gameObject.tag == "Projectile" && killZombieHasRun == false)
        {
            TakeDmg(tankScript.dmg);

        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Collision")
        {
            isTouchingPlayer = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ZombieAttCol"))
        {
            if (!isStunned)
            {
                PlayerManager.Instance.PlayerHit(attckDmg * Time.deltaTime);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("LeaveStunnedZombie"))
        {
            isStunned = false;
        }
    }

    // Navigation Functions
    void SetTargetPosition()
    {
        if(PlayerManager.Instance.insideVehicle == false)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Tank");
        }
        target = player.transform.position;
    }
    void SetAgentPosition()
    {
        // NavMesh:
        agent.SetDestination(new Vector3(target.x, target.y, transform.position.z));

        // Homemade AI:
        /*distanceToPlayer = Vector2.Distance(transform.position, target);
        transform.position = Vector2.MoveTowards(this.transform.position, target, currentAgentSpeed * Time.deltaTime);*/
    }

    void SetAgentDirection()
    {
        Vector3 targ = target;
        targ.z = 0f;
        Vector3 objectPos = transform.position;
        targ.x = targ.x - objectPos.x;
        targ.y = targ.y - objectPos.y;
        float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle+90f));
        
    }

    void CalculateMovement()
    {
        SetTargetPosition();

        // Calculate attraction vector to player
        Vector3 attractionVector = player.transform.position - transform.position;

        // Calculate repulsion vectors from other zombies
        Vector3 repulsionVector = Vector3.zero;
        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject otherZombieObject in zombies)
        {
            if (otherZombieObject != gameObject)
            {
                float distance = Vector3.Distance(transform.position, otherZombieObject.transform.position);
                if (distance < repulsionRadius)
                {
                    repulsionVector += (transform.position - otherZombieObject.transform.position).normalized / distance;
                }
            }
        }

        // Weight the vectors
        attractionVector *= attractionWeight;
        repulsionVector *= repulsionWeight;

        // Combine vectors
        Vector3 resultantVector = attractionVector + repulsionVector;

        // Normalize the resultant vector
        Vector3 moveDirection = resultantVector.normalized;

        // Update zombie rotation based on the desired direction
        SetAgentDirection();

        // Update zombie speed based on distance
        SetAgentSpeed();

        // Update zombie position based on the desired direction and speed
        //SetAgentPosition(moveDirection);
    }
    void SetAgentSpeed()
    {
        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

        if (isStunned == true)
        {
            currentAgentSpeed = 0;
            collider2D.isTrigger = true;
        }
        else if (isStunned == false)
        {
            if (distanceToPlayer < upCloseDistance && !isTouchingPlayer)
            {
                isCloseToPlayer = true;

                currentAgentSpeed = upClosePushSpeed;
            }
            /*else if(isTouchingPlayer == true) // vil gerne s�tte speed til 0 for at undg� at skubbe spilleren ud af kortet. Virker ikke lige nu
            {
                currentAgentSpeed = 0;
            }*/
            else
            {
                collider2D.isTrigger = false;
                currentAgentSpeed = agentSpeed;
            }

        }
        agent.speed = currentAgentSpeed;
    }
}
