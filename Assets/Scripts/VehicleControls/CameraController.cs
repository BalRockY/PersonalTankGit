using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform tank;

    private void Start()
    {
        FindTank();
    }
    public void FindTank()
    {
        tank = GameObject.FindGameObjectWithTag("Tank").transform;
    }
    private void Update()
    {
        if (tank != null)
            transform.position = new Vector3(tank.transform.position.x, tank.transform.position.y, transform.position.z);
        else
            transform.position = this.transform.position;
            
    }

}
