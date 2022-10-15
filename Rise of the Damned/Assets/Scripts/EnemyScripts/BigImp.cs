using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigImp : EnemyController
{
    private Coroutine stateUpdate; //calls the coroutine for the state of the enemy
    [SerializeField]
    bool isAttacking = true;
    bool isSwinging = false;

    [SerializeField]
    float swingDistance = 1f;

    [Header("Animations")]
    public Animator animator;


    // Update is called once per frame
    public override void Update2()
    {

    }

    public override void Wander()
    {
        if (isAttacking)
        {
            isAttacking = false;
            if (stateUpdate != null) { StopCoroutine(stateUpdate); }
            stateUpdate = StartCoroutine(Idle());
        }
        if (Vector2.Distance(rb.position, PlayerController.controller.rb.position) < aggroDist && !isAttacking //far enough away from player
            && Mathf.Abs(rb.position.x - PlayerController.controller.rb.position.x) > .5) //and not under or over the player
        {
            state = State.Attack;
        }
    }

    public override void Attack()
    {
        if (Vector2.Distance(rb.position, PlayerController.controller.rb.position) < swingDistance)
        {
            isSwinging = true;
            stateUpdate = StartCoroutine(Swing());
        }
        if (!isAttacking)
        {
            isAttacking = true;
            if (stateUpdate != null) { StopCoroutine(stateUpdate); }
            stateUpdate = StartCoroutine(Agro());
        }
        if (Vector2.Distance(rb.position, PlayerController.controller.rb.position) > aggroDist * 1.5 && isAttacking 
            || Mathf.Abs(rb.position.x - PlayerController.controller.rb.position.x) < .5)
        {
            state = defaultState;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (PlayerController.TakeDamage(damage))
            {
                PlayerController.controller.Knockback(knockback, gameObject.transform);
            }
        }
    }

    IEnumerator Agro()
    {
        while (true)
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
            yield return new WaitForSeconds(1/4);

            while (!hitEdge) // loop until hit a wall or edge
            {
                animator.SetBool("isRunning", true);
                FacePlayer();
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
                        velocity = (new Vector2(0, rb.velocity.y)); // stops moving
                        animator.SetBool("isRunning", false);
                    }
                }

                rb.velocity = velocity;

                yield return new WaitForEndOfFrame();
            }
        }

    }

    IEnumerator Idle()
    {
        while (true)
        {
            bool hitEdge = false;
            //beginning animation
            animator.SetBool("isRunning", false); //idle for 6 seconds
            rb.velocity = (new Vector2(0, rb.velocity.y)); //stops moving
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
                        velocity = (new Vector2(0, rb.velocity.y)); //stops moving
                        animator.SetBool("isRunning", false);
                        TurnAround();
                    }
                }

                rb.velocity = velocity;

                

                yield return new WaitForEndOfFrame();
            }
        }
    }

    IEnumerator Swing()
    {

        rb.velocity = (new Vector2(0, rb.velocity.y)); //stops moving

        animator.SetBool("isAttacking", true); // starts swinging animation

        yield return new WaitForSeconds(1/2);

        animator.SetBool("isAttacking", false); // starts swinging animation
        isSwinging = false;
    }
    public new void TurnAround()
    {
        direction.x *= -1;
        GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
        velocity.x = 0;
    }

}
