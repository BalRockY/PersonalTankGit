using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moneyPickup : MonoBehaviour
{
    private SpriteRenderer sprite;
    private Transform tank;

    private void Awake()
    {
        sprite = this.gameObject.GetComponent<SpriteRenderer>();
        tank = GameObject.FindGameObjectWithTag("Tank").transform;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Collision"))
        {
            UIManager.Instance.cash += 50;
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        var tankDistance = Mathf.Sqrt(Mathf.Pow(tank.transform.position.x - transform.position.x, 2) + Mathf.Pow(tank.transform.position.y - transform.position.y, 2));
        if (tankDistance < gameManager.Instance.renderDistance)
        {
            sprite.enabled = true;

        }
        else
        {
            sprite.enabled = false;
        }
    }
}
