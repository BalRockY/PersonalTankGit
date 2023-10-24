using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
   
    private Vector3 mousePosWorld;
    private Camera mainCam;
    private AudioSource aSourceShoot;
    private AudioSource aSourceTurretMove;
    public AudioClip gunShot;

    public float turretRotationSpeedFactor;
    
    //public Transform target;

    private List<Transform> gunfireList;
    private GameObject gunfireHolder;
    private Transform gunfire1;
    private Transform gunfire2;
    private Transform gunfire3;
    private Transform gunfire4;

    public float interval;

    private int shotSpriteCount = 0;

    public int shotVolleyCount = 1;

    public float volleyFiringSpeed = 0.2f;
    public float reloadSpeed = 2;
    private bool shooting = false;

    // Gun flash lighting

    public GameObject gunflashLeft;
    public GameObject gunflashBackingLeft;
    public GameObject gunflashRight;
    public GameObject gunflashBackingRight;

    public float lightTime;

    [SerializeField]
    GameObject projectile;

    [SerializeField]
    Transform firepointL;

    [SerializeField]
    Transform firepointR;

    // Turret a-clips
    private AudioClip turretStart;
    private AudioClip turretMid;
    private AudioClip turretEnd;
    private AudioClip alternativeTurretMovement;

    //For calculating turret rotation
    private float previousRotation;
    //For calculating turret speed
    private float previousTime;

    private float m_rotationSpeed;








    private void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        // gunlight disable from start
        gunflashLeft.SetActive(false);
        gunflashBackingLeft.SetActive(false);
        gunflashRight.SetActive(false);
        gunflashBackingRight.SetActive(false);

        aSourceShoot = GetComponent<AudioSource>();
        aSourceTurretMove = this.gameObject.transform.GetChild(3).GetComponent<AudioSource>();

        gunfireHolder = GameObject.Find("GunfireHolder");
        gunfire1 = gunfireHolder.gameObject.transform.GetChild(0);
        gunfire2 = gunfireHolder.gameObject.transform.GetChild(1);
        gunfire3 = gunfireHolder.gameObject.transform.GetChild(2);
        gunfire4 = gunfireHolder.gameObject.transform.GetChild(3);

        gunfireList = new List<Transform>();

        gunfireList.Add(gunfire1);
        gunfireList.Add(gunfire2);
        gunfireList.Add(gunfire3);
        gunfireList.Add(gunfire4);

        turretStart = AudioManager.Instance.turretStart;
        turretMid = AudioManager.Instance.turretMid;
        turretEnd = AudioManager.Instance.turretEnd;
        alternativeTurretMovement = AudioManager.Instance.turretRotation;

        aSourceTurretMove.clip = alternativeTurretMovement;


        // Store the initial rotation value and time
        previousRotation = transform.rotation.eulerAngles.z;
        previousTime = Time.time;
    }

    IEnumerator ShowGunshotSprite(float seconds)
    {
        
        gunfireList[shotSpriteCount].gameObject.SetActive(true);

        yield return new WaitForSeconds(seconds);
            
            if (gameObject.activeInHierarchy)
            gunfireList[shotSpriteCount].gameObject.SetActive(false);

        shotSpriteCount++;

        if (shotSpriteCount >= gunfireList.Count)
        {
            shotSpriteCount = 0;
        }
        

    }
    
    IEnumerator MachinegunFire()
    {
        shooting = true;

        for (int i = 0; i < shotVolleyCount; i++)
        {

            
            StartCoroutine(LightTime(lightTime));
            CameraShake.Shake(0.05f, 0.1f);

            aSourceShoot.clip = gunShot;
            aSourceShoot.pitch = Random.Range(0.95f, 1.05f);

            aSourceShoot.Play();            

            StartCoroutine(ShowGunshotSprite(interval));
            Instantiate(projectile, firepointL.position, transform.rotation);
            Instantiate(projectile, firepointR.position, transform.rotation);

            
            yield return new WaitForSeconds(volleyFiringSpeed);

            
        }

        //yield return new WaitForSeconds(reloadSpeed); //extra interval between volleys
        shooting = false;
        Debug.Log("Machinegun fired");
    }

    IEnumerator LightTime(float lt)
    {
        gunflashLeft.SetActive(true);
        gunflashBackingLeft.SetActive(true);
        gunflashRight.SetActive(true);
        gunflashBackingRight.SetActive(true);
        yield return new WaitForSeconds(lt);
        gunflashLeft.SetActive(false);
        gunflashBackingLeft.SetActive(false);
        gunflashRight.SetActive(false);
        gunflashBackingRight.SetActive(false);
    }

    void ShotCount()
    {
        
        if (Input.GetKey(KeyCode.Mouse0) && shooting == false)
        {
            
            StartCoroutine(MachinegunFire());

        }

    }

    private void Update()
    {
        

        ShotCount();

        // Orthographic view
        /*mousePosWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
        
        Vector3 targetPosition = mousePosWorld;

        Vector3 dir = targetPosition - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, turretRotationSpeedFactor * Time.deltaTime);
        */

        // Perspective view
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPosition = hit.point;

            Vector3 dir = targetPosition - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, turretRotationSpeedFactor * Time.deltaTime);
        }

        //PlayTurretMovementSound();

    }
    


    void PlayTurretMovementSound()
        {
        // Get the current rotation value and time
        float currentRotation = transform.rotation.eulerAngles.z;
        float currentTime = Time.time;

        // Calculate the difference in rotation and time
        float rotationDifference = Mathf.DeltaAngle(previousRotation, currentRotation) * Mathf.Deg2Rad;
        float timeDifference = currentTime - previousTime;

        // Calculate the rotation speed
        float rotationSpeed = rotationDifference / timeDifference;
        m_rotationSpeed = Mathf.Abs(rotationSpeed);

        // Determine the rotation direction
        //RotationDirection direction = GetRotationDirection(rotationDifference);

        // Store the current rotation and time as the previous values for the next frame
        previousRotation = currentRotation;
        previousTime = currentTime;
        aSourceTurretMove.pitch = 0.8f + 0.03f * m_rotationSpeed;

        if (m_rotationSpeed >= 0.5f)
        {
            if(turretStartBool == false)
            {
                aSourceTurretMove.clip = turretStart;
                //aSourceTurretMove.pitch = 1f;
                aSourceTurretMove.Play();
                turretStartBool = true;
                turretEndBool = false;
                turretMidBool = false;
            }
            else if (turretMidBool == false && aSourceTurretMove.time == turretStart.length)
            {
                aSourceTurretMove.clip = turretMid;
                //aSourceTurretMove.pitch = 0.96f;
                aSourceTurretMove.Play();
            }
            else if(aSourceTurretMove.time == turretMid.length)
            {
                aSourceTurretMove.clip = turretMid;
                //aSourceTurretMove.pitch = 0.96f;
                aSourceTurretMove.Play();
            }
        }
        else
        {
            if(turretEndBool == false)
            {
                aSourceTurretMove.clip = turretEnd;
                //aSourceTurretMove.pitch = 1f;
                aSourceTurretMove.Play();
                turretEndBool = true;
                turretStartBool = false;
                turretMidBool = false;
            }
        }

    }


    public bool turretStartBool = false;
    public bool turretEndBool = false;
    public bool turretMidBool = false;

}
