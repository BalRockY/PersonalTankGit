using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentMovement : MonoBehaviour
{
    
    private Vector3 target;
    NavMeshAgent agent;
    private GameObject player;
    
    public GameObject selectedUI;
    private GameObject agentObject;
    private TankController tankScript;
    private new BoxCollider2D collider2D;
    private float currentAgentSpeed;
    private GameObject splatAnimGO;

    private SpriteRenderer sprite;
    private Transform tank;
    private AudioSource aSource;
    private AudioClip[] bulletHitFleshSounds;

    private bool killZombieHasRun = false;


    [SerializeField]
    private float attckDmg;
    [SerializeField]
    private float hp;
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



    private void Awake()
    {
        
        player = GameObject.FindGameObjectWithTag("Tank");
        tankScript = player.GetComponent<TankController>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agentObject = this.gameObject;
        collider2D = agentObject.GetComponent<BoxCollider2D>();
        splatAnimGO = Resources.Load<GameObject>("SplatAnimation");
        sprite = this.gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>();
        tank = player.transform;
        aSource = this.gameObject.GetComponent<AudioSource>();
        bulletHitFleshSounds = AudioManager.Instance.bulletHitFleshClips;
}
    void Start()
    {
        
    }

    // Activate Selection UI
    public void Select()
    {
        selectedUI.SetActive(!selectedUI.activeSelf); //Selectionbox - unused for now
    }


    // Update is called once per frame
    void Update()
    {
        
        SetTargetPosition();
        SetAgentPosition();
        SetAgentSpeed();
        TakeDamgeWhileStunned();
        ZombieDeath();
        var tankDistance = Mathf.Sqrt(Mathf.Pow(tank.transform.position.x - transform.position.x, 2) + Mathf.Pow(tank.transform.position.y - transform.position.y, 2));

        if (tankDistance < gameManager.Instance.renderDistance && killZombieHasRun == false)
        {
            sprite.enabled = true;

        }
        else
        {
            sprite.enabled = false;
        }
    }

    void TakeDamgeWhileStunned()
    {
        if (isStunned)
        {
            hp -= (vehichleDmg * Time.deltaTime);
        }
    }

    void ZombieDeath()
    {
        if(hp <= 0 && killZombieHasRun == false)
        {
            KillZombie();
        }
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("ZombieAttCol"))
        {
            if(!isStunned)
            {
                PlayerManager.Instance.playerhit(attckDmg * Time.deltaTime);
            }
        }
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
            /*else if(isTouchingPlayer == true) // vil gerne sætte speed til 0 for at undgå at skubbe spilleren ud af kortet. Virker ikke lige nu
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

    void KillZombie()
    {
        //AUDIO MANAGER IS PLAYING THE SOUND 
        AudioManager.Instance.aSourceAM.volume = 0.04f;
        AudioManager.Instance.aSourceAM.PlayOneShot(AudioManager.Instance.splatSound);

        killZombieHasRun = true;
        attckDmg = 0;
        collider2D.isTrigger = true; //these are so that the the bullet hit sound get time to play before gameobject is destroyed, but enemy is still technically dead.
        sprite.enabled = false;
        PlayerManager.Instance.playerkill();
        Instantiate(splatAnimGO, this.transform.position, Quaternion.identity);
        SpawnManager.Instance.MoneySpawn(this.transform.position);
        Destroy(agentObject, AudioManager.Instance.bulletHitFleshClips[0].length); // Wait for longest bullethitflesh clip, which is bulletHitFlesh1
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("agent hit: " + collision.gameObject);
        if (collision.gameObject.tag == "Tank")
        {
            var chassisHitDmg = tankScript.speed * tankScript.speedAsDmgMultiplier;
            Debug.Log(chassisHitDmg);
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
            hp -= tankScript.dmg;
            int randomnumber = Random.Range(0, 2);
            aSource.volume = 0.3f;
            aSource.PlayOneShot(bulletHitFleshSounds[randomnumber]);
            Debug.Log("aclip played: " + bulletHitFleshSounds[randomnumber].name);
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Collision")
        {
            isTouchingPlayer = true;
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("LeaveStunnedZombie"))
        {
            Debug.Log("Tank left");
            isStunned = false;
        }
    }

    void SetTargetPosition()
    {
        target = player.transform.position;
    }
    void SetAgentPosition()
    {
        agent.SetDestination(new Vector3(target.x, target.y, transform.position.z));
    }
}
