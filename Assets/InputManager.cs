using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager _instance;
    public static InputManager Instance
    {
        get
        {
            if (_instance is null)
                Debug.LogError("Input Manager is NULL");

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    public static Vector3 _mousePosition { get; private set; }

    private void Update()
    {
        
    }

    public Vector3 MousePosition()
    {
        _mousePosition = Input.mousePosition;
        return _mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(_mousePosition.x, _mousePosition.y, Camera.main.transform.position.z));
        Debug.Log("mouse pos: " + _mousePosition);

        
    }

}
