using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public int rotSpeed;

    private Rigidbody2D rb;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //if (rb.rotation < 180)
        rb.rotation += rotSpeed * Time.deltaTime;
        rb.position = new Vector2(player.transform.position.x, player.transform.position.y);
    }
}
