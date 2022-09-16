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

    new public void Wander()
    {
        if (!CheckGround() || CheckWall())
            TurnAround();

        if (isAttacking)
        {
            isAttacking = false;
            if (stateUpdate != null) { StopCoroutine(stateUpdate); }
            stateUpdate = StartCoroutine(Idle());
        }
        if (Vector2.Distance(rb.position, PlayerController.controller.rb.position) < aggroDist && !isAttacking)
        {
            state = State.Attack;
        }
    }

    new public void Attack()
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
                collision.attachedRigidbody.velocity += new Vector2(Mathf.Sign(collision.transform.position.x - transform.position.x) * knockback * -2, knockback / 2);
        }

    }

    IEnumerator Agro() {
        while (true) {
            //start animation
            animator.SetBool("isRunning", true);

            //start jumping part of animation

            //1 if player is to the right, -1 if player is to the left
            //direction = (this.transform.position.x < player.position.x) ? 1 : -1;
            FacePlayer();

            //moves toward the player
            rb.velocity = (new Vector2(speed * direction, 0));

            yield return new WaitForSeconds(0);
        }

    }

    IEnumerator Idle() {
        direction = 1;

        while (true) {
            //beginning animation
            animator.SetBool("isRunning", false);

            yield return new WaitForSeconds(6);

            //move
            animator.SetBool("isRunning", true);
            rb.velocity = (new Vector2(speed * direction, 0));
            //change direction
            direction *= -1;
            //wait
            animator.SetBool("isRunning", false);


                yield return new WaitForSeconds(2);

            animator.SetBool("isRunning", true);
            rb.velocity = (new Vector2(-rb.velocity.x, -rb.velocity.y));
            yield return new WaitForSeconds(1);
        }
    }

    private void CheckTurnAround(int direction) {
        if ((!CheckGround()) || CheckWall()) {
            direction *= -1;
        }
    }


}
