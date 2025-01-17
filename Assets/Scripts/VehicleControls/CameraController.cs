using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject tank;
    public GameObject walkingCharacter;
    public float duration;

    private void Start()
    {
        //FindTank();
        PlayerManager.Instance.driving = true;
    }
    public void FindTank()
    {
        tank = GameObject.FindGameObjectWithTag("Tank");
        Debug.Log("this is: " + tank);
    }
    private void Update()
    {
        if(PlayerManager.Instance.driving == true)
        {
            if (tank != null)
                transform.position = new Vector3(tank.transform.position.x, tank.transform.position.y, transform.position.z);
            else
            {
                tank = GameObject.FindGameObjectWithTag("Tank");
            }
        }
        
        else if(PlayerManager.Instance.walking == true)
        {
            if (walkingCharacter != null)
            {
                transform.position = new Vector3(walkingCharacter.transform.position.x, walkingCharacter.transform.position.y, transform.position.z);
            }
                
            else
            {
                walkingCharacter = GameObject.FindGameObjectWithTag("Player");
            }
        }
            
           
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
