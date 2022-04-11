using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class temp_wall_Spike_Fix : MonoBehaviour
{
        private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.tag.Equals("Player")) { //if player collides with lava
            PlayerController.health = PlayerController.health - 10;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        { //if player collides with spikes
            PlayerController.health = PlayerController.health - 10;
            //knockback
            //collision.attachedRigidbody.velocity += new Vector2(0, Mathf.Sign(collision.transform.position.x - transform.position.x) * knockback);

        }
    }
}
