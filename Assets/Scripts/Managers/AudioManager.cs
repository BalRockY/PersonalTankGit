using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance is null)
                Debug.LogError("AudioManager Manager is NULL");

            return _instance;
        }
    }
    private void UIManagerStateChange(GameState newState)
    {
        switch (newState)
        {
            case GameState.MainMenu:

                break;

            case GameState.Start:

                break;

            case GameState.Playing:
                break;

            case GameState.Paused:
                break;

            case GameState.GameOver:

                break;


            default:
                break;
        }
    }

    private void Awake()
    {
        _instance = this;
        GameManager.OnGameStateChanged += UIManagerStateChange;
        aSourceAM = this.gameObject.GetComponent<AudioSource>();
        aSourceAM2 = this.gameObject.transform.GetChild(0).GetComponent<AudioSource>();
        bulletHitMetalClips = Resources.LoadAll<AudioClip>("Sound/BulletHitMetal");
        bulletHitFleshClips = Resources.LoadAll<AudioClip>("Sound/BulletHitFlesh");
        zombieHitCarClips = Resources.LoadAll<AudioClip>("Sound/ZombieHitCar");
        moneyPickUpSound = Resources.Load<AudioClip>("Sound/PickUpPling");

    }
    public AudioClip splatSound;
    public AudioClip wetSplat;
    public AudioClip death1;

    //Bullet hit sounds
    public AudioClip[] bulletHitFleshClips;
    public AudioClip[] bulletHitMetalClips;

    //Car hit sounds
    public AudioClip[] zombieHitCarClips;

    //Pickup Sounds
    public AudioClip moneyPickUpSound;

    public AudioSource aSourceAM;
    public AudioSource aSourceAM2;


}
