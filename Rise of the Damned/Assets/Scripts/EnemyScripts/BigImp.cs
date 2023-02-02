using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigImp : EnemyController
{
    private Coroutine stateUpdate; //calls the coroutine for the state of the enemy
    bool isIdle = false;
    bool isAttacking = true;
    bool isSwinging = false;
    bool isSwinging2 = false; //second half of animation for knockback cancel
    bool lockSwing = false; //locks the state to swinging to prevent half swings

    [SerializeField]
    float swingDistance = 2f;
    [SerializeField]
    float attackCooldown = 1f;

    [Header("Animations")]
    public Animator animator;


    // Update is called once per frame
    public override void Update2()
    {
        if (Vector2.Distance(rb.position, PlayerController.controller.rb.position) > aggroDist)
        {
            state = State.Wander;
        }
        else if (Vector2.Distance(rb.position, PlayerController.controller.rb.position) < aggroDist//far enough away from player
            && Vector2.Distance(rb.position, PlayerController.controller.rb.position) > swingDistance) //and not closer than swing distance
        {
            state = State.Attack;
        }
        else if (Vector2.Distance(rb.position, PlayerController.controller.rb.position) < swingDistance)
        {
            state = State.Swing;
        }
        //Debug.Log(state);
    }

    public override void Wander()
    {
        if (!isIdle && !lockSwing) // is not already idle and not currently swinging
        {
            isIdle = true;
            isAttacking = false;
            isSwinging = false;
            if (stateUpdate != null) { StopCoroutine(stateUpdate); }
            stateUpdate = StartCoroutine(Idle());
        }

    }

    public override void Attack()
    {
        if (!isAttacking && !lockSwing) // is not already attacking and not currently swinging
        {
            isAttacking = true;
            isIdle = false;
            isSwinging = false;
            if (stateUpdate != null) { StopCoroutine(stateUpdate); }
            stateUpdate = StartCoroutine(Agro());
        }
        
    }

    public override void Swinging()
    {
        if(!isSwinging)
        {
            isSwinging = true;
            isIdle = false;
            isAttacking = false;
            if (stateUpdate != null) { StopCoroutine(stateUpdate); }
            stateUpdate = StartCoroutine(Swing());
        }

    }
    IEnumerator Idle()
    {
        animator.SetBool("isAttacking", false); // ends swinging animation
        while (true)
        {
            bool hitEdge = false;
            //beginning animation
            animator.SetBool("isRunning", false); //idle for 6 seconds
            rb.velocity = (new Vector2(0, 0)); //stops moving
            yield return new WaitForSeconds(Random.Range(2, 5));

            //move
            animator.SetBool("isRunning", true); //running for 1 second

            while (!hitEdge) // loop until hit a wall or edge
            {
                desired_velocity = new Vector2(direction.x, 0f) * Mathf.Max(max_speed, 0f);
                velocity = rb.velocity;
                acceleration = max_acceleration;
                max_speed_change = acceleration * Time.deltaTime;

                if (!receivingKnockback && !isSwinging)
                {
                    velocity.x = Mathf.MoveTowards(velocity.x, desired_velocity.x, max_speed_change);
                    if (!CheckEdge() || CheckWall())
                    {
                        hitEdge = true;
                        velocity = (new Vector2(0, 0)); //stops moving
                        animator.SetBool("isRunning", false);
                        TurnAround();
                    }
                }

                rb.velocity = velocity;



                yield return new WaitForEndOfFrame();
            }
        }
    }
    IEnumerator Agro()
    {
        animator.SetBool("isAttacking", false); // ends swinging animation
        while (true)
        {
            if (!receivingKnockback)
            {
                FacePlayer();
                bool hitEdge = false;
                if (!CheckEdge() || CheckWall()) //checks for edge before moving towards the player
                {
                    hitEdge = true;
                } else
                {
                    hitEdge = false;
                }
                yield return new WaitForSeconds(1 / 4);

                while (!hitEdge) // loop until hit a wall or edge
                {
                    FacePlayer();
                    if (Mathf.Abs(rb.position.x - PlayerController.controller.rb.position.x) < .5) //if under or over player, stop moving
                    {
                        animator.SetBool("isRunning", false);
                        desired_velocity = new Vector2(0f, 0f); //stops moving
                    } else
                    {
                        animator.SetBool("isRunning", true);
                        desired_velocity = new Vector2(direction.x, 0f) * Mathf.Max(max_speed, 0f);
                        velocity = rb.velocity;
                        acceleration = max_acceleration;
                        max_speed_change = acceleration * Time.deltaTime;
                    }

                    if (!receivingKnockback && !isSwinging)
                    {
                        velocity.x = Mathf.MoveTowards(velocity.x, desired_velocity.x, max_speed_change);


                        if (!CheckEdge() || CheckWall())
                        {
                            hitEdge = true;
                            velocity = (new Vector2(0, 0)); // stops moving
                            animator.SetBool("isRunning", false);
                        }
                    }

                    rb.velocity = velocity;

                    yield return new WaitForEndOfFrame();
                }
            }
            yield return new WaitForEndOfFrame();
        }

    }

    IEnumerator Swing()
    {
        while (true)
        {
            if(!receivingKnockback)
            {
                FacePlayer();

                animator.SetBool("isRunning", false); //stops running -> to idle animation

                rb.velocity = (new Vector2(0, 0)); //stops moving

                animator.SetBool("isAttacking", true); // starts swinging animation
                lockSwing = true;

                yield return new WaitForSeconds(.3f);

                isSwinging2 = true;

                yield return new WaitForSeconds(.7f);

                animator.SetBool("isAttacking", false); // stops swinging animation
                lockSwing = false;
                isSwinging2 = false;

                yield return new WaitForSeconds(attackCooldown); // wait for attack cooldown 
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public override void KnockbackDrag()
    {
        if (receivingKnockback && isSwinging2 && CheckGround())
        {
            rb.drag = 50;
        } else if (receivingKnockback)
        {
            rb.drag = knockback_drag;
        } else
        {
            rb.drag = 0;
        }
    }
}
