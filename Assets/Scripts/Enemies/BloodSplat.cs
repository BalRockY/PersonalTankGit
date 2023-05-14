using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSplat : MonoBehaviour
{
    // Audio Variables
    private AudioSource aSource;
    private AudioClip splat;
    private AudioClip wetsplat;
    private AudioClip death1;
    private AudioClip[] bulletHitFleshSounds;

    // Sprite Variables
    private SpriteRenderer sprRendAnim;
    private SpriteRenderer sprRendBloodPool;
    private Transform tank;



    private void Awake()
    {
        // Setup Tank Reference
        tank = GameObject.FindGameObjectWithTag("Tank").transform;

        // Setup Sprite References
        sprRendAnim = this.gameObject.GetComponent<SpriteRenderer>();
        sprRendBloodPool = this.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();

        // Setup Audio References
        splat = AudioManager.Instance.splatSound;
        wetsplat = AudioManager.Instance.wetSplat;
        death1 = AudioManager.Instance.death1;
        aSource = this.gameObject.GetComponent<AudioSource>();
        bulletHitFleshSounds = AudioManager.Instance.bulletHitFleshClips;
    }


    private void Start()
    {
        //aSource.PlayOneShot(wetsplat);
        //aSource.PlayOneShot(splat);
        //aSource.PlayOneShot(bulletHitFleshSounds[Random.Range(0, 2)]);
        aSource.PlayOneShot(death1);
    }

    private void Update()
    {
        // Render/Unrender Sprite based on distance to player
        var tankDistance = Mathf.Sqrt(Mathf.Pow(tank.transform.position.x - transform.position.x, 2) + Mathf.Pow(tank.transform.position.y - transform.position.y, 2));
        if (tankDistance < GameManager.Instance.renderDistance)
        {
            sprRendAnim.enabled = true;
            sprRendBloodPool.enabled = true;
        }
        else
        {
            sprRendAnim.enabled = false;
            sprRendBloodPool.enabled = false;
        }
    }
}
