using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectileController : MonoBehaviour
{
    [SerializeField]
    Rigidbody2D rb2d;

    public float speed;

    private GameObject theTank;
   

    //private GunController gunController;

    //public float projectileForce;

    //private float randomSpread;

    //private float projectileAngle;

    private void Awake()
    {
        rb2d = this.gameObject.GetComponent<Rigidbody2D>();
        theTank = GameObject.FindGameObjectWithTag("Tank");
    }

    private void Start()
    {
        //randomSpread = Random.Range(0, 10);
        //projectileAngle = gunController.angle;
        StartCoroutine(DestroyAfterTime());
        
        //transform.eulerAngles = new Vector3(0, 0, gunController.angle);
        //transform.Rotate(0, 0, (projectileAngle*randomSpread));
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("projectile hit: " + collision.gameObject);
        
        if(collision.gameObject.tag != "Tank")
        {
            Destroy(this.gameObject);
        }
        if(collision.gameObject.tag == "Tank")
        {
            
        }
        

    }


    private void Update()
    {
        rb2d.velocity = transform.up * speed;
        //rb2d.AddForce(transform.position * projectileForce, ForceMode2D.Impulse);
    }

    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(5);

        Destroy(this.gameObject);
    }
}
