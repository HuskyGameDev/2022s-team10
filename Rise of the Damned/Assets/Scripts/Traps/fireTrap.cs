using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireTrap : MonoBehaviour
{
    [SerializeField] private float damage;

    [Header("Firetrap Timers")]
    [SerializeField] private float activationDelay;
    [SerializeField] private float activeTime;

    [Header("knockback")]
    public float knockbackForce = 8f;

    [Header("Bools")]
    [SerializeField] private bool triggered; //player hits trap
    [SerializeField] private bool active; //trap is on
    [SerializeField] private bool playerInsideTrap;

    private Collider2D playerCollider;

    private Animator anim;
    private SpriteRenderer spriteRend;

    private void Awake() // get things ready
    {
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision){ //even better than onTriggerEnter2D! buy now! 
        
        if (collision.tag == "Player")
        { 
            playerCollider = collision;
            playerInsideTrap = true;
        
            if(!triggered)
            {
                StartCoroutine(ActivateFiretrap());
            }

            if(active)
            {
                TrapKnockback(playerCollider);
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision){
        
        playerInsideTrap = false;
       
    }

    private IEnumerator ActivateFiretrap(){ //coroutine lets it run one time step by step so you cant spam damaging yourself...
        
        // lets player know the trap will activate by turning it greyish
        triggered = true;
        spriteRend.color = Color.Lerp(Color.white, Color.black, .2f);

        // wait for delay, activate trap, turn on animation, return color
        yield return new WaitForSeconds(activationDelay);
        spriteRend.color = Color.white; // return color back to normal
        active = true;
        anim.SetBool("activated", true);
        
        if(playerInsideTrap == true){
            TrapKnockback(playerCollider);
        }

        // wait until x seconds, deactivate trap and reset variables
        yield return new WaitForSeconds(activeTime);
        active = false;
        triggered = false;
        anim.SetBool("activated", false);
    }

    private void TrapKnockback(Collider2D collision){ //after a 2dboxcollider is triggered, use that collider to get the Rigidbody2D, then yeet. might use this for enemies too later

        PlayerController.TakeDamage(damage); 
                
        // create knoockback by referencing their rigid body 
        Rigidbody2D player = collision.GetComponent<Rigidbody2D>();
        Vector2 direction = player.transform.position - transform.position;
        direction = direction.normalized * knockbackForce; //normalized sets vector to a max length of 1, we need direction times the force we want
        player.AddForce(direction, ForceMode2D.Impulse);   
    }


}
