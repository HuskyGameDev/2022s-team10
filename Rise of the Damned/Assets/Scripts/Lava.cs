using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{

    public float knockback;

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.tag.Equals("Player")) { //if player collides with lava
            PlayerController.health = PlayerController.health - 10;
            //knockback
            collision.rigidbody.velocity += new Vector2(0, knockback);
            PlayerController.controller.CreateDust();
        }
    }

}