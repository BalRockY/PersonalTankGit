using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingCharacterController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private float rotationSpeed;

    [SerializeField] private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        PlayAnimation();
    }

    void Move()
    {
        Vector2 inputVector2 = Vector2.zero;

        inputVector2.x = Input.GetAxisRaw("Horizontal");
        inputVector2.y = Input.GetAxisRaw("Vertical");

        // Normalize the input vector if it's not zero
        if (inputVector2 != Vector2.zero)
        {
            float angle = Mathf.Atan2(inputVector2.y, inputVector2.x) * Mathf.Rad2Deg + 90f;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // Rotate the character towards the direction it is facing
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            inputVector2.Normalize();
        }

        // Apply the normalized input vector to the velocity
        body.velocity = inputVector2 * moveSpeed;

    }  
    void PlayAnimation()
    {
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            animator.SetBool("PressingMove", true);
        }
        else
        {
            animator.SetBool("PressingMove", false);
        }
    }


}
