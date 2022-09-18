using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{

    public float bowStrength;
    public float knockback, bowKnockback;

    public GameObject weaponAttack;

    private GameObject equippedWeapon = null;
    private GameObject equippedBow = null;
    private GameObject equippedArmor = null;

    private Rigidbody2D rb;
    private PlayerController pcontroller;
    private Collider2D thisCollider;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pcontroller = GetComponent<PlayerController>();
        thisCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerController.isPaused){
            if(Input.GetKeyDown(KeyCode.F) && equippedWeapon != null && GameObject.Find("SwordSwipe(Clone)") == null)
            {
                GameObject swipe = Instantiate(weaponAttack, transform.position, Quaternion.identity);
                swipe.GetComponent<SpriteRenderer>().sprite = equippedWeapon.GetComponent<SpriteRenderer>().sprite;
                swipe.GetComponent<WeaponController>().rotSpeed = equippedWeapon.GetComponent<ItemController>().meleeSpeed;
            }
            if(Input.GetKeyDown(KeyCode.E) && equippedBow != null)
            {
                GameObject shoot = Instantiate(equippedBow.GetComponent<ItemController>().arrow, transform.position, Quaternion.identity);
                int shootAngle = 90;
                int diff = 80;
                if (Input.GetKey(KeyCode.UpArrow))
                    diff = 45;
                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    shootAngle = 270;
                    diff = -45;
                }

                if (Input.GetKey(KeyCode.RightArrow))
                    shootAngle -= diff;
                if (Input.GetKey(KeyCode.LeftArrow))
                    shootAngle += diff;

                if (shootAngle == 90 && diff == 80)
                    shootAngle = pcontroller.facingRight ? 10 : 170;
                

                shoot.GetComponent<Rigidbody2D>().rotation = shootAngle;
                shoot.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(shootAngle * Mathf.Deg2Rad) * bowStrength, Mathf.Sin(shootAngle * Mathf.Deg2Rad) * bowStrength);
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                PickupItem();
            }
        }
    }

    void PickupItem()
    { //tylers shit
        List<Collider2D> results = new List<Collider2D>();
        thisCollider.OverlapCollider(new ContactFilter2D(), results);
        foreach (Collider2D collision in results)
        {
            if (collision.gameObject.tag == "ItemDrop")
            {
                ItemController item = collision.GetComponent<ItemController>();
                switch (item.type)
                {
                    case ItemController.ItemType.Sword:
                        if (equippedWeapon != null) //reactivate old weapon
                        {
                            equippedWeapon.SetActive(true);
                            equippedWeapon.GetComponent<Rigidbody2D>().position = collision.attachedRigidbody.position;
                        }

                        equippedWeapon = collision.gameObject;  //equip new weapon
                        PlayerController.meleeDamage = item.meleeDamage;
                        equippedWeapon.SetActive(false);
                        GameObject.Find("HUD Weapon").GetComponent<SpriteRenderer>().sprite = equippedWeapon.GetComponent<SpriteRenderer>().sprite;
                        break;
                    case ItemController.ItemType.Bow:
                        if (equippedBow != null) //reactivate old weapon
                        {
                            equippedBow.SetActive(true);
                            equippedBow.GetComponent<Rigidbody2D>().position = collision.attachedRigidbody.position;
                        }

                        equippedBow = collision.gameObject;  //equip new weapon
                        PlayerController.rangedDamage = item.rangedDamage;
                        equippedBow.SetActive(false);
                        GameObject.Find("HUD Bow").GetComponent<SpriteRenderer>().sprite = equippedBow.GetComponent<SpriteRenderer>().sprite;
                        break;
                    case ItemController.ItemType.Armor:
                        if (equippedArmor != null) //reactivate old weapon
                        {
                            equippedArmor.SetActive(true);
                            equippedArmor.GetComponent<Rigidbody2D>().position = collision.attachedRigidbody.position;
                        }

                        equippedArmor = collision.gameObject;  //equip new weapon
                        PlayerController.armor = item.armor;
                        equippedArmor.SetActive(false);
                        GameObject.Find("HUD Armor").GetComponent<SpriteRenderer>().sprite = equippedArmor.GetComponent<SpriteRenderer>().sprite;
                        break;
                }
            }
        }
    }
}
