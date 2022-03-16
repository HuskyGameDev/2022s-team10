using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Enemy Stats")]
    public int damage;
    public float health;
    public float knockback;

    [SerializeField]
    private float speed;

    [Header("Enemy AI")]
    [SerializeField]
    private string defaultState;
    [SerializeField] 
    private float aggroDist;
    private string state;

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
        switch (state)  //do different things based on the current state
        {
            case "wander":
                wander();
                if (Vector2.Distance(rb.position, PlayerController.controller.rb.position) < aggroDist)
                {
                    state = "attack";
                }
                break;
            case "attack":
                transform.position = Vector2.MoveTowards(transform.position, PlayerController.player.transform.position, Time.deltaTime * speed);

                if (Vector2.Distance(rb.position, PlayerController.controller.rb.position) > aggroDist * 1.5)
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
        if (collision.gameObject.tag == "Player")
        {
            if(PlayerController.takeDamage(damage))
                collision.attachedRigidbody.velocity += new Vector2(Mathf.Sign(collision.transform.position.x - transform.position.x) * knockback * -2, knockback / 2);
        }

    }
    private void wander()   //just threw the previous code in here for now
    {
        transform.position = Vector2.MoveTowards(transform.position, positions[index], Time.deltaTime * speed);

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

}
