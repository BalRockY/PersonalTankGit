using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wallController : MonoBehaviour
{
    private AudioSource aSource;
    private AudioClip[] bulletHitWallSounds;
    private void Awake()
    {
        aSource = this.gameObject.GetComponent<AudioSource>();
        bulletHitWallSounds = AudioManager.Instance.bulletHitMetalClips;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Projectile")
        {
            int randomnumber = Random.Range(0, 5);
            aSource.volume = 0.5f;
            aSource.PlayOneShot(bulletHitWallSounds[randomnumber]);
            //Debug.Log("aclip played: " + bulletHitWallSounds[randomnumber].name);
        }
    }
    
}
