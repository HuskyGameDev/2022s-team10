using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    [Header("Default: 10dmg, 3kb")]
    public float damage = 10f;
    public float knockback = 12f;

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag.Equals("Player")) { //if player collides with spikes
            PlayerController.TakeDamage(damage);


            


            /*
            //knockback
            //if (!name.ToLower().Contains("wall"))
                collision.attachedRigidbody.velocity = new Vector2(
                    collision.attachedRigidbody.velocity.x, 
                    Mathf.Sign(knockback) == Mathf.Sign(collision.attachedRigidbody.velocity.y) ?
                    collision.attachedRigidbody.velocity.y + knockback : knockback);    //yea i know this sucks, it's how i wrote it ok?

            /*Vector2 knockDir = new Vector2(collision.transform.position.x - transform.position.x - offsetX, collision.transform.position.y - transform.position.y - offsetY);
            collision.attachedRigidbody.velocity += knockDir.normalized * knockback;
            Debug.Log(knockDir);*/


        }
    }

}
