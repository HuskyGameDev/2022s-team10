using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    private Rigidbody2D rb;

    public int rotSpeed;
    public float lifespan;

    private float life = 0;

    private GameObject player;
    private AttackController aController;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        aController = player.GetComponent<AttackController>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.rotation += rotSpeed * Time.deltaTime * Mathf.Sign(rb.velocity.x) * -1;
        //rb.rotation = Vector2.Angle(Vector2.zero, rb.velocity) - 90;
        life += Time.deltaTime;
        if (life > lifespan || rb.position.y < -15)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            collision.GetComponent<EnemyController>().health -= player.GetComponent<AttackController>().damage;
            collision.attachedRigidbody.velocity += new Vector2(Mathf.Sign(collision.transform.position.x - player.transform.position.x) * aController.knockback, aController.knockback / 2);
            Destroy(gameObject);
        }

    }
}
