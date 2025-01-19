using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingCharacterController : MonoBehaviour
{
    // Movement
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;

    // Gun 
    [SerializeField] private float _gunfireSpriteAppearenceTime;
    [SerializeField] private Sprite _gunfireSprite1;
    [SerializeField] private Sprite _gunfireSprite2;
    [SerializeField] private Sprite _gunfireSprite3;
    [SerializeField] private Sprite _gunfireSprite4;
    [SerializeField] private GameObject _gunfireObject;
    [SerializeField] private GameObject _gunFlash;
    [SerializeField] private GameObject _gunFlashBacking;
    private int _gunfireSpriteCounter = 0;
    private List<Sprite> _gunfireSpriteList;
    [SerializeField] private float _firingSpeed;
    private bool _shooting = false;
    private AudioClip _subMachineGun_Silenced;

    // Audio
    private AudioSource _aSource;


    //[SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D body;

    // Start is called before the first frame update
    void Start()
    {
        //animator = this.GetComponent<Animator>();
        _aSource = this.GetComponent<AudioSource>();

        _gunfireSpriteList = new List<Sprite> { _gunfireSprite1, _gunfireSprite2, _gunfireSprite3, _gunfireSprite4 };

        _gunFlash.SetActive(false);
        _gunfireObject.SetActive(false);
        _gunFlashBacking.SetActive(false);
        _subMachineGun_Silenced = AudioManager.Instance.subMachineGun_Silenced;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        //PlayWalkAnimation();

        Shoot();
    }

    void Shoot()
    {
        if (Input.GetKey(KeyCode.Mouse0) && _shooting == false)
        {

            StartCoroutine(MachinegunFire());

        }
    }
    private void FixedUpdate()
    {
        Rotate();
    }

    void Move()
    {
        Vector2 inputVector2 = Vector2.zero;

        inputVector2.x = Input.GetAxisRaw("Horizontal");
        inputVector2.y = Input.GetAxisRaw("Vertical");

        if (inputVector2.magnitude > 1) // Normalize speed so diagonal movement is not double speed
        {
            inputVector2 = inputVector2.normalized; 
        }

        body.velocity = inputVector2 * moveSpeed;

    }

    IEnumerator MachinegunFire()
    {
        _shooting = true;
        StartCoroutine(GunFireFlash(_gunfireSpriteAppearenceTime));
        _aSource.pitch = Random.Range(0.94f, 1.06f);
        _aSource.PlayOneShot(_subMachineGun_Silenced);
        yield return new WaitForSeconds(_firingSpeed);
        _shooting = false;
    }

    void Rotate()
    {
        Vector2 lookDir = InputManager.Instance.MousePosition() - transform.position; // Vector from mouse to player
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f; // Calculate the angle of the vector. The -90 is an offset that apparently is required to fit the angle.
        body.rotation = angle; // Rotate the object
    }

    IEnumerator GunFireFlash(float seconds)
    {
        _gunfireObject.gameObject.GetComponent<SpriteRenderer>().sprite = _gunfireSpriteList[_gunfireSpriteCounter];
        _gunfireObject.SetActive(true);
        _gunFlash.SetActive(true);
        _gunFlashBacking.SetActive(true);
        float randomRotationValue = Random.Range(_gunFlashBacking.transform.rotation.z-5f, _gunFlashBacking.transform.rotation.z + 5f); // vary backing light rotation randomly
        _gunFlashBacking.transform.Rotate(_gunFlashBacking.transform.rotation.x, _gunFlashBacking.transform.rotation.y, randomRotationValue);

        yield return new WaitForSeconds(seconds);
        
        _gunfireObject.SetActive(false);
        _gunFlash.SetActive(false);
        _gunFlashBacking.SetActive(false);

        _gunfireSpriteCounter++;

        if (_gunfireSpriteCounter >= _gunfireSpriteList.Count)
        {
            _gunfireSpriteCounter = 0;
        }
    }
    /*void PlayWalkAnimation()
    {
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            animator.SetBool("PressingMove", true);
        }
        else
        {
            animator.SetBool("PressingMove", false);
        }
    }*/


}
