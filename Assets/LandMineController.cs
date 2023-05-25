using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMineController : MonoBehaviour
{
    private AudioSource aSource;
    private AudioSource aSource2;
    private AudioClip beepSound;
    public Animator landmineAnimator;
    private AnimatorClipInfo[] currentAnimationClip;
    private float currentAnimationClipLength;
    private AudioClip mineExplosion;

    private void Awake()
    {
        aSource = this.gameObject.GetComponent<AudioSource>();
        aSource2 = this.gameObject.transform.GetChild(0).GetComponent<AudioSource>();
        beepSound = AudioManager.Instance.beep;
        landmineAnimator = this.gameObject.GetComponent<Animator>();
        mineExplosion = AudioManager.Instance.mineExplosion;
    }

    private void Start()
    {
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
        if(collision.CompareTag("Enemy"))
        {
            landmineAnimator.SetTrigger("ZombieTrigger");
            Destroy(collision.gameObject);
            aSource2.PlayOneShot(mineExplosion);
            StartCoroutine(DestroyLandMine());
            
        }
    }
}
