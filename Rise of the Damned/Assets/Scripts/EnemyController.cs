using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameObject FireBall;
    public int damage;
    public float health;

    public float knockback = 6;
    
    [SerializeField]
    private float speed;

    [SerializeField]
    private Vector2[] positions;

    private int index = 0;

    // Update is called once per frame

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, positions[index], Time.deltaTime * speed);

        if (Mathf.Abs(transform.position.x - positions[index].x) < .1)
        {
            if(index == positions.Length-1)
            {
    
                index = 0;
            }
            else
            {
                index++;
            }
        }

        if (health < 0) 
            Destroy(gameObject);

        if (gameObject.name == "demon skull")    
        {
            GameObject shoot = Instantiate(FireBall, transform.position, Quaternion.identity);
            float shootAngle = Vector2.Angle(transform.position, PlayerController.player.transform.position);

            shoot.GetComponent<Rigidbody2D>().rotation = shootAngle;
            shoot.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(shootAngle * Mathf.Deg2Rad) * 10, Mathf.Sin(shootAngle * Mathf.Deg2Rad) * 10);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerController.health -= damage - PlayerController.armor;
            collision.rigidbody.velocity += new Vector2(Mathf.Sign(collision.transform.position.x - transform.position.x) * knockback * -2, knockback / 2);
        }

    }
}



