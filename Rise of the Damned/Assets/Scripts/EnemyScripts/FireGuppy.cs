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

    [SerializeField]
    private GameObject fireball;
    [SerializeField]
    private Transform FireballSpawnPos;

    [SerializeField]
    private float projDmg;
    [SerializeField]
    private int bounceLimit = 3;

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

        animator.SetTrigger("PopIn"); // goes back into the ground

        yield return new WaitForSeconds(0);
    }

    IEnumerator Agro()
    {
        FacePlayer();
        animator.SetTrigger("PopOut"); // comes out of the ground

        yield return new WaitForSeconds(1);

        while (true)
        {
            FacePlayer();

            animator.SetTrigger("Attack"); // start attack animation

            yield return new WaitForSeconds(8f / 15); // waits till the animation from where the guppy spits the fireball
            //spit the fireball

            GameObject spit = Instantiate(fireball, FireballSpawnPos.position, Quaternion.identity);
            spit.GetComponent<EnemyProjController>().damage = projDmg;
            spit.GetComponent<BouncyFireBall>().direction = (int) direction.x;
            spit.GetComponent<BouncyFireBall>().bounceLimit = bounceLimit;

            yield return new WaitForSeconds(attackCooldown); // wait for attack cooldown
        }
    }


    public override void Knockback(float knockback, Transform knockback_location)
    {
        // does not receive knockback
    }
}
