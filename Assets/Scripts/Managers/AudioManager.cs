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
                Debug.LogError("Audio Manager is NULL");

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
                PlayMusic();
                break;

            case GameState.Playing:
                break;

            case GameState.Paused:
                break;

            case GameState.RoundOver:

                break;


            default:
                break;
        }
    }

    void PlayMusic()
    {
        aSourceAM4.clip = musicTrack1;
        aSourceAM4.Play();
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
        idleMotor = Resources.Load<AudioClip>("Sound/IdleMotor");
        driveMotor = Resources.Load<AudioClip>("Sound/DriveMotor");
        turretStart = Resources.Load<AudioClip>("Sound/TurretMovement/turret_start");
        turretMid = Resources.Load<AudioClip>("Sound/TurretMovement/turret_mid");
        turretRotation = Resources.Load<AudioClip>("Sound/TurretRotate");
        turretEnd = Resources.Load<AudioClip>("Sound/TurretMovement/turret_end");
        beep = Resources.Load<AudioClip>("Sound/MineBeep");
        mineExplosion = Resources.Load<AudioClip>("Sound/MineExplosion");
        mineDeploy = Resources.Load<AudioClip>("Sound/Deploy_Mine");
        tankVaultClose = Resources.Load<AudioClip>("Sound/TankVaultShut");
        tankVaultClose2 = Resources.Load<AudioClip>("Sound/TankVaultShut2");
        tankVaultClose3 = Resources.Load<AudioClip>("Sound/TankVaultShut3");
        tankVaultOpen1 = Resources.Load<AudioClip>("Sound/TankVaultOpen1");
        subMachineGun_Silenced = Resources.Load<AudioClip>("Sound/suppressed-submachinegun");
    }
    //Zombie death
    public AudioClip splatSound;
    public AudioClip wetSplat;
    public AudioClip death1;

    // Shooting sounds
    public AudioClip subMachineGun_Silenced;

    //Bullet hit sounds
    public AudioClip[] bulletHitFleshClips;
    public AudioClip[] bulletHitMetalClips;

    //Car hit sounds
    public AudioClip[] zombieHitCarClips;

    //Pickup Sounds
    public AudioClip moneyPickUpSound;

    //Tank drive sounds
    public AudioClip idleMotor;
    public AudioClip driveMotor;

    //Tank turret sounds
    public AudioClip turretStart;
    public AudioClip turretEnd;
    public AudioClip turretMid;

    //Tank close vault sound
    public AudioClip tankVaultClose;
    public AudioClip tankVaultClose2;
    public AudioClip tankVaultClose3;
    public AudioClip tankVaultOpen1;

    //Alternative sound
    public AudioClip turretRotation;

    //Landmine Sounds
    public AudioClip beep;
    public AudioClip mineExplosion;
    public AudioClip mineDeploy;

    // Environment sounds
    public AudioClip rain1;

    // Music
    public AudioClip musicTrack1;

    // Audio Sources
    public AudioSource aSourceAM;
    public AudioSource aSourceAM2;
    public AudioSource aSourceAM3;
    public AudioSource aSourceAM4;
}
