using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjController : MonoBehaviour
{
    private Rigidbody2D rb;

    public int rotSpeed;
    public float projSpeed, lifespan;

    [System.NonSerialized]
    public float damage;

    private AttackController aController;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        aController = PlayerController.player.GetComponent<AttackController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb != null && rb.rotation != 270)
        {
            if (rb.rotation != 90)
                rb.rotation += rotSpeed * Time.deltaTime * Mathf.Sign(rb.velocity.x) * -1;
            else if (rb.velocity.y < 1 && !(transform.rotation.eulerAngles.z == 270 && transform.rotation.eulerAngles.x < 180)) //if(Mathf.Abs(rb.velocity.y) < 0.01) 
                transform.Rotate(new Vector3(0, Time.deltaTime * 180, 0), Space.Self);
        }
        //Debug.Log(transform.rotation.eulerAngles);
        //rb.rotation = Vector2.Angle(Vector2.zero, rb.velocity) - 90;
        lifespan -= Time.deltaTime;
        if (lifespan <= 0 || (rb != null && rb.position.y < -15))
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && rb != null)
        {
            EnemyController script = collision.GetComponent<EnemyController>();
            if (script == null)
            {
                collision.GetComponent<Lucifer>().TakeDamage(damage);
            }
            else
            {
                //script.Knockback(6, gameObject.transform);
                script.TakeDamage(damage);
                collision.attachedRigidbody.velocity += new Vector2(Mathf.Sign(collision.transform.position.x - PlayerController.player.transform.position.x) * aController.bowKnockback, aController.bowKnockback / 2);
            }
            
            Destroy(gameObject);
        }
        else if(collision.CompareTag("EnemyProjectile"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
        else if(collision.CompareTag("Ground") || collision.CompareTag("Walls"))
        {
            //Play ArrowHitGround sound
            FindObjectOfType<AudioManager>().Play("ArrowHitGround");
            Destroy(rb);
            lifespan = 5f;
        }
    }
}
