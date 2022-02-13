using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    public int health = 50;
    public int maxHealth = 100;

    public float bowStrength;

    public GameObject weaponAttack, arrow;

    private Rigidbody2D rb;
    private PlayerController pcontroller;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pcontroller = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F) && GameObject.Find("SwordSwipe(Clone)") == null)
        {
            Instantiate(weaponAttack, transform.position, Quaternion.identity);
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            GameObject shoot = Instantiate(arrow, transform.position, Quaternion.identity);
            int shootAngle = pcontroller.facingRight? 10 : 170;

            if (Input.GetKey(KeyCode.RightArrow))
                shootAngle = 10;
            else if (Input.GetKey(KeyCode.LeftArrow))
                shootAngle = 170;

            if (Input.GetKey(KeyCode.UpArrow))
            {
                if (Input.GetKey(KeyCode.RightArrow))
                    shootAngle = 45;
                else if (Input.GetKey(KeyCode.LeftArrow))
                    shootAngle = 90 + 45;
                else
                    shootAngle += 80 * (pcontroller.facingRight ? 1 : -1);
            }

            shoot.GetComponent<Rigidbody2D>().rotation = shootAngle;
            shoot.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(shootAngle * Mathf.Deg2Rad) * bowStrength, Mathf.Sin(shootAngle * Mathf.Deg2Rad) * bowStrength);
        }
    }
}
