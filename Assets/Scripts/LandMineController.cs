using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMineController : MonoBehaviour
{
    private AudioSource aSource;
    private AudioSource aSource2;
    private AudioSource aSource3;
    private AudioClip beepSound;
    public Animator landmineAnimator;
    private AnimatorClipInfo[] currentAnimationClip;
    private float currentAnimationClipLength;
    private AudioClip mineExplosionSound;
    private AudioClip deployMineSound;
    private SpriteRenderer sprite;
    private CircleCollider2D collider;
    private EnemyController enemyController;

    [SerializeField]
    private float mineKillDistance;

    private void Awake()
    {
        aSource = this.gameObject.GetComponent<AudioSource>();
        aSource2 = this.gameObject.transform.GetChild(0).GetComponent<AudioSource>();
        aSource3 = this.gameObject.transform.GetChild(1).GetComponent<AudioSource>();
        beepSound = AudioManager.Instance.beep;
        landmineAnimator = this.gameObject.GetComponent<Animator>();
        mineExplosionSound = AudioManager.Instance.mineExplosion;
        deployMineSound = AudioManager.Instance.mineDeploy;
        sprite = this.gameObject.GetComponent<SpriteRenderer>();
        collider = this.gameObject.GetComponent<CircleCollider2D>();
        
    }

    private void Start()
    {
        aSource3.volume = 0.3f;
        aSource3.PlayOneShot(deployMineSound);
        StartCoroutine(PlayBeepSound());
    }

    IEnumerator PlayBeepSound()
    {
        aSource.clip = beepSound;
        currentAnimationClip = this.landmineAnimator.GetCurrentAnimatorClipInfo(0);
        currentAnimationClipLength = currentAnimationClip[0].clip.length;
        yield return new WaitForSeconds(currentAnimationClipLength);
        aSource.PlayOneShot(beepSound);
        StartCoroutine(PlayBeepSound());
    }
    IEnumerator DestroyLandMine()
    {
        currentAnimationClip = this.landmineAnimator.GetCurrentAnimatorClipInfo(0);
        currentAnimationClipLength = currentAnimationClip[0].clip.length;
        sprite.sortingOrder = 3;

        //First wait for previous animation to finish
        yield return new WaitForSeconds(currentAnimationClipLength);

        //State info
        AnimatorStateInfo stateInfo = landmineAnimator.GetCurrentAnimatorStateInfo(0);

        // Calculate the remaining time
        float remainingTime = stateInfo.length - stateInfo.normalizedTime * stateInfo.length;

        yield return new WaitForSeconds(remainingTime);
        Destroy(this.gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // Make sure no other zombies trigger the landmine
            collider.enabled = false;

            // Triggers explosion animation
            landmineAnimator.SetTrigger("ZombieTrigger");
            
            GameObject[] enemiesNearPointZero = GameObject.FindGameObjectsWithTag("Enemy");
            
            foreach(GameObject enemy in enemiesNearPointZero)
            {
                float distanceToExplosion = Vector3.Distance(this.transform.position, enemy.transform.position);
                if(distanceToExplosion < mineKillDistance)
                {
                    enemy.gameObject.GetComponent<EnemyController>().hp -= 100;
                }
            }
            aSource2.PlayOneShot(mineExplosionSound);
            StartCoroutine(DestroyLandMine());
        }
    }
}
