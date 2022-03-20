using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    public float offsetX, offsetY;
    public float knockback;

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag.Equals("Player")) { //if player collides with spikes
            PlayerController.health = PlayerController.health - 10;
            //knockback
            collision.attachedRigidbody.velocity = new Vector2(0, Mathf.Sign(collision.transform.position.x - transform.position.x) * knockback);

            /*Vector2 knockDir = new Vector2(collision.transform.position.x - transform.position.x - offsetX, collision.transform.position.y - transform.position.y - offsetY);
            collision.attachedRigidbody.velocity += knockDir.normalized * knockback;
            Debug.Log(knockDir);*/
        }
    }

}
