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
    bool isGrounded = false;

    [Header("Animations")]
    public Animator animator;

    // Update2 is called once per frame
    public override void Update2()
    {
        if (CheckGround())
        {
            if (isJumping)
            {
                rb.velocity = (new Vector2(0, 0));
            }
            isGrounded = true;
            isJumping = false;
            animator.SetBool("isJumping", false);
        }
        else
        {
            isGrounded = false;
            isJumping = true;
            animator.SetBool("isPrejumping", false);
            animator.SetBool("isJumping", true);
        }
    }

    new public void Wander()
    {
        if (isAttacking)
        {
            isAttacking = false;
            if (slimeUpdate != null) { StopCoroutine(slimeUpdate); }
            slimeUpdate = StartCoroutine(Idle());
        }
        if (Vector2.Distance(rb.position, PlayerController.controller.rb.position) < aggroDist && !isAttacking)
        {
            state = State.Attack;
        }
    }

    new public void Attack()
    {
        FacePlayer();

        if (!isAttacking)
        {
            isAttacking = true;
            if (slimeUpdate != null) { StopCoroutine(slimeUpdate); }
            slimeUpdate = StartCoroutine(Agro());
        }

        if (Vector2.Distance(rb.position, PlayerController.controller.rb.position) > aggroDist * 1.5 && isAttacking)
        {
            state = defaultState;
        }
    }

    IEnumerator Agro()
    {
        while (true)
        {
            //start animation
            animator.SetBool("isPrejumping", true);

            yield return new WaitForSeconds(1);

            //start jumping part of animation

            //1 if player is to the right, -1 if player is to the left
            FacePlayer();
            //Jump towards player
            rb.velocity = (new Vector2(agroSpeed * direction, jumpSpeed));

            yield return new WaitForSeconds(2);
        }

    }

    IEnumerator Idle()
    {
        //int direction = 1;

        while (true)
        {
            //beginning animation

            yield return new WaitForSeconds(2);

            //move
            rb.velocity = (new Vector2(speed * direction, 0));
            //change direction
            //direction *= -1;
            //wait
            while(!isGrounded) {
                yield return new WaitForSeconds(1);
            }
            rb.velocity = (new Vector2(-rb.velocity.x, -rb.velocity.y));
            yield return new WaitForSeconds(1);
        }
    }
}
