using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankInputController : MonoBehaviour
{
    TankController tankController;

    private void Awake()
    {
        tankController = GetComponent<TankController>();
    }
    

    void Update()
    {
        Vector2 inputVector2 = Vector2.zero;

        inputVector2.x = Input.GetAxisRaw("Horizontal");
        inputVector2.y = Input.GetAxisRaw("Vertical");

        tankController.SetInputVector(inputVector2);
    }
}
