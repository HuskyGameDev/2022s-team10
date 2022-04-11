using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Enemy Stats")]
    public float damage;
    public float health;
    public float knockback;

    [SerializeField]
    private float speed;

    public enum State { Wander, Attack, Stay };
    [Header("Enemy AI")]
    [SerializeField]
    private State defaultState;
    [SerializeField] 
    private float aggroDist;
    private State state;
    public bool flying;
    private int wanderDir = 1;  //1 for right, -1 for left

    [Header("Item Drops")]
    [SerializeField]
    private GameObject[] drops;
    [SerializeField]
    private float[] dropChance;
    [SerializeField]
    private float heartDropChance;
    [SerializeField]
    private GameObject heartDrop;

    [Header("Projectiles")]
    [SerializeField]
    private GameObject FireBall;
    [SerializeField]
    private float cooldown;
    private float cooldownTime;

    [Header("For Ground")]
    public float checkGroundRadius; // is going to tell us whats the radius of our GroundChecker
    public Transform groundChecker1, groundChecker2, wallChecker1, wallChecker2; // Transform of an empty object that is going to be placed bellow player
    public LayerMask groundLayer, wallLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        state = defaultState;
        cooldownTime = cooldown / 2;

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
        switch (state)  //do different things based on the current state
        {
            case State.Wander:
                wander();
                CheckTurnAround();
                if (Vector2.Distance(rb.position, PlayerController.controller.rb.position) < aggroDist)
                {
                    state = State.Attack;
                }
                break;
            case State.Attack:
                if (name.Contains("Large Imp") || name.Contains("Reaper"))
                {
                    GetComponent<SpriteRenderer>().flipX = PlayerController.player.transform.position.x < transform.position.x;
                    wanderDir = (int)Mathf.Sign(PlayerController.player.transform.position.x - transform.position.x);
                }

                if (!CheckWall())
                    transform.position = Vector2.MoveTowards(transform.position, 
                        new Vector2(PlayerController.player.transform.position.x, flying ? PlayerController.player.transform.position.y : transform.position.y), 
                        Time.deltaTime * speed);
                
                if (Vector2.Distance(rb.position, PlayerController.controller.rb.position) > aggroDist * 1.5)
                {
                    state = defaultState;
                }
                break;
            case State.Stay:
                if (Vector2.Distance(rb.position, PlayerController.controller.rb.position) < aggroDist)
                {
                    state = State.Attack;
                }
                break;
        }



        if (health <= 0)
        {
            Destroy(gameObject);

            float diceRoll = (float)Random.Range(1, 10001) / 100f;   //1-100 dice roll with 2 point precision
            float chanceTotal = 0;  //sum of drop chances prior to the current drop

            for (int i = 0; i < drops.Length; i++)
            {
                if (chanceTotal + dropChance[i] >= diceRoll)    //if the dice roll is in the current range 
                {
                    Instantiate(drops[i], transform.position, Quaternion.identity);
                    break;
                }
                chanceTotal += dropChance[i];
            }

            diceRoll = (float)Random.Range(1, 10001) / 100f;
            if (diceRoll < heartDropChance)
                Instantiate(heartDrop, transform.position, Quaternion.identity);

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

        if (name.Contains("Demon Skull"))
        {   
            if (cooldownTime >= cooldown)
            {
                Vector3 rayDir = PlayerController.player.transform.position - transform.position;
                if (!Physics2D.Raycast(transform.position, rayDir, Vector2.Distance(PlayerController.player.transform.position, transform.position), groundLayer))
                { 
                     GameObject shoot = Instantiate(FireBall, transform.position, Quaternion.identity);
                     shoot.GetComponent<EnemyProjController>().enemy = gameObject;
                }
                //float shootAngle = Vector2.Angle(transform.position, PlayerController.player.transform.position);

                //shoot.GetComponent<Rigidbody2D>().rotation = shootAngle;
                //shoot.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(shootAngle * Mathf.Deg2Rad) * 10, Mathf.Sin(shootAngle * Mathf.Deg2Rad) * 10);
                cooldownTime = 0;
            }
            cooldownTime += Time.deltaTime;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if(PlayerController.TakeDamage(damage))
                collision.attachedRigidbody.velocity += new Vector2(Mathf.Sign(collision.transform.position.x - transform.position.x) * knockback * -2, knockback / 2);
        }

    }
    private void wander()   
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + wanderDir, transform.position.y), Time.deltaTime * speed);
               
    }

    private void CheckTurnAround()
    {
        if ((!flying && !CheckGround()) || CheckWall())
        {
            wanderDir *= -1;
            /*if(name.Contains("Large Imp"))
            {
                GetComponent<SpriteRenderer>().flipX = wanderDir == -1;
            }*/
        }
    }

    private bool CheckGround()
    {
        return Physics2D.OverlapCircle(wanderDir == -1 ? groundChecker1.position : groundChecker2.position, checkGroundRadius, groundLayer) != null;
    }
    private bool CheckWall()
    {
        return Physics2D.OverlapCircle(wanderDir == -1 ? wallChecker1.position : wallChecker2.position, checkGroundRadius, wallLayer + groundLayer) != null;
    }
}
