using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    [Header("Default: 10dmg, 14kb")]
    public float damage;
    public float knockback;

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.CompareTag("Player")) { //if player collides with lava
            PlayerController.TakeDamage(damage);
            //knockback
            collision.rigidbody.velocity += new Vector2(0, knockback);
            PlayerController.controller.CreateDust();
        }
    }

}