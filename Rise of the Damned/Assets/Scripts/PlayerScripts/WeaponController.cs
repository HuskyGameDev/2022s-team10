using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField]
    public int rotSpeed;
    [SerializeField]
    private int rotOffset;
    [SerializeField]
    private float displacement;

    private Rigidbody2D rb;
    private GameObject player;
    private PlayerController pcontroller;
    private AttackController acontroller;
    private List<Collider2D> hit = new List<Collider2D>();

    private float rot = -45;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = PlayerController.player;
        pcontroller = player.GetComponent<PlayerController>();
        acontroller = player.GetComponent<AttackController>();
    }

    // Update is called once per frame
    void Update()
    {
        int facingRight = pcontroller.facingRight ? 1 : -1;
        rot += rotSpeed * Time.deltaTime;
        rb.rotation =  Mathf.RoundToInt(rot) * facingRight * -1 + rotOffset;

        rb.position = new Vector2(player.transform.position.x + Mathf.Cos(Mathf.Deg2Rad * (rot - 90)) * displacement * facingRight, 
                                  player.transform.position.y + Mathf.Sin(Mathf.Deg2Rad * (rot - 90)) * displacement * -1) ;
        if (rot > 180)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy") && !hit.Contains(collision))
        {
            collision.GetComponent<EnemyController>().TakeDamage(PlayerController.meleeDamage);
            collision.GetComponent<EnemyController>().Knockback(6, gameObject.transform);
            // ^ currently cant figure out how to access the itemcontroller script so can't set a weapon specific knockback
            hit.Add(collision);
        }
        
    }
}
