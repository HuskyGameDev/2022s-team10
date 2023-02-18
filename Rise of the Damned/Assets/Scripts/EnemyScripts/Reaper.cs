using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reaper : EnemyController
{

    private Coroutine stateUpdate; //calls the coroutine for the state of the enemy
    [SerializeField]
    bool isAttacking = true;

    [Header("Animations")]
    public Animator animator;

    public override void Update2()
    {
        if (Vector2.Distance(rb.position, PlayerController.controller.rb.position) > aggroDist * 1.5 && isAttacking)
        {
            state = defaultState;
        } else if (Vector2.Distance(rb.position, PlayerController.controller.rb.position) < aggroDist && !isAttacking //far enough away from player
              && Mathf.Abs(rb.position.x - PlayerController.controller.rb.position.x) > .5) //and not under or over the player
        {
            state = State.Attack;
        }
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
        if (!isAttacking)
        {
            isAttacking = true;
            if (stateUpdate != null) { StopCoroutine(stateUpdate); }
            stateUpdate = StartCoroutine(Agro());
        }
    }

    IEnumerator Idle()
    {
        while (true)
        {
            bool hitWall = false;
            //beginning animation
            //animator.SetBool("isRunning", false); //idle for 6 seconds
            rb.velocity = (new Vector2(0, 0)); //stops moving
            yield return new WaitForSeconds(Random.Range(2, 5));

            if(CheckWall()) //if started idle while next to a wall
            {
                TurnAround();
            }

            //move
            //animator.SetBool("isRunning", true); //running for 1 second

            while (!hitWall) // loop until hit a wall or edge
            {
                desired_velocity = new Vector2(direction.x, 0f) * Mathf.Max(max_speed, 0f);
                velocity = rb.velocity;
                acceleration = max_acceleration;
                max_speed_change = acceleration * Time.deltaTime;

                if (!receivingKnockback)
                {
                    velocity.x = Mathf.MoveTowards(velocity.x, desired_velocity.x, max_speed_change);
                }

                rb.velocity = velocity;

                if (CheckWall()) 
                {
                    TurnAround();
                    hitWall = true;
                }

                yield return new WaitForEndOfFrame();
            }
        }
    }

    IEnumerator Agro()
    {
        while (true)
        {
            //start animation
            //animator.SetBool("isRunning", true);

            //start jumping part of animation

            //1 if player is to the right, -1 if player is to the left
            FacePlayer();

            //moves toward the player
            desired_velocity = new Vector2(direction.x, direction.y) * Mathf.Max(max_speed, 0f);
            velocity = rb.velocity;
            acceleration = max_acceleration;
            max_speed_change = acceleration * Time.deltaTime;

            if (!receivingKnockback)
            {
                velocity.x = Mathf.MoveTowards(velocity.x, desired_velocity.x, max_speed_change);
                velocity.y = Mathf.MoveTowards(velocity.y, desired_velocity.y, max_speed_change);
            }

            if (CheckEdge() && velocity.y < 0) //checks for ground, if close to ground and trying to move down then don't move down
            {
                velocity.y = 0;
            }

            rb.velocity = velocity;

            yield return new WaitForSeconds(0);
        }

    }

    public override void FacePlayer()
    {
        Vector3 rotation = transform.eulerAngles;
        direction.x = (int)Mathf.Sign(PlayerController.player.transform.position.x - transform.position.x);
        if (PlayerController.player.transform.position.y - transform.position.y < -.1f &&
            PlayerController.player.transform.position.y - transform.position.y > -.4f)
        {
            direction.y = 0;
        } else
        {
            direction.y = (int)Mathf.Sign(PlayerController.player.transform.position.y - transform.position.y);
        }

        if (direction.x == -1)
        {
            rotation.y = 180f;
        } else if (direction.x == 1)
        {
            rotation.y = 0f;
        }
        transform.eulerAngles = rotation;
        velocity.x = 0;
    }

    public override bool CheckWall() // returns true if there is a wall, false if there is no wall
    {
        //return Physics2D.OverlapCircle(direction.x == -1 ? wallChecker1.position : wallChecker2.position, checkGroundRadius, wallLayer) != null;
        return Physics2D.OverlapCircle(wallChecker2.position, checkGroundRadius, wallLayer) != null || Physics2D.OverlapCircle(wallChecker1.position, checkGroundRadius, wallLayer) != null;
    }

}
