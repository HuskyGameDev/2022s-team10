using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public int rotSpeed;
    public float displacement;

    private Rigidbody2D rb;
    private GameObject player;
    private PlayerController pcontroller;

    private float rot = -45;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        pcontroller = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        int facingRight = pcontroller.facingRight ? 1 : -1;
        rot += rotSpeed * Time.deltaTime;
        rb.rotation =  Mathf.RoundToInt(rot) * facingRight * -1;

        rb.position = new Vector2(player.transform.position.x + Mathf.Cos(Mathf.Deg2Rad * (rot - 90)) * displacement * facingRight, 
                                  player.transform.position.y + Mathf.Sin(Mathf.Deg2Rad * (rot - 90)) * displacement * -1) ;
        if (rot > 180)
            Destroy(gameObject);
    }
}
