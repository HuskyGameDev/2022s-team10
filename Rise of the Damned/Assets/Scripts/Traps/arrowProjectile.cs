using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrowProjectile : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float knockbackForce;
    [SerializeField] private float speed = 5;
    [SerializeField] private float resetTime; //deactivate object after a certain periode of time
    private float lifetime;

    void Update(){
 
        transform.Translate(speed * Time.deltaTime, 0, 0); //burr arrow go forword 

        lifetime += Time.deltaTime;
        if (lifetime > resetTime){
            gameObject.SetActive(false);
            //Debug.Log("destroyed arrow");
        }
    }

    public void ActivateProjectile(){
        lifetime = 0;
        gameObject.SetActive(true);

    }

    void OnTriggerEnter2D(Collider2D collision){ //arrow hits something 

        if (collision.tag == "Player"){
            TrapKnockback(collision);
            PlayerController.TakeDamage(damage);
        }
        //gameObject.SetActive(false); // when hits anything
    }

    private void TrapKnockback(Collider2D collision){ //after a 2dboxcollider is triggered, use that collider to get the Rigidbody2D, then yeet
  
        // create knoockback by referencing their rigid body 
        Rigidbody2D player = collision.GetComponent<Rigidbody2D>();
        Vector2 direction = player.transform.position - transform.position;
        direction = direction.normalized * knockbackForce; //normalized sets vector to a max length of 1, we need direction times the force we want
        player.AddForce(direction, ForceMode2D.Impulse);   
    }


}
