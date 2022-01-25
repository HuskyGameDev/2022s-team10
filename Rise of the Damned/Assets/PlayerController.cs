using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int speed;
    private Rigidbody2D rb;
    private Collider2D thisCollider;

    // Start is called before the first frame update
    void Start()
    {
       rb = GetComponent<Rigidbody2D>();
       thisCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        int horiz = 0;
        int vert = 0;
        if (Input.GetKey(KeyCode.D))
            horiz += 1;
        if (Input.GetKey(KeyCode.A))
            horiz -= 1;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
            vert += 1;
        Vector3 input = new Vector3(horiz, vert * 30, 0f);
        transform.Translate(input * speed * Time.deltaTime);
    }
}
