using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public abstract class EnemyController : MonoBehaviour
{
    [System.NonSerialized]
    public Rigidbody2D rb;
    [System.NonSerialized]
    public SpriteRenderer sr;

    [Header("Enemy Stats")]

    public float damage;
    public float health;
    public float knockback;
    public float speed;

    public enum State { Wander, Attack, Stay };
    [Header("Enemy AI")]
    public State defaultState;
    public float aggroDist;
    [System.NonSerialized]
    public State state;
    public bool flying;
    public int direction = 1;  //1 for right, -1 for left

    [Header("Item Drops")]
    public GameObject[] drops;
    public float[] dropChance;
    public float heartDropChance;
    public GameObject heartDrop;

    

    [Header("For Ground")]
    public float checkGroundRadius; // is going to tell us whats the radius of our GroundChecker
    public Transform groundChecker1, groundChecker2, wallChecker1, wallChecker2; // Transform of an empty object that is going to be placed bellow player
    public LayerMask groundLayer, wallLayer;

    [SerializeField]
    private float knockback_time = 1; //time in seconds knockback is in effect 
    private bool receivingKnockback = false;

    private float redTime = 0;  //the time that the enemy is red

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
        switch (state)  //do different things based on the current state
        {
            case State.Wander:
                Wander();
                break;
            case State.Attack:
                Attack();
                break;
            case State.Stay:
                Stay();
                break;
            default:
                Substate();
                break;
        }



        if (health <= 0)
        {
            Die();
        }

        if (redTime > 0)
        {
            redTime -= Time.deltaTime;
            if (redTime <= 0)
                sr.color = Color.white;
        }

        this.Update2();
    }

    public abstract void Update2();
    public void Substate() { }

    public void Die()
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


    public virtual void Attack()
    {
        //if (name.Contains("Large Imp") || name.Contains("Reaper"))
        //{
        FacePlayer();
        
        //}

        if (!CheckWall())
            transform.position = Vector2.MoveTowards(transform.position,
                new Vector2(PlayerController.player.transform.position.x, flying ? PlayerController.player.transform.position.y : transform.position.y),
                Time.deltaTime * speed);

        if (Vector2.Distance(rb.position, PlayerController.controller.rb.position) > aggroDist * 1.5)
        {
            state = defaultState;
        }
    }

    public void Stay()
    {
        if (Vector2.Distance(rb.position, PlayerController.controller.rb.position) < aggroDist)
        {
            state = State.Attack;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (PlayerController.TakeDamage(damage))
                PlayerController.controller.Knockback(knockback, gameObject.transform);
        }
    }

    public virtual void Wander()
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + direction, transform.position.y), Time.deltaTime * speed);
        if ((!flying && !CheckGround()) || CheckWall())
            TurnAround();
        if (Vector2.Distance(rb.position, PlayerController.controller.rb.position) < aggroDist)
        {
            state = State.Attack;
        }
    }

    public void TurnAround()
    {
            direction *= -1;
            GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
    }

    public void FacePlayer()
    {
        GetComponent<SpriteRenderer>().flipX = PlayerController.player.transform.position.x < transform.position.x;
        direction = (int)Mathf.Sign(PlayerController.player.transform.position.x - transform.position.x);
    }

    public bool CheckGround()
    {
        if (Physics2D.OverlapCircle(groundChecker1.position, checkGroundRadius, groundLayer) != null)
            return true;
        else
            return Physics2D.OverlapCircle(groundChecker2.position, checkGroundRadius, groundLayer) != null;
    }
    public bool CheckWall()
    {
        return Physics2D.OverlapCircle(direction == -1 ? wallChecker1.position : wallChecker2.position, checkGroundRadius, wallLayer + groundLayer) != null;
    }

    public void TakeDamage(float damage)
    {
        if (damage > 0)
        {
            health -= damage;
            redTime = .2f;
            sr.color = Color.red;
        }
    }

    public bool CheckEdge()
    {
        return Physics2D.OverlapCircle(direction == -1 ? groundChecker1.position : groundChecker2.position, checkGroundRadius, groundLayer) != null;
    }

    public void Knockback(float knockback, Transform knockback_location)
    {
        receivingKnockback = true;
        float horizontal_enemy_direction = (knockback_location.position.x - rb.position.x) / Mathf.Abs(knockback_location.position.x - rb.position.x); // horizontal vector distance from player to enemy
        float vertical_enemy_direction = (knockback_location.position.y - rb.position.y) / Mathf.Abs(knockback_location.position.y - rb.position.y);

        rb.AddForce(new Vector2(knockback * -horizontal_enemy_direction, knockback * -vertical_enemy_direction * (float).75), ForceMode2D.Impulse);
        Invoke("SetReceivingKnockback", knockback_time);
    }
    private void SetReceivingKnockback()
    {
        receivingKnockback = false;
    }
}
