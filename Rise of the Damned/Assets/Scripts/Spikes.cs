using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{

    public float knockback;

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.tag.Equals("Player")) { //if player collides with spikes
            PlayerController.health = PlayerController.health - 10;
            //knockback
            //PlayerController.attachedRigidbody.velocity += new Vector2(Mathf.Sign(collision.transform.position.x - PlayerController.player.transform.position.x) * knockback, knockback / 2);
        }
    }

}
