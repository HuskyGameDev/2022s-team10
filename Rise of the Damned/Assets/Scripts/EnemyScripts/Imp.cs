using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Imp : EnemyController
{
    //[Header("Enemy Stats")]
    //public float agroSpeed;

    private Coroutine stateUpdate; //calls the coroutine for the state of the enemy
    [SerializeField]
    bool isAttacking = true;

    [Header("Animations")]
    public Animator animator;


       // Update is called once per frame
    public override void Update2()
    {

        //switches the way the sprite is facing
        /*if (this.transform.position.x < PlayerController.controller.rb.transform.position.x) {
            this.transform.localScale = new Vector2(1, 1);
        } else {
            this.transform.localScale = new Vector2(-1, 1);
        } */ //old version that is always facing toward the player at all times

        //this.transform.localScale = new Vector2(direction, 1);

    }

    public override void Wander()
    {
        //if (!CheckGround() || CheckWall())
        //    TurnAround();

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

        if (Vector2.Distance(rb.position, PlayerController.controller.rb.position) > aggroDist * 1.5 && isAttacking)
        {
            state = defaultState;
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            if (PlayerController.TakeDamage(damage))
            {
                PlayerController.controller.Knockback(knockback, gameObject.transform);
            }
        }
    }

    IEnumerator Agro() {
        while (true) {
            //start animation
            animator.SetBool("isRunning", true);

            //start jumping part of animation

            //1 if player is to the right, -1 if player is to the left
            FacePlayer();

            if (Mathf.Abs(rb.position.x - PlayerController.controller.rb.position.x) < .5) 
                    { //sets imp to idle if it is under or over the player
                state = State.Wander;
            } else { //moves toward the player
                desired_velocity = new Vector2(direction.x, 0f) * Mathf.Max(max_speed, 0f);
                velocity = rb.velocity;
                acceleration = max_acceleration;
                max_speed_change = acceleration * Time.deltaTime;

                if (!receivingKnockback)
                {
                    velocity.x = Mathf.MoveTowards(velocity.x, desired_velocity.x, max_speed_change);
                }

                rb.velocity = velocity;
            }

            yield return new WaitForSeconds(0);
        }

    }

    IEnumerator Idle() {
        direction.x = 1;
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

                if (!receivingKnockback)
                {
                    velocity.x = Mathf.MoveTowards(velocity.x, desired_velocity.x, max_speed_change);
                }

                rb.velocity = velocity;

                if (!CheckEdge() || CheckWall())
                {
                    TurnAround();
                    hitEdge = true;
                }

                yield return new WaitForEndOfFrame();
            }
        }
    }

    public new void TurnAround()
    {
        direction.x *= -1;
        GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
        velocity.x = 0;
    }


}
