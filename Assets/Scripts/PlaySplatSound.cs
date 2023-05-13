using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySplatSound : MonoBehaviour
{
    private AudioSource aSource;
    private AudioClip splat;
    private AudioClip wetsplat;
    private AudioClip death1;
    private AudioClip[] bulletHitFleshSounds;
    private SpriteRenderer sprRendAnim;
    private SpriteRenderer sprRendBloodPool;
    private Transform tank;



    private void Awake()
    {
        aSource = this.gameObject.GetComponent<AudioSource>();
        splat = AudioManager.Instance.splatSound;
        wetsplat = AudioManager.Instance.wetSplat;
        death1 = AudioManager.Instance.death1;
        sprRendAnim = this.gameObject.GetComponent<SpriteRenderer>();
        sprRendBloodPool = this.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        tank = GameObject.FindGameObjectWithTag("Tank").transform;
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
        var tankDistance = Mathf.Sqrt(Mathf.Pow(tank.transform.position.x - transform.position.x, 2) + Mathf.Pow(tank.transform.position.y - transform.position.y, 2));

        if (tankDistance < gameManager.Instance.renderDistance)
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
