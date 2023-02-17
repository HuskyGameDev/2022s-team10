using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackController : MonoBehaviour
{

    //public float bowStrength;
    [System.NonSerialized]
    public float meleeKnockback, rangedKnockback;

    public GameObject weaponAttack;
     
    public GameObject equippedWeapon;
    public GameObject equippedBow;
    private GameObject equippedArmor;

    private Rigidbody2D rb;
    private PlayerController pcontroller;
    private WeaponController wcontroller;
    private Collider2D thisCollider;

    private PlayerInput playerInput;
    private InputAction swapAction, attackAction, pickupAction;
    private Vector2 attackDir;

    private bool usingRanged = false; // 1 or 0, checks if bow is "equipped", currently only toggles between using ranged or melee

    public GameObject swipe;

    private bool wasHoldingAttack = false;
    private int shootAngle;
    private int diff;

    private float timeSinceLastSwing = 0;
    private float meleeCooldown;

    [Tooltip("Minimum time to be able to shoot")]
    public float bowHoldTime;       //minimum time to hold the bow and still be able to shoot in seconds
    private float bowChargeTime;    //minimum time to fully charge the bow in seconds
    private float currBowHoldTime = 0;  //time the bow has currently been held in seconds

    private bool readyToFire = false;

    [Tooltip("Time to remember that a diagonal direction was held")]
    public float rememberDiagFor;   //time to remember that a diagonal direction was held after one of the keys was released
    private Vector2 rememberDiagTime = Vector2.zero;
    private Vector2 diagMemory = Vector2.zero;

    public GameObject SelectedMelee;
    public GameObject SelectedRanged;

    [SerializeField]
    private PlayerSO PlayerData; //stored player data to persist between scenes

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pcontroller = GetComponent<PlayerController>();
        thisCollider = GetComponent<Collider2D>();

        PlayerController.attackController = this;
        playerInput = GetComponent<PlayerInput>();

        if(PlayerData.EquippedWeapon != null)
        {
            //Debug.Log("Equipping: " + PlayerData.EquippedWeapon.name);
            EquipItem(PlayerData.EquippedWeapon.GetComponent<ItemController>());
        } else
        {
            //Debug.Log("Weapon not found");
        }

        if (PlayerData.EquippedBow != null)
            EquipItem(PlayerData.EquippedBow.GetComponent<ItemController>());
        if (PlayerData.EquippedArmor != null)
            EquipItem(PlayerData.EquippedArmor.GetComponent<ItemController>());

        swipe = Instantiate(weaponAttack, transform.position, Quaternion.identity);
        swipe.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        pickupAction = playerInput.actions["Pickup"];
        swapAction = playerInput.actions["Swap"];
        attackAction = playerInput.actions["Attack"];

        //Debug.Log("Weapon: " + equippedWeapon);
        if (!PlayerController.isPaused && PlayerController.isActive)
        {
            if (GameObject.Find("SwordSwipe(Clone)") == null)
            {
                timeSinceLastSwing += Time.deltaTime;
                //swing sword
                if (attackAction.triggered && equippedWeapon != null && !usingRanged && timeSinceLastSwing >= meleeCooldown)
                {
                    timeSinceLastSwing = 0;
                    attackDir = attackAction.ReadValue<Vector2>();
                    if (attackDir.y == 1)
                    {
                        //Debug.Log("up");
                        WeaponController.attackDir = 0; // Up
                    }
                    else if (attackDir.y == -1)
                    {
                        WeaponController.attackDir = 1; // Down
                    }
                    else if (attackDir.x == 1)
                    {
                        WeaponController.attackDir = 2; // Right
                    }
                    else if (attackDir.x == -1)
                    {
                        WeaponController.attackDir = 3; // Left
                    }
                    //Debug.Log("attack dir: " + wcontroller.attackDir);

                    //GameObject swipe = Instantiate(weaponAttack, transform.position, Quaternion.identity);
                    swipe.SetActive(true);
                    SpriteRenderer[] children = equippedWeapon.GetComponentsInChildren<SpriteRenderer>();
                    foreach (SpriteRenderer child in children)
                    {
                        if (child.name.Equals("ItemTexture"))
                        {
                            swipe.GetComponent<SpriteRenderer>().sprite = child.sprite;
                        }
                    }
                    swipe.GetComponent<WeaponController>().rotSpeed = equippedWeapon.GetComponent<ItemController>().meleeSpeed;

                    //Play sword swing
                    int n = (int)Random.Range(1f, 3f);
                    FindObjectOfType<AudioManager>().Play("SwordSwing" + n);
                }
            }
            if(IsHoldingAttack() && usingRanged && equippedBow != null)
            {
                //Play BowDrawback sound
                if(!wasHoldingAttack)
                {
                    FindObjectOfType<AudioManager>().Play("BowDrawback");
                }
                //Play BowReadyToFire sound
                if (!readyToFire && currBowHoldTime - bowChargeTime > 0)
                {
                    //stop BowDrawback sound
                    FindObjectOfType<AudioManager>().StopPlaying("BowDrawback");
                    //Play BowReadyToFire sound
                    FindObjectOfType<AudioManager>().Play("BowReadyToFire");
                    readyToFire = true;
                }

                //reset diagonal memory if necessary
                rememberDiagTime += Vector2.one * Time.deltaTime;
                if (rememberDiagTime.x >= rememberDiagFor)
                    diagMemory.x = 0;
                if (rememberDiagTime.y >= rememberDiagFor)
                    diagMemory.y = 0;

                //calculate the direction to fire the bow based on which key(s) were held
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
                //fire the bow
                if (currBowHoldTime >= bowHoldTime && equippedBow != null && usingRanged)
                {
                    ItemController bow = equippedBow.GetComponent<ItemController>();
                    GameObject shoot = Instantiate(bow.arrow, transform.position, Quaternion.identity);
                    ProjController scr = shoot.GetComponent<ProjController>();
                    Rigidbody2D srb = shoot.GetComponent<Rigidbody2D>();

                    float chargeRate = Mathf.Min(currBowHoldTime / bowChargeTime, 1);
                    float projSpeed = bow.projVelocity * chargeRate;

                    scr.damage = (Mathf.Round(1000 / (.9972472f + Mathf.Pow(4, (chargeRate-.5f) * -8.5f))) / 1000f) * PlayerController.rangedDamage;
                    scr.projSpeed = bow.projVelocity;
                    srb.rotation = shootAngle;
                    srb.velocity = new Vector2(Mathf.Cos(shootAngle * Mathf.Deg2Rad) * projSpeed, Mathf.Sin(shootAngle * Mathf.Deg2Rad) * projSpeed);

                    //Debug.Log("Memory: " + diagMemory + "\t Time: " + rememberDiagTime);
                    //Debug.Log("Damage %: " + scr.damage / PlayerController.rangedDamage);
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

            if(equippedBow != null && usingRanged && wasHoldingAttack && !IsHoldingAttack())
            {
                //stop BowDrawback sound
                FindObjectOfType<AudioManager>().StopPlaying("BowDrawback");
                if(currBowHoldTime > bowHoldTime)
                {
                    //play BowRelease sound
                    FindObjectOfType<AudioManager>().Play("BowRelease");
                }

                readyToFire = false;
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
                EquipItem(item);

                //Play pickup sound
                FindObjectOfType<AudioManager>().Play("Pickup");
            }
        }
    }


    void EquipItem(ItemController item)
    {
        switch (item.type)
        {
            case ItemController.ItemType.Melee:
                if (equippedWeapon != null) //reactivate old weapon
                {
                    equippedWeapon.SetActive(true);
                    //equippedBow.GetComponent<Rigidbody2D>().position = collision.attachedRigidbody.position;
                    equippedWeapon.transform.position = transform.position;
                    equippedWeapon.GetComponentInChildren<Transform>().rotation = Quaternion.identity;
                    equippedWeapon.transform.SetParent(item.transform.parent);
                }

                equippedWeapon = item.gameObject;  //equip new weapon
                PlayerData.EquippedWeapon = equippedWeapon; //store in playerdata
                equippedWeapon.transform.SetParent(null);

                PlayerController.meleeDamage = item.meleeDamage;
                meleeKnockback = item.meleeKnockback;
                meleeCooldown = item.meleeCooldown;
                equippedWeapon.SetActive(false);
                SpriteRenderer[] children = equippedWeapon.GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer child in children)
                {
                    if (child.name.Equals("ItemTexture"))
                    {
                        GameObject.Find("HUD Weapon").GetComponent<SpriteRenderer>().sprite = child.sprite;
                    }
                }
                break;
            case ItemController.ItemType.Ranged:
                if (equippedBow != null) //reactivate old bow
                {
                    equippedBow.SetActive(true);
                    //equippedBow.GetComponent<Rigidbody2D>().position = collision.attachedRigidbody.position;
                    equippedBow.transform.position = transform.position;
                    equippedBow.GetComponentInChildren<Transform>().rotation = Quaternion.identity;
                    equippedBow.transform.SetParent(item.transform.parent);
                }

                equippedBow = item.gameObject;  //equip new bow
                PlayerData.EquippedBow = equippedBow; //store in playerdata
                equippedBow.transform.SetParent(null);

                PlayerController.rangedDamage = item.rangedDamage;
                bowChargeTime = item.drawbackTime;
                rangedKnockback = item.rangedKnockback;
                equippedBow.SetActive(false);
                SpriteRenderer[] children2 = equippedBow.GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer child in children2)
                {
                    if (child.name.Equals("ItemTexture"))
                    {
                        GameObject.Find("HUD Bow").GetComponent<SpriteRenderer>().sprite = child.sprite;
                    }
                }
                break;
            case ItemController.ItemType.Armor:
                if (equippedArmor != null) //reactivate old armor
                {
                    equippedArmor.SetActive(true);
                    //equippedArmor.GetComponent<Rigidbody2D>().position = collision.attachedRigidbody.position;
                    equippedArmor.transform.position = transform.position;
                    equippedArmor.GetComponentInChildren<Transform>().rotation = Quaternion.identity;
                    equippedArmor.transform.SetParent(item.transform.parent);
                }

                equippedArmor = item.gameObject;  //equip new armor
                PlayerData.EquippedArmor = equippedArmor; //store in playerdata
                equippedArmor.transform.SetParent(null);

                PlayerController.armor = item.armor;
                equippedArmor.SetActive(false);
                SpriteRenderer[] children3 = equippedArmor.GetComponentsInChildren<SpriteRenderer>();
                foreach (SpriteRenderer child in children3)
                {
                    if (child.name.Equals("ItemTexture"))
                    {
                        GameObject.Find("HUD Armor").GetComponent<SpriteRenderer>().sprite = child.sprite;
                    }
                }
                break;
        }
    }

    public bool IsHoldingAttack()
    {
        if (attackAction.ReadValue<Vector2>().x != 0)
            return true;
        return attackAction.ReadValue<Vector2>().y != 0;
    }
}
