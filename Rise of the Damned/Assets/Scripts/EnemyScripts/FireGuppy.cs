using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireGuppy : EnemyController
{

    private Coroutine stateUpdate; //calls the coroutine for the state of the enemy
    bool isIdle = true;
    bool isAttacking = false;

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
        } else if (Vector2.Distance(rb.position, PlayerController.controller.rb.position) < aggroDist) //far enough away from player
        {
            state = State.Attack;
        }

    }

    public override void Wander()
    {
        if (!isIdle) // is not already idle and not currently swinging
        {
            isIdle = true;
            isAttacking = false;
            if (stateUpdate != null) { StopCoroutine(stateUpdate); }
            stateUpdate = StartCoroutine(Idle());
        }
    }

    public override void Attack()
    {
        if (!isAttacking) // is not already attacking and not currently swinging
        {
            isAttacking = true;
            isIdle = false;
            if (stateUpdate != null) { StopCoroutine(stateUpdate); }
            stateUpdate = StartCoroutine(Agro());
        }
    }

    IEnumerator Idle()
    {
        FacePlayer();
        animator.SetBool("isAttacking", false); // ends swinging animation

        animator.SetBool("PopIn", true); // goes back into the ground
        animator.SetBool("PopIn", false);

        yield return new WaitForSeconds(0);
    }

    IEnumerator Agro()
    {
        animator.SetBool("PopOut", true); // comes out of the ground
        animator.SetBool("PopOut", false);

        yield return new WaitForSeconds(2);

        animator.SetBool("isAttacking", true);
        animator.SetBool("isAttacking", false);

        while (true)
        {
            FacePlayer();
            //shoot at player
        }
        yield return new WaitForSeconds(0);
    }

}
