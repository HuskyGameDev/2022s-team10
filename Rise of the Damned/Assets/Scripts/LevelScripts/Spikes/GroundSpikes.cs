using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSpikes : MonoBehaviour
{

    public float damage = 10f;
    public float knockback = 12f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        { //if player collides with spikes
            PlayerController.TakeDamage(damage);

            collision.attachedRigidbody.velocity = new Vector2(0, 0); // stops player velocity so all spike knockback is the same
                                                                      // and not affected by the speed the player hits it
                                                                      //collision.attachedRigidbody.AddForce(new Vector2(0, knockback), ForceMode2D.Impulse);

            PlayerController.controller.SpikeKnockback(knockback, new Vector2(0, 1));
        }
    }

    // yes, making 4 new scripts for each of the spike directions is dumb
    // and more work than necessary but if you wanted it to be perfect
    // then you should've done it yourself

}
