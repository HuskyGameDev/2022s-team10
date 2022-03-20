using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    private Rigidbody2D rb;

    public int rotSpeed;
    public float lifespan;

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
        if (rb != null)
            if (rb.rotation != 90 && rb.rotation != 270)
                rb.rotation += rotSpeed * Time.deltaTime * Mathf.Sign(rb.velocity.x) * -1;
            else if (rb.velocity.y < 1 && !(transform.rotation.eulerAngles.z == 270 && transform.rotation.eulerAngles.x < 180)) //if(Mathf.Abs(rb.velocity.y) < 0.01) 
                transform.Rotate(new Vector3(0, Time.deltaTime * 180, 0), Space.Self);
        //Debug.Log(transform.rotation.eulerAngles);
        //rb.rotation = Vector2.Angle(Vector2.zero, rb.velocity) - 90;
        lifespan -= Time.deltaTime;
        if (lifespan <= 0 || (rb != null && rb.position.y < -15))
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" && rb != null)
        {
            collision.GetComponent<EnemyController>().health -= PlayerController.bowDamage;
            collision.attachedRigidbody.velocity += new Vector2(Mathf.Sign(collision.transform.position.x - PlayerController.player.transform.position.x) * aController.knockback, aController.knockback / 2);
            Destroy(gameObject);
        }
        else if(collision.tag == "Ground" || collision.tag == "Walls")
        {
            Destroy(rb);
            lifespan = 5f;
        }
    }
}
