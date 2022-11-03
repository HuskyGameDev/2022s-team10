using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackController : MonoBehaviour
{

    //public float bowStrength;
    public float knockback, bowKnockback;

    public GameObject weaponAttack;

    private GameObject equippedWeapon = null;
    private GameObject equippedBow = null;
    private GameObject equippedArmor = null;

    private Rigidbody2D rb;
    private PlayerController pcontroller;
    private Collider2D thisCollider;

    private PlayerInput playerInput;
    private InputAction swapAction, attackAction, pickupAction;
    private bool usingRanged = false; // 1 or 0, checks if bow is "equipped", currently only toggles between using ranged or melee

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pcontroller = GetComponent<PlayerController>();
        thisCollider = GetComponent<Collider2D>();

        playerInput = GetComponent<PlayerInput>();
        pickupAction = playerInput.actions["Pickup"];
        swapAction = playerInput.actions["Swap"];
        attackAction = playerInput.actions["Attack"];
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerController.isPaused){
            if(attackAction.triggered && equippedWeapon != null && GameObject.Find("SwordSwipe(Clone)") == null && !usingRanged)
            {
                GameObject swipe = Instantiate(weaponAttack, transform.position, Quaternion.identity);
                SpriteRenderer[] children = equippedWeapon.GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer child in children){
                    if (child.name.Equals("ItemTexture")){
                        swipe.GetComponent<SpriteRenderer>().sprite = child.sprite;
                    }
                }
                swipe.GetComponent<WeaponController>().rotSpeed = equippedWeapon.GetComponent<ItemController>().meleeSpeed;
            }
            if(attackAction.triggered && equippedBow != null && usingRanged)
            {
                Vector2 dir = attackAction.ReadValue<Vector2>();
                GameObject shoot = Instantiate(equippedBow.GetComponent<ItemController>().arrow, transform.position, Quaternion.identity);
                int shootAngle = 90;
                int diff = 80;
                if (dir.y == 1)
                    diff = 45;
                else if (dir.y == -1)
                {
                    shootAngle = 270;
                    diff = -45;
                }

                if (dir.x == 1)
                    shootAngle -= diff;
                if (dir.x == -1)
                    shootAngle += diff;

                if (shootAngle == 90 && diff == 80)
                    shootAngle = pcontroller.facingRight ? 10 : 170;
                
                float projSpeed = shoot.GetComponent<ProjController>().projSpeed;

                shoot.GetComponent<Rigidbody2D>().rotation = shootAngle;
                shoot.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(shootAngle * Mathf.Deg2Rad) * projSpeed, Mathf.Sin(shootAngle * Mathf.Deg2Rad) * projSpeed);
            }
            if (pickupAction.triggered)
            {
                PickupItem();
            }
            if (swapAction.triggered){
                usingRanged = !usingRanged;
            }
        }
    }

    void PickupItem()
    {
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
                        SpriteRenderer[] children = equippedWeapon.GetComponentsInChildren<SpriteRenderer>();
                        foreach (SpriteRenderer child in children){
                            if (child.name.Equals("ItemTexture")){
                                GameObject.Find("HUD Weapon").GetComponent<SpriteRenderer>().sprite = child.sprite;
                            }
                        }
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
                        SpriteRenderer[] children2 = equippedBow.GetComponentsInChildren<SpriteRenderer>();
                        foreach (SpriteRenderer child in children2){
                            if (child.name.Equals("ItemTexture")){
                                GameObject.Find("HUD Bow").GetComponent<SpriteRenderer>().sprite = child.sprite;        
                            }
                        }
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
                        SpriteRenderer[] children3 = equippedArmor.GetComponentsInChildren<SpriteRenderer>();
                        foreach (SpriteRenderer child in children3){
                            if (child.name.Equals("ItemTexture")){
                                GameObject.Find("HUD Armor").GetComponent<SpriteRenderer>().sprite = child.sprite;        
                            }
                        }
                        break;
                }
            }
        }
    }
}
