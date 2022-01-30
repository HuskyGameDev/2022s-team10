using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int speed;   //speed of player horizontally
    private Rigidbody2D rb;
    private Collider2D thisCollider;
    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        thisCollider = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        int horiz = 0;
        //int vert = 0;
        if (Input.GetKey(KeyCode.D))
            horiz += 1;
        if (Input.GetKey(KeyCode.A))
            horiz -= 1;
        
        /*Vector3 input = new Vector3(horiz, vert * 30, 0f);
        transform.Translate(input * speed * Time.deltaTime);*/
        rb.AddForce(Time.deltaTime * speed * new Vector2(horiz, 0).normalized);

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space))
            rb.velocity = rb.velocity - new Vector2(0, rb.velocity.y * Time.deltaTime * .1f);

        List<Collider2D> results = new List<Collider2D>();
        thisCollider.OverlapCollider(new ContactFilter2D(), results);

        foreach(Collider2D collision in results)
            if (collision.tag == "Ground")
            {
                rb.velocity = rb.velocity - new Vector2(0, rb.velocity.y * Time.deltaTime);
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space) && rb.velocity.y < 400)
                    rb.AddForce(new Vector2(rb.velocity.y < 0 ? 1000 * Mathf.Sign(transform.position.x - collision.gameObject.transform.position.x) : 0, 1000));
            }

        // Change sprite X flip status based on whether the player is moving left or right
        if (Input.GetKey(KeyCode.A))
            sr.flipX = true;
        if (Input.GetKey(KeyCode.D))
            sr.flipX = false;
    }
}
