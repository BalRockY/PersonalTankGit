using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{
    private GameObject tank;

    [Header("Drive settings")]
    public float driftFactor;
    public float accelerationFactor;
    public float turnFactor;
    public float decelerationDrag;
    public float maxSpeed;
    public float reverseMaxSpeedFactor;
    

    [Header("Local Variables")]
    float accelerationInput = 0;
    float steeringInput = 0;
    float rotationAngle = 0;
    float velocityVsUp = 0;

    public float speed;
    private bool keepMoving = false;
    public AudioSource aSource;
    public AudioClip deathExplosion;
    private ParticleSystem ps;
    private Collider2D collider;

    [SerializeField] private AudioListener _aListener;

    public bool dead = false;
    [SerializeField]
    float dieTime;

    [SerializeField]
    private bool movingFwd = false;
    [SerializeField]
    private bool movingBck = false;

    public Vector3 LastPOS;
    public Vector3 NextPOS;

    public Vector2 currentPos;

    [Header("Stats")]
    public float hp = 10;
    public float dmg = 10;
    public float speedAsDmgMultiplier;

    public Animator animator;

    private GameObject landmine;

    Rigidbody2D tankRB2D;

    bool driveKeyHeld = false;

    // Audio Clip References
    private AudioClip driving;
    private AudioClip idling;


    private void Awake()
    {
        _aListener = this.GetComponent<AudioListener>();
        tank = this.gameObject;
        collider = transform.Find("Collision").gameObject.GetComponent<EdgeCollider2D>();
        aSource = this.gameObject.GetComponent<AudioSource>();
        tankRB2D = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        StartCoroutine(SpeedCalculation());
        ps = GameObject.Find("TankExplosion").GetComponent<ParticleSystem>();
        landmine = Resources.Load<GameObject>("LandMine");
        
    }
    private void Start()
    {
        ps.Stop();
        aSource.volume = 0.15f;

        //Audio Source setup
        aSource.loop = true;
        driving = AudioManager.Instance.driveMotor;
        idling = AudioManager.Instance.idleMotor;
        aSource.clip = idling;
        aSource.Play();
    }

    public void SetAudioListener(bool boolSetter)
    {
        _aListener.enabled = boolSetter;
    }

    public IEnumerator Die(float dietime)
    {
        aSource.volume = 1f;
        aSource.PlayOneShot(deathExplosion);
        ps.Play();
            yield return new WaitForSeconds(dietime);
        ps.Stop();
            GameManager.Instance.UpdateGameState(GameState.RoundOver);
            
    }
    private void Update()
    {
        //UIManager.Instance.hp = hp;
        //if (hp <= 0)
        //{
        //    if(dead == false)
        //    {
        //        dead = true;
        //        StartCoroutine(Die(dieTime));
        //    }
        //}

        PlayAnimation();
        PlayDriveSounds();
        SpawnLandMine();

        /*
        if (speed > 0)
        {
            movingFwd = true;

        }
            animator.SetInteger("speed", this.speed);
        else animator.SetBool("notMoving", true);

        if (accelerationInput < 0 && speed > 0.5)
            animator.SetInteger("speed", -this.speed);
        else animator.SetBool("notMoving", true);
        */

        /*if (movingFwd)
            animator.SetBool("movingForward", true);
        animator.SetBool("movingBackwards", false);

        if (movingBck)
            animator.SetBool("movingBackwards", true);
        animator.SetBool("movingForward", false);
        */

        /*if (this.speed == 0)
            animator.SetBool("notMoving", true);
        else animator.SetBool("notMoving", false);*/
    }

    private void FixedUpdate()
    {  


        ApplyEngineForce();
        ApplySteering();
        KillOrthogonalVelocity();
        
    }

    void SpawnLandMine()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(landmine, new Vector3(tank.transform.position.x, tank.transform.position.y, 0), tank.transform.rotation);
        }
    }

    void PlayAnimation()
    {
        Vector2 localVel = transform.InverseTransformDirection(tankRB2D.velocity); //Local retning: y = frem eller tilbage, + eller -

        if (localVel.y > 0.1)
        {
            movingFwd = true;
            movingBck = false;
            animator.SetFloat("speed", localVel.y);
            animator.SetBool("notMoving", false);
        }
        else if (localVel.y < -0.1)
        {
            movingFwd = false;
            movingBck = true;
            animator.SetFloat("speed", localVel.y);
            animator.SetBool("notMoving", false);
        }
        else if(localVel.y <= 0.1 && localVel.y >= -0.1)
        {
            movingFwd = false;
            movingBck = false;
            animator.SetBool("notMoving", true);
        }

        /*
        if (movingFwd == true && speed > 0.5)
        {
            animator.SetInteger("speed", speed);
        }
        else if (movingBck == true && speed > 0.5)
        {
            animator.SetInteger("speed", speed);
        }
        else
        {
            animator.SetBool("notMoving", true);
        }
        */
    }

    // Movement Functions
    void PlayDriveSounds()
    {
        aSource.pitch = 1 + 0.1f * speed * Random.Range(0.8f, 1.2f);
        
        if (Mathf.Abs(accelerationInput) == 1f)
        {
            if (aSource.clip == idling)
            {
                aSource.clip = driving;
                aSource.Play();
            }

        }
        else
        {
            if (aSource.clip == driving)
            {
                aSource.clip = idling;
                aSource.Play();
            }

        }
        
    }

    IEnumerator PlayEngineDriveSound()
    {

        yield return new WaitForSeconds(AudioManager.Instance.driveMotor.length);
    }
    IEnumerator SpeedCalculation()
    {
        bool isPlaying = true;

        while (isPlaying)
        {
            Vector3 prevPos = transform.position;

            yield return new WaitForFixedUpdate();

            speed = Mathf.Abs(Vector3.Distance(transform.position, prevPos) / Time.fixedDeltaTime);
        }
    }
    void ApplyEngineForce()
    {
        velocityVsUp = Vector2.Dot(transform.up, tankRB2D.velocity);

        /*if (velocityVsUp > 0 && accelerationInput < 0)
            tankRB2D.drag = Mathf.Lerp(tankRB2D.drag, decelerationDrag, Time.fixedDeltaTime * 3);
        if (velocityVsUp < 0 && accelerationInput > 0)
            tankRB2D.drag = Mathf.Lerp(tankRB2D.drag, decelerationDrag, Time.fixedDeltaTime * 3);*/ //Vil gerne apply drag når man kører frem, men prøver at bakke. 
                                                                                                    //Denne kode gør ikke umiddelbart tricket.

        if (velocityVsUp > maxSpeed && accelerationInput > 0)
            return;

        if (velocityVsUp < -maxSpeed * reverseMaxSpeedFactor && accelerationInput < 0)
            return;

        if (tankRB2D.velocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0)
            return;

        if (accelerationInput == 0)
            tankRB2D.drag = Mathf.Lerp(tankRB2D.drag, decelerationDrag, Time.fixedDeltaTime * 3);
        else if (velocityVsUp > 0 && accelerationInput < 0)
            tankRB2D.drag = Mathf.Lerp(tankRB2D.drag, decelerationDrag, Time.fixedDeltaTime * 3);
        else if (-velocityVsUp > 0 && accelerationInput > 0)
            tankRB2D.drag = Mathf.Lerp(tankRB2D.drag, decelerationDrag, Time.fixedDeltaTime * 3);
        else tankRB2D.drag = 0;

        



        Vector2 engineForceVector = transform.up * accelerationInput * accelerationFactor;

        tankRB2D.AddForce(engineForceVector, ForceMode2D.Force);
    }

    void ApplySteering()
    {
        //Quaternion targetRotation = tankRB2D.transform.rotation;

        rotationAngle -= steeringInput * turnFactor;

        //rotationAngleBackwards += steeringInput * turnFactor;

        
            tankRB2D.MoveRotation(rotationAngle);
        


        /*if (velocityReverseRotation < 0)
        {
            tankRB2D.MoveRotation(-rotationAngle);
        }*/
        
    }

    void KillOrthogonalVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(tankRB2D.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(tankRB2D.velocity, transform.right);

        tankRB2D.velocity = forwardVelocity + rightVelocity * driftFactor;
    }

    // Input Functions

    public void SetInputVector(Vector2 inputVector)
    {
        /*if (velocityVsUp > 0 || accelerationInput > 0)
            steeringInput = inputVector.x;
        else if (velocityVsUp < 0 || accelerationInput < 0)*/
            steeringInput = inputVector.x;  // sæt inputVector til minus hvis du vil have omvendt baglæns styring.

        accelerationInput = inputVector.y;

    }

    // Collision Functions

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Money"))
        {
            AudioManager.Instance.aSourceAM2.volume = 0.1f;
            AudioManager.Instance.aSourceAM2.PlayOneShot(AudioManager.Instance.moneyPickUpSound);
            
        }
        if(collision.gameObject == RoundManager.Instance.caravanImage)
        {

            RoundManager.Instance.triggerEndRound = true;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            aSource.pitch = Random.Range(0.92f, 1.08f);
            if(speed >= 2.5)
                aSource.PlayOneShot(AudioManager.Instance.zombieHitCarClips[1]);
            else if(speed < 2.5)
            {
                aSource.PlayOneShot(AudioManager.Instance.zombieHitCarClips[0]);
            }
        }
    }

}
