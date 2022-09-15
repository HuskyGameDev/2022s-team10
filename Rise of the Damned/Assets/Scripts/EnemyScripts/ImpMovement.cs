using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpMovement : EnemyController
{
    //[Header("Enemy Stats")]
    //public float agroSpeed;

    private Coroutine stateUpdate; //calls the coroutine for the state of the enemy
    [SerializeField]
    bool isAttacking = true;
    int direction = 1;

    [Header("Animations")]
    public Animator animator;

    private float redTime = 0;  //the time that the enemy is red

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        state = defaultState;

        float sum = 0;
        foreach (float i in dropChance)
            sum += i;
        if (sum > 100)
            Debug.LogError("Drop chances exceed 100% (was " + sum + "% for " + dropChance.Length + " items).");
        if (dropChance.Length != drops.Length)
            Debug.LogError("Drop chances are not parallel to drops (was " + dropChance.Length + "items, expected " + drops.Length + ").");
    }

    // Update is called once per frame
    void Update()
    {

        //switches the way the sprite is facing
        /*if (this.transform.position.x < PlayerController.controller.rb.transform.position.x) {
            this.transform.localScale = new Vector2(1, 1);
        } else {
            this.transform.localScale = new Vector2(-1, 1);
        } */ //old version that is always facing toward the player at all times

        this.transform.localScale = new Vector2(direction, 1);

        switch (state)  //do different things based on the current state
        {
            case State.Wander:
                CheckTurnAround(direction);

                if (isAttacking) {
                    isAttacking = false;
                    if (stateUpdate != null) { StopCoroutine(stateUpdate); }
                    stateUpdate = StartCoroutine(Idle());
                }
                if (Vector2.Distance(rb.position, PlayerController.controller.rb.position) < aggroDist && !isAttacking) {
                    state = State.Attack;
                }
                break;
            case State.Attack:
                if (!isAttacking) {
                    isAttacking = true;
                    if (stateUpdate != null) { StopCoroutine(stateUpdate); }
                    stateUpdate = StartCoroutine(Agro(PlayerController.controller.rb.transform));
                }

                if (Vector2.Distance(rb.position, PlayerController.controller.rb.position) > aggroDist * 1.5 && isAttacking) {
                    state = defaultState;
                }
                break;
            case State.Stay:
                break;
            default:
                break;
        }

        if (health < 0) {
            Destroy(gameObject);

            float diceRoll = (float)Random.Range(1, 10001) / 100f;   //1-100 dice roll with 2 point precision
            float chanceTotal = 0;  //sum of drop chances prior to the current drop

            for (int i = 0; i < drops.Length; i++) {
                if (chanceTotal + dropChance[i] >= diceRoll)    //if the dice roll is in the current range 
                {
                    Instantiate(drops[i], new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
                    break;
                }
                chanceTotal += dropChance[i];
            }

        }

        if (redTime > 0) {
            redTime -= Time.deltaTime;
            if (redTime <= 0)
                sr.color = Color.white;
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            if (PlayerController.TakeDamage(damage))
                collision.attachedRigidbody.velocity += new Vector2(Mathf.Sign(collision.transform.position.x - transform.position.x) * knockback * -2, knockback / 2);
        }

    }

    IEnumerator Agro(Transform player) {
        while (true) {
            //start animation
            animator.SetBool("isRunning", true);

            //start jumping part of animation

            //1 if player is to the right, -1 if player is to the left
            direction = (this.transform.position.x < player.position.x) ? 1 : -1;

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
        if ((!CheckGround(direction)) || CheckWall(direction)) {
            direction *= -1;
        }
    }

    public bool CheckGround(int direction) {
        return Physics2D.OverlapCircle(direction == -1 ? groundChecker1.position : groundChecker2.position, checkGroundRadius, groundLayer) != null;
    }
    private bool CheckWall(int direction) {
        return Physics2D.OverlapCircle(direction == -1 ? wallChecker1.position : wallChecker2.position, checkGroundRadius, wallLayer + groundLayer) != null;
    }

}
