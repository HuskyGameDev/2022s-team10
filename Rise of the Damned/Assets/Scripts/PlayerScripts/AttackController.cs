using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackController : MonoBehaviour
{

    //public float bowStrength;
    public float knockback, bowKnockback;

    public GameObject weaponAttack;
     
    public GameObject equippedWeapon = null;
    public GameObject equippedBow = null;
    private GameObject equippedArmor = null;

    private Transform EW_Parent, EB_Parent, EA_Parent;

    private Rigidbody2D rb;
    private PlayerController pcontroller;
    private Collider2D thisCollider;

    private PlayerInput playerInput;
    private InputAction swapAction, attackAction, pickupAction;
    private bool usingRanged = false; // 1 or 0, checks if bow is "equipped", currently only toggles between using ranged or melee

    private bool wasHoldingAttack = false;
    private int shootAngle;
    private int diff;

    public float bowHoldTime;
    public float bowChargeTime;
    private float currBowHoldTime = 0;

    public float rememberDiagFor;
    private Vector2 rememberDiagTime = Vector2.zero;
    private Vector2 diagMemory = Vector2.zero;

    public GameObject SelectedMelee;
    public GameObject SelectedRanged;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pcontroller = GetComponent<PlayerController>();
        thisCollider = GetComponent<Collider2D>();

        PlayerController.attackController = this;
        playerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        pickupAction = playerInput.actions["Pickup"];
        swapAction = playerInput.actions["Swap"];
        attackAction = playerInput.actions["Attack"];

        //Debug.Log("Weapon: " + equippedWeapon);
        if (!PlayerController.isPaused && PlayerController.isActive){
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

            if(IsHoldingAttack())
            {
                rememberDiagTime += Vector2.one * Time.deltaTime;
                if (rememberDiagTime.x >= rememberDiagFor)
                    diagMemory.x = 0;
                if (rememberDiagTime.y >= rememberDiagFor)
                    diagMemory.y = 0;

                Vector2 dir = attackAction.ReadValue<Vector2>();
                if (dir.x != 0)
                {
                    diagMemory.x = dir.x;
                    rememberDiagTime.x = 0;
                }
                if (dir.y != 0)
                {
                    diagMemory.y = dir.y;
                    rememberDiagTime.y = 0;
                }


                shootAngle = 90;
                diff = 80;
                if (diagMemory.y > 0 )
                    diff = 45;
                else if (diagMemory.y < 0)
                {
                    shootAngle = 270;
                    diff = -45;
                }

                if (diagMemory.x > 0)
                    shootAngle -= diff;
                if (diagMemory.x < 0)
                    shootAngle += diff;

                if (shootAngle == 90 && diff == 80)
                    shootAngle = pcontroller.facingRight ? 10 : 170;

                currBowHoldTime += Time.deltaTime;
            }
            else if(wasHoldingAttack)
            {
                if (currBowHoldTime >= bowHoldTime && equippedBow != null && usingRanged)
                {
                    GameObject shoot = Instantiate(equippedBow.GetComponent<ItemController>().arrow, transform.position, Quaternion.identity);

                    float projSpeed = shoot.GetComponent<ProjController>().projSpeed * Mathf.Min(currBowHoldTime / bowChargeTime, 1);

                    shoot.GetComponent<Rigidbody2D>().rotation = shootAngle;
                    shoot.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Cos(shootAngle * Mathf.Deg2Rad) * projSpeed, Mathf.Sin(shootAngle * Mathf.Deg2Rad) * projSpeed);

                    Debug.Log("Memory: " + diagMemory + "\t Time: " + rememberDiagTime);
                }
                currBowHoldTime = 0;
            }
            if (pickupAction.triggered)
            {
                PickupItem();
            }
            if (swapAction.triggered){
                SelectedMelee.SetActive(!SelectedMelee.activeSelf);
                SelectedRanged.SetActive(!SelectedRanged.activeSelf);
                usingRanged = !usingRanged;
            }

            wasHoldingAttack = IsHoldingAttack();
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
                            equippedWeapon.transform.SetParent(EW_Parent);
                        }

                        equippedWeapon = collision.gameObject;  //equip new weapon

                        EW_Parent = equippedWeapon.transform.parent;
                        equippedWeapon.transform.SetParent(null);

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
                        if (equippedBow != null) //reactivate old bow
                        {
                            equippedBow.SetActive(true);
                            equippedBow.GetComponent<Rigidbody2D>().position = collision.attachedRigidbody.position;
                            equippedBow.transform.SetParent(EB_Parent);
                        }

                        equippedBow = collision.gameObject;  //equip new bow

                        EB_Parent = equippedBow.transform.parent;
                        equippedBow.transform.SetParent(null);

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
                        if (equippedArmor != null) //reactivate old armor
                        {
                            equippedArmor.SetActive(true);
                            equippedArmor.GetComponent<Rigidbody2D>().position = collision.attachedRigidbody.position;
                            equippedArmor.transform.SetParent(EA_Parent);
                        }

                        equippedArmor = collision.gameObject;  //equip new armor

                        EA_Parent = equippedArmor.transform.parent;
                        equippedArmor.transform.SetParent(null);

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

    public bool IsHoldingAttack()
    {
        if (attackAction.ReadValue<Vector2>().x != 0)
            return true;
        return attackAction.ReadValue<Vector2>().y != 0;
    }
}
