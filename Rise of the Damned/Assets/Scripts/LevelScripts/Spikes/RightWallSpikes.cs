using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightWallSpikes : MonoBehaviour
{

    public float damage = 10f;
    public float knockback = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        { //if player collides with spikes
            PlayerController.TakeDamage(damage);


            collision.attachedRigidbody.velocity = new Vector2(0, 0); // stops player velocity so all spike knockback is the same
                                                                      // and not affected by the speed the player hits it
                                                                      //collision.attachedRigidbody.AddForce(new Vector2(-knockback, 0), ForceMode2D.Impulse);

            PlayerController.controller.SpikeKnockback(knockback, new Vector2(-1, 0));
        }
    }
}
