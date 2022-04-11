using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Enemy Stats")]
    public int damage;
    public float health;
    public float knockback;
    public float jumpSpeed;
    public float agroSpeed;

    private Coroutine slimeUpdate;
    [SerializeField]
    bool isAttacking = true;
    [SerializeField]
    bool isJumping = false;
    [SerializeField]
    bool isGrounded = false;

    [SerializeField]
    private float speed;

    [Header("Enemy AI")]
    [SerializeField]
    private string defaultState;
    [SerializeField]
    private float aggroDist;
    private string state;
    public bool flying;

    [SerializeField]
    private Vector2[] positions;
    private int index = 0;

    [Header("Item Drops")]
    [SerializeField]
    private GameObject[] drops;
    [SerializeField]
    private float[] dropChance;

    [Header("Projectiles")]
    [SerializeField]
    private GameObject FireBall;

    [Header("For Ground")]
    public float rememberGroundedFor; // help to keep us grounded for a little longer, smooth out jumps just after leaving ground
    public float checkGroundRadius; // is going to tell us whats the radius of our GroundChecker
    public Transform isGroundedChecker; // Transform of an empty object that is going to be placed bellow player
    public LayerMask groundLayer;
    float lastTimeGrounded; // when was the last time we were standing on the ground

    [Header("Animations")]
    public Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

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

        CheckIfGrounded();

        switch (state)  //do different things based on the current state
        {
            case "wander":
                if (isAttacking)
                {
                    isAttacking = false;
                    if (slimeUpdate != null) { StopCoroutine(slimeUpdate); }
                    slimeUpdate = StartCoroutine(Idle());
                }
                if (Vector2.Distance(rb.position, PlayerController.controller.rb.position) < aggroDist && !isAttacking)
                {
                    state = "attack";
                }
                break;
            case "attack":
                if (!isAttacking)
                {
                    isAttacking = true;
                    if (slimeUpdate != null) { StopCoroutine(slimeUpdate); }
                    slimeUpdate = StartCoroutine(Agro(PlayerController.controller.rb.transform));
                }

                if (Vector2.Distance(rb.position, PlayerController.controller.rb.position) > aggroDist * 1.5 && isAttacking)
                {
                    state = defaultState;
                }
                break;
            case "stay":
                break;
            default:
                break;
        }



        if (health < 0)
        {
            Destroy(gameObject);

            float diceRoll = (float)Random.Range(1, 10001) / 100f;   //1-100 dice roll with 2 point precision
            float chanceTotal = 0;  //sum of drop chances prior to the current drop

            for (int i = 0; i < drops.Length; i++)
            {
                if (chanceTotal + dropChance[i] >= diceRoll)    //if the dice roll is in the current range 
                {
                    Instantiate(drops[i], new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
                    break;
                }
                chanceTotal += dropChance[i];
            }

            /*if (diceRoll <= itemOnePercentChance)
            {
                Instantiate(itemOne, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
            }
            else if (diceRoll <= itemTwoPercentChance + itemTwoPercentChance)
            {
                Instantiate(itemTwo, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
            }
            else
            {
                Instantiate(itemThree, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
            }*/

        }

        if (gameObject.name == "demon skull")
        {
            GameObject shoot = Instantiate(FireBall, transform.position, Quaternion.identity);
            float shootAngle = Vector2.Angle(transform.position, PlayerController.player.transform.position);

            shoot.GetComponent<Rigidbody2D>().rotation = shootAngle;
            shoot.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(shootAngle * Mathf.Deg2Rad) * 10, Mathf.Sin(shootAngle * Mathf.Deg2Rad) * 10);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (PlayerController.TakeDamage(damage))
                collision.attachedRigidbody.velocity += new Vector2(Mathf.Sign(collision.transform.position.x - transform.position.x) * knockback * -2, knockback / 2);
        }

    }
    private void wander()   //just threw the previous code in here for now
    {
        transform.position = Vector2.MoveTowards(transform.position, !flying ? new Vector2(positions[index].x, transform.position.y) : positions[index], Time.deltaTime * speed);

        if (Mathf.Abs(transform.position.x - positions[index].x) < .1)
        {
            if (index == positions.Length - 1)
            {

                index = 0;
            }
            else
            {
                index++;
            }
        }

    }

    IEnumerator Agro(Transform player)
    {
        while (true)
        {
            //start animation
            yield return new WaitForSeconds(2);

            //start jumping part of animation

            //1 if player is to the right, -1 if player is to the left
            int direction = (this.transform.position.x < player.position.x) ? 1 : -1;
            //Jump towards player
            rb.velocity = (new Vector2(agroSpeed * direction, jumpSpeed));

            yield return new WaitForSeconds(2);
        }

    }

    IEnumerator Idle()
    {
        int direction = 1;

        while (true)
        {
            //beginning animation

            yield return new WaitForSeconds(2);

            //move
            rb.velocity = (new Vector2(speed * direction, 0));
            //change direction
            direction *= -1;
            //wait
            while(!isGrounded) {
                yield return new WaitForSeconds(1);
            }
            rb.velocity = (new Vector2(-rb.velocity.x, -rb.velocity.y));
            yield return new WaitForSeconds(1);
        }
    }

    void CheckIfGrounded() {
        Collider2D collider = Physics2D.OverlapCircle(isGroundedChecker.position, checkGroundRadius, groundLayer);
        if(collider != null) {
            if(isJumping) {
                rb.velocity = (new Vector2(0, 0));
            }
                isGrounded = true;
                isJumping = false;
            animator.SetBool("isJumping", false);
        } else {
            if (isGrounded) {
                lastTimeGrounded = Time.time; // Time.time holds how much time has passed since we are running our game
            }
                isGrounded = false;
                isJumping = true;
            animator.SetBool("isJumping", true);
        }
    }


}
