using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : EnemyController
{
    [Header("Enemy Stats")]
    public float jumpSpeed;
    public float agroSpeed;

    private Coroutine slimeUpdate;
    [SerializeField]
    bool isAttacking = true;
    [SerializeField]
    bool isJumping = false;
    [SerializeField]
    bool isPrejumping = false;
    [SerializeField]
    bool inAir = false;

    bool k = false; //trigger var
    [SerializeField]
    float ground_drag;

    [Header("Animations")]
    public Animator animator;


    // ------ DO NOT CHANGE ANY OF THIS CODE OR SO HELP ME GOD
    // If I have to fix this fucking slime one more time there will be riots


    // Update2 is called once per frame
    public override void Update2()
    {
        if (CheckGround() && !isPrejumping && isJumping) //if slime is landing on the ground
        {
            if(inAir)
            {
                rb.velocity = (new Vector2(0, 0));
                isJumping = false;
                inAir = false;
                animator.SetBool("isJumping", false);
            }
        }

        
        if(k && !receivingKnockback) //triggers on the frame where knockback ends
        {
            rb.velocity = (new Vector2(0, velocity.y));
        }
        if(!k && receivingKnockback) //triggers on the frame where knockback starts
        {
            animator.SetBool("isJumping", false);
            
        }

        if(receivingKnockback) //sets up trigger
        {
            k = true;
        } else
        {
            k = false;
        }


        if(!CheckGround())
        {
            knockback_drag = .8f;
        } else
        {
            knockback_drag = ground_drag;
        }

    }

    public override void Wander()
    {
        if (isAttacking) //if slime was attacking before
        {
            isAttacking = false;
            if (slimeUpdate != null) { StopCoroutine(slimeUpdate); }
            slimeUpdate = StartCoroutine(Idle());
        }
        if (Vector2.Distance(rb.position, PlayerController.controller.rb.position) < aggroDist && !isAttacking) 
        { //if slime is close enough to player then attack
            state = State.Attack;
        }
    }

    public override void Attack()
    {
        if (CheckGround()) { //slime can only turn to face the player when it is on the ground
            FacePlayer();
        }

        if (!isAttacking) //if slime was idle before
        {
            isAttacking = true;
            if (slimeUpdate != null) { StopCoroutine(slimeUpdate); }
            slimeUpdate = StartCoroutine(Agro());
        }
        if (Vector2.Distance(rb.position, PlayerController.controller.rb.position) > aggroDist * 1.5 && isAttacking && !isPrejumping && !isJumping)
        { //if slime is too far from the player then idle
            state = State.Wander;
        }
    }

    IEnumerator Agro()
    {
        while (true)
        {
            //start animation
            animator.SetBool("isPrejumping", true);
            isJumping = true;
            isPrejumping = true;
            yield return new WaitForSeconds(1);

            //start jumping part of animation
            isPrejumping = false;
            

            //1 if player is to the right, -1 if player is to the left
            FacePlayer();

            //Jump towards player
            rb.velocity = (new Vector2(agroSpeed * direction.x, jumpSpeed));
            yield return new WaitForSeconds(.03f);
            animator.SetBool("isPrejumping", false);
            animator.SetBool("isJumping", true);
            inAir = true;
            yield return new WaitForSeconds(1.97f);
        }

    }

    IEnumerator Idle()
    {
        yield return new WaitForSeconds(0);
    }
}
