using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [System.NonSerialized]
    public float rotSpeed;
    [SerializeField]
    private int rotOffset;
    [SerializeField]
    private float displacement;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private GameObject player;
    private PlayerController pcontroller;
    private AttackController acontroller;
    private List<Collider2D> hit = new List<Collider2D>();

    public static int attackDir;

    private float rot;
    private float initialRot;
    private int counterClock;

    // Start is called before the first frame update
    void Start()
    {
        /*
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        player = PlayerController.player;
        pcontroller = player.GetComponent<PlayerController>();
        acontroller = player.GetComponent<AttackController>();
        */
        /*
        int facingRight = pcontroller.facingRight ? 1 : -1;
        switch (attackDir){
            case 0:
                if (facingRight == 1){
                    rot = 405;
                    initialRot = 405;
                    counterClock = -1;
                } else {
                    rot = 315;
                    initialRot = 315;
                    counterClock = 1;
                }
            break;
            case 1:
                if (facingRight == 1){
                    rot = 225;
                    initialRot = 225;
                    counterClock = -1;
                } else {
                    rot = 135;
                    initialRot = 135;
                    counterClock = 1;
                }
            break;
            case 2:
                rot = 315;
                initialRot = 315;
                counterClock = -1;
            break;
            case 3:
                rot = 45;
                initialRot = 45;
                counterClock = 1;
            break;
        } */
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!sr.enabled)
        {
            sr.enabled = true;
        }
        else
        {
            rot += rotSpeed * Time.deltaTime * counterClock;
            rb.rotation = Mathf.RoundToInt(rot) + rotOffset;
            rb.position = new Vector2(player.transform.position.x + Mathf.Cos(Mathf.Deg2Rad * (rot - 90)) * -1 * displacement,
                                      player.transform.position.y + Mathf.Sin(Mathf.Deg2Rad * (rot - 90)) * -1 * displacement);
            Destroy(gameObject, 2f);
            if (Mathf.Abs(rot) > initialRot + 60 || Mathf.Abs(rot) < initialRot - 60)
                Destroy(gameObject);
                //gameObject.SetActive(false);


        }
    }

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        player = PlayerController.player;
        pcontroller = player.GetComponent<PlayerController>();
        acontroller = player.GetComponent<AttackController>();
        //gameObject.SetActive(true);
        if (rb == null)
            Start();

        int facingRight = pcontroller.facingRight ? 1 : -1;
        switch (attackDir)
        {
            case 0:
                if (facingRight == 1)
                {
                    initialRot = 405;
                    counterClock = -1;
                }
                else
                {
                    initialRot = 315;
                    counterClock = 1;
                }
                break;
            case 1:
                if (facingRight == 1)
                {
                    initialRot = 225;
                    counterClock = -1;
                }
                else
                {
                    initialRot = 135;
                    counterClock = 1;
                }
                break;
            case 2:
                initialRot = 315;
                counterClock = -1;
                break;
            case 3:
                initialRot = 45;
                counterClock = 1;
                break;
        }
        rot = initialRot;
        rotOffset *= (counterClock * -1);
        rb.rotation = initialRot + rotOffset;
        rb.position = new Vector2(player.transform.position.x + Mathf.Cos(Mathf.Deg2Rad * (initialRot - 90)) * -1 * displacement,
                                  player.transform.position.y + Mathf.Sin(Mathf.Deg2Rad * (initialRot - 90)) * -1 * displacement);

        hit = new List<Collider2D>();
        sr.flipX = (counterClock == 1);
    }

    private void OnDisable()
    {
        //sr.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy") && !hit.Contains(collision))
        {
            EnemyController script = collision.GetComponent<EnemyController>();
            if (script == null)
            {
                collision.GetComponent<Lucifer>().TakeDamage(PlayerController.meleeDamage);
            }
            else
            {
                script.Knockback(acontroller.meleeKnockback, gameObject.transform);
                script.TakeDamage(PlayerController.meleeDamage);
            }
            
            // ^ currently cant figure out how to access the itemcontroller script so can't set a weapon specific knockback
            // ^ here u go: PlayerController.attackController.equippedWeapon.GetComponent<ItemController>();
            hit.Add(collision);
        }
        
    }
}
