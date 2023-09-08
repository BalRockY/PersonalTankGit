using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform tank;
    public float duration;

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

    public IEnumerator Shake()
    {
        Vector3 startPosition = this.gameObject.transform.position;

        float elapsedTime = 0f;

        while(elapsedTime < duration)
        {
            Debug.Log("shakin");
            elapsedTime += Time.deltaTime;
            this.gameObject.transform.position = startPosition + new Vector3((transform.position.x + Random.Range(0,10)), (transform.position.y + Random.Range(0, 10)), transform.position.z);
            yield return null;
        }

        this.gameObject.transform.position = startPosition;
    }

}
