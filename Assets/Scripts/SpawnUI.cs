using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnUI : MonoBehaviour
{
    private GameObject shopUI;

    private void Awake()
    {
        shopUI = Resources.Load<GameObject>("ShopUI");
    }
    void Start()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Collision"))
        {
            Debug.Log("Opening Shop!");
            Instantiate(shopUI);
        }
    }

}
