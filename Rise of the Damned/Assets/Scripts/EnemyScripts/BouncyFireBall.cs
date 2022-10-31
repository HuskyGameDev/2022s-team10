using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyFireBall : EnemyProjController
{
    private bool fired = false;
    private int bounceCounter = 0;
    [System.NonSerialized]
    public int bounceLimit;
    [System.NonSerialized]
    public int direction;

    // Update is called once per frame
    void Update()
    {
        
        if(fired == false)
        {
            fired = true;
            rb.AddForce(new Vector2(Random.Range(3f, 6f) * direction, Random.Range(3f, 6f)), ForceMode2D.Impulse); //
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController.TakeDamage(damage);
            PlayerController.controller.Knockback(knockback, gameObject.transform);
            Destroy(gameObject);
        } 
        else if (collision.name.Contains("SwordSwipe"))
        {
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Ground"))
        {
            bounceCounter += 1;
            if(bounceCounter >= bounceLimit)
            {
                Destroy(gameObject);
            }
        }
    }


}
