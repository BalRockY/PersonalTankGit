using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
   
    private Vector3 mousePosWorld;
    private Camera mainCam;
    private AudioSource audioSource;
    public AudioClip gunShot;

    public float speed;
    
    //public Transform target;

    private List<Transform> gunfireList;
    private GameObject gunfireHolder;
    private Transform gunfire1;
    private Transform gunfire2;
    private Transform gunfire3;
    private Transform gunfire4;

    public float interval;

    private int shotSpriteCount = 0;

    public int shotVolleyCount = 5;

    public float volleyFiringSpeed = 0.2f;
    public float reloadSpeed = 2;
    private bool shooting = false;

    [SerializeField]
    GameObject projectile;

    [SerializeField]
    Transform firepointL;

    [SerializeField]
    Transform firepointR;

    

    
        



    private void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        audioSource = GetComponent<AudioSource>();

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
            
            audioSource.clip = gunShot;
            audioSource.pitch = Random.Range(0.95f, 1.05f);

            audioSource.Play();

            

            StartCoroutine(ShowGunshotSprite(interval));
            Instantiate(projectile, firepointL.position, transform.rotation);
            Instantiate(projectile, firepointR.position, transform.rotation);

            
            yield return new WaitForSeconds(volleyFiringSpeed);


        }

        yield return new WaitForSeconds(reloadSpeed); //extra interval between volleys
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
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, speed * Time.deltaTime);


    }


}
