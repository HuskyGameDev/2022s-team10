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

    private Animator anim;
    private SpriteRenderer spriteRend;

    private bool triggered; //player hits trap
    private bool active; //trap is on

    private void Awake() // get things ready
    {
        anim = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerStay2D(Collider2D collision){ //even better than onTriggerEnter2D! buy now! 

        if (collision.tag == "Player")
        { 
            if(!triggered)
            {
                StartCoroutine(ActivateFiretrap());
            }

            if(active)
            {
                PlayerController.TakeDamage(damage); 
                
                // yeet player by getting acess to their rigid body 
                Rigidbody2D player = collision.GetComponent<Rigidbody2D>();
                Vector2 direction = player.transform.position - transform.position;
                direction = direction.normalized * knockbackForce; //normalized sets vector to a max length of 1, we need direction times the force we want
                player.AddForce(direction, ForceMode2D.Impulse);    
            }

        }
    }

    private IEnumerator ActivateFiretrap(){ //coroutine lets it run one time step by step so you cant spam damaging yourself...
        
        // lets player know the trap will activate by turning it red
        triggered = true;
        spriteRend.color = Color.Lerp(Color.white, Color.black, .1f);

        // wait for delay, activate trap, turn on animation, return color
        yield return new WaitForSeconds(activationDelay);
         spriteRend.color = Color.white; // return color back to normal
        active = true;
        anim.SetBool("activated", true);

        // wait until x seconds, deactivate trap and reset variables
        yield return new WaitForSeconds(activeTime);
        active = false;
        triggered = false;
        anim.SetBool("activated", false);
    }

}
