using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    private Rigidbody2D rb;

    public int rotSpeed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.rotation += rotSpeed * Time.deltaTime * Mathf.Sign(rb.velocity.x) * -1;
        //rb.rotation = Vector2.Angle(Vector2.zero, rb.velocity) - 90;

    }
}
