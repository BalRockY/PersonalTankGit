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

    public float turretRotationSpeed;
    
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









    private void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

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

        mousePosWorld = mainCam.ScreenToWorldPoint(Input.mousePosition);
        
        Vector3 targetPosition = mousePosWorld;

        Vector3 dir = targetPosition - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, turretRotationSpeed * Time.deltaTime);
        
        PlayTurretMovementSound();

    }
    private Quaternion oldRot;

    void PlayTurretMovementSound()
    {
        if (oldRot != transform.rotation)
        {
            aSourceTurretMove.clip = turretStart;
            aSourceTurretMove.Play();

        }
            
        else
        {
            aSourceTurretMove.Stop();
        }
            
        oldRot = transform.rotation;
        
    }

}
