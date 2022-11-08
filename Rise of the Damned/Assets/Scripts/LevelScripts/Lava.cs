using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    public float damage = 10f;
    public float knockback = 12f;

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("Player")) { //if player collides with lava
            PlayerController.TakeDamage(damage);
            //knockback
            PlayerController.controller.SpikeKnockback(knockback, new Vector2(0, 1));
            PlayerController.controller.CreateDust();
        }
    }

}