using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjController : MonoBehaviour
{
    private Vector3 movePos;
    public float speed;
    [SerializeField]
    protected float lifespan, knockback;
    [System.NonSerialized]
    public float damage;
    private bool moveTowardsPoint = true;

    protected Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movePos = PlayerController.player.transform.position;
        rb.angularVelocity = 360 * 2;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (moveTowardsPoint)
            transform.position = Vector2.MoveTowards(transform.position, movePos, speed * Time.deltaTime);
        else
        {
            if(Vector2.Distance(transform.position, movePos) <= .3)
                rb.velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * (speed / 2);
            else
                transform.position = Vector2.MoveTowards(transform.position, movePos, speed * Time.deltaTime);
            //Debug.Log(":zany_face:");
        }

        if (transform.position.Equals(movePos))
            moveTowardsPoint = false;

        lifespan -= Time.deltaTime;
        if (lifespan <= 0)
            Destroy(gameObject);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController.TakeDamage(damage);
            PlayerController.controller.Knockback(knockback, gameObject.transform);
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Ground") || collision.CompareTag("Walls") || collision.name.Contains("SwordSwipe"))
        {
            Destroy(gameObject);
        }
    }
}
