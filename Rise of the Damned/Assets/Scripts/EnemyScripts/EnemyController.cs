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
    public float knockback_resistance = 1f; // divides knockback by resistance
    [SerializeField]
    protected float max_speed = 3f;
    public enum State { Wander, Attack, Stay, Swing };
    [Header("Enemy AI")]
    public State defaultState;
    public float aggroDist;
    [System.NonSerialized]
    public State state;
    public bool flying;

    [Header("Movement")]
    [SerializeField]
    protected float max_acceleration = 30f;
    protected Vector2 direction, desired_velocity, velocity;
    protected float acceleration;
    [SerializeField]
    protected float max_speed_change;
    [SerializeField]
    protected float knockback_time = 1.5f; //time in seconds knockback is in effect 
    [SerializeField]
    protected float knockback_drag = 3f;
    protected bool receivingKnockback = false;

    [Header("Item Drops")]
    public GameObject[] drops;
    public float[] dropChance;
    public float heartDropChance;
    public GameObject heartDrop;

    

    [Header("For Ground")]
    public float checkGroundRadius; // is going to tell us whats the radius of our GroundChecker
    public Transform groundChecker1, groundChecker2, wallChecker1, wallChecker2; // Transform of an empty object that is going to be placed bellow player
    public LayerMask groundLayer, wallLayer;

    public Transform collisionPoint; // point at the center bottom of the sprite to send to knockback function


    private float redTime = 0;  //the time that the enemy is red

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        state = defaultState;

        direction.x = 1;

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
            case State.Swing: //for attack animation
                Swinging();
                break;
            case State.Stay:
                Stay();
                break;
            default:
                Substate();
                break;
        }


        IFrames();

        KnockbackDrag();

        this.Update2();
    }

    public virtual void Update2()
    {
        if (Vector2.Distance(rb.position, PlayerController.controller.rb.position) > aggroDist * 1.5)
        {
            state = defaultState;
        } 
        else if (Vector2.Distance(rb.position, PlayerController.controller.rb.position) < aggroDist)
        {
            state = State.Attack;
        }
    }

    public void Substate() { }

    public void IFrames()
    {
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
    }

    public void Die()
    {
        //Debug.Log("Drop Stuff");
        float diceRoll = (float)Random.Range(1, 10001) / 100f;   //1-100 dice roll with 2 point precision
        float chanceTotal = 0;  //sum of drop chances prior to the current drop

        for (int i = 0; i < drops.Length; i++)
        {
            //Debug.Log("Checking drop " + i);
            if (chanceTotal + dropChance[i] >= diceRoll)    //if the dice roll is in the current range 
            {
                GameObject drop = Instantiate(drops[i], transform.parent.parent);
                drop.transform.position = new Vector3(transform.position.x, transform.position.y, 0);
                //Debug.Log("Dropping " + i);
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

        Destroy(gameObject);
    }


    public virtual void Attack()
    {
        //if (name.Contains("Large Imp") || name.Contains("Reaper"))
        //{
        FacePlayer();
        
        //}

        if (!CheckWall())
        {
            /*
            transform.position = Vector2.MoveTowards(transform.position,
                new Vector2(PlayerController.player.transform.position.x, flying ? PlayerController.player.transform.position.y : transform.position.y),
                Time.deltaTime * speed);
            */
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
            

        
    }

    public void Stay()
    {
        desired_velocity = new Vector2(0f, 0f);
        if (!receivingKnockback)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, desired_velocity.x, max_speed_change);
        }

        rb.velocity = velocity;

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (PlayerController.TakeDamage(damage))
                PlayerController.controller.Knockback(knockback, collisionPoint);
        }
        if (collision.gameObject.CompareTag("Spike")) //spike knockback
        {
            if(collision.GetComponent<GroundSpikes>() != null)
            {
                TakeDamage(collision.GetComponent<GroundSpikes>().damage / 3);
                rb.velocity = new Vector2(0, 0);
                SpikeKnockback(collision.GetComponent<GroundSpikes>().knockback / 2, new Vector2(0, 1));
            }
        }
        if (collision.gameObject.CompareTag("Lava")) //lava knockback
        {
            if (collision.GetComponent<Lava>() != null)
            {
                TakeDamage(collision.GetComponent<Lava>().damage / 3);
                rb.velocity = new Vector2(0, 0);
                SpikeKnockback(collision.GetComponent<Lava>().knockback / 4, new Vector2(0, 1));            }
        }
    }

    public virtual void Wander()
    {
        //transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + direction.x, transform.position.y), Time.deltaTime * speed);

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
            TurnAround();
        
    }
    
    public virtual void Swinging()
    {

    }

    public void TurnAround()
    {
        direction.x *= -1;
        //GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX; only flips the sprite and not hit boxes of animations
        Vector3 rotation = transform.eulerAngles;
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

    public virtual void FacePlayer()
    {
        Vector3 rotation = transform.eulerAngles;
        direction.x = (int)Mathf.Sign(PlayerController.player.transform.position.x - transform.position.x);

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

    public bool CheckGround()
    {
        if (Physics2D.OverlapCircle(groundChecker1.position, checkGroundRadius, groundLayer) != null)
            return true;
        else
            return Physics2D.OverlapCircle(groundChecker2.position, checkGroundRadius, groundLayer) != null;
    }
    public virtual bool CheckWall() // returns true if there is a wall, false if there is no wall
    {
        //return Physics2D.OverlapCircle(direction.x == -1 ? wallChecker1.position : wallChecker2.position, checkGroundRadius, wallLayer) != null;
        return Physics2D.OverlapCircle(wallChecker2.position, checkGroundRadius, wallLayer) != null;
    }
    public bool CheckEdge() // returns false if on an edge true if not near an edge
    {
        //return Physics2D.OverlapCircle(direction.x == -1 ? groundChecker1.position : groundChecker2.position, checkGroundRadius, groundLayer) != null;
        return Physics2D.OverlapCircle(groundChecker2.position, checkGroundRadius, groundLayer) != null;
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

    private Coroutine knocked; //knockback coroutine
    public virtual void Knockback(float knockback, Transform knockback_location) //knockback location is where the knockback is coming from
    {
        if(!receivingKnockback) //only receive knockback if not already receiving knockback
        {
            receivingKnockback = true;
            float horizontal_enemy_direction = (knockback_location.position.x - rb.position.x) / Mathf.Abs(knockback_location.position.x - rb.position.x); // horizontal vector distance from player to enemy
            float vertical_enemy_direction = (knockback_location.position.y - rb.position.y) / Mathf.Abs(knockback_location.position.y - rb.position.y);

            rb.velocity = new Vector2(0, 0); //stops all enemy movement right before knockback so that knockback is more consistent
            rb.AddForce(new Vector2((knockback / knockback_resistance) * -horizontal_enemy_direction, (knockback / knockback_resistance) * 1 * (float).45), ForceMode2D.Impulse);
            knocked = StartCoroutine(KnockbackSequence());
        }
    }
    public virtual void SpikeKnockback(float knockback, Vector2 knockbackdir) //knockback direction is the direction the entity will be knocked back
    {
        receivingKnockback = true;

        rb.AddForce(new Vector2(knockback * knockbackdir.x, knockback * knockbackdir.y), ForceMode2D.Impulse);
        knocked = StartCoroutine(KnockbackSequence());
    }
    IEnumerator KnockbackSequence()
    {
        yield return new WaitForSeconds(knockback_time);
        receivingKnockback = false;
        yield return new WaitForSeconds(0);
    }

    public virtual void KnockbackDrag()
    {
        if(receivingKnockback)
        {
            rb.drag = knockback_drag;
        } else
        {
            rb.drag = 0;
        }
    }
}
