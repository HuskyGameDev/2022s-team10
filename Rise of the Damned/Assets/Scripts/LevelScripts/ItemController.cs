using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class ItemController : MonoBehaviour
{
    public enum ItemType {Melee, Ranged, Armor, Health}
    public ItemType type;
    [Header("Melee")]
    public float meleeDamage;
    [Tooltip("How often you can swing the sword AFTER the previous swing ends")]
    public float meleeCooldown;
    [Tooltip("Rotation Speed of the Sword (How fast it swings)")]
    public float meleeSpeed;
    [Tooltip("How much knockback it gives enemies")]
    public float meleeKnockback;
    [Header("Ranged")]
    public float rangedDamage;
    [Tooltip("How fast & far the projectile goes")]
    public float projVelocity;
    [Tooltip("Time in seconds until maximum drawback")]
    public float drawbackTime;
    [Tooltip("How much knockback it gives enemies")]
    public float rangedKnockback;
    public GameObject arrow;
    [Header("Armor")]
    [Tooltip("Static Damage Reduction (ex. -5dmg)")]
    public float constantArmor;
    [Tooltip("Relative Damage Reduction (ex. -5% dmg)")]
    public float linearArmor;
    [System.NonSerialized]
    public float armor;

    [Header("Sprites")]
    public Sprite[] spriteArray;
    private SpriteRenderer sr;
    private SpriteRenderer[] children;
    private Transform[] transformChildren;

    // Floating item variables
    private static float amplitude = 0.1f;
    private static float frequency = 1f;
    Vector3 posOffset = new Vector3 ();
    Vector3 tempPos = new Vector3 ();


    void Start(){
        sr = GetComponent<SpriteRenderer>();
        children = GetComponentsInChildren<SpriteRenderer>();
        transformChildren = GetComponentsInChildren<Transform>();

        //like uhh... some bullshit. @Tyler This will need to be changed when equipment gui gets reworked
        armor = constantArmor + linearArmor;
    }

    void Update()
    {
        if (!CompareTag("HealthDrop")){
            posOffset = transform.position;
            foreach (Transform t in GetComponentsInChildren<Transform>()){
                if (t.CompareTag("ItemInfo") || t.CompareTag("Untagged")){
                    t.eulerAngles = new Vector3(t.eulerAngles.x, 0, t.eulerAngles.z);
                }
                else if (t.name.Equals("ItemTexture")){
                    tempPos = posOffset;
                    tempPos.y += Mathf.Sin (Time.fixedTime * Mathf.PI * frequency) * amplitude;
                    t.position = tempPos;
                    t.Rotate(new Vector3(0, Time.deltaTime * 100, 0), Space.Self);
                    Transform tchild = t.GetChild(0);
                    if (t.eulerAngles.y % 360 > 90 && t.eulerAngles.y % 360 < 270)
                        tchild.localPosition = new Vector3(Mathf.Abs(tchild.localPosition.x) * -1, tchild.localPosition.y, 0);
                    else
                        tchild.localPosition = new Vector3(Mathf.Abs(tchild.localPosition.x), tchild.localPosition.y, 0);
                    //Debug.Log("Rotation: " + (t.eulerAngles) + "\tLocal Rotation: " + t.localEulerAngles + "\tChild: " + t.GetChild(0).name + "\tFlip: " + t.GetChild(0).GetComponent<SpriteRenderer>().flipX);
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other){
        if (other.gameObject.name.Equals("Player"))
        { 
            if (CompareTag("HealthDrop")) {
                GameObject.Find("HUD").GetComponent<HUDController>().OnAction();
                if (PlayerController.health < PlayerController.maxHealth - 9) {
                    PlayerController.health += 10;
                } else {
                    PlayerController.health = PlayerController.maxHealth;
                }
                Destroy(gameObject);
                //Play HeartPickup sound
                FindObjectOfType<AudioManager>().Play("HeartPickup");
            }
            else if (CompareTag("ItemDrop")) {
                Color color = sr.color;
                color.a = 1.0f;
                Color childColor;
                foreach (SpriteRenderer child in children) {
                    if (child.CompareTag("ItemInfo")) {
                        childColor = child.color;
                        childColor.a = 1.0f;
                        child.color = color;
                    }

                    // Sword damage numbers. Yes this could probably be more efficient. No I won't fix it right now Kolby.

                    // One digit damage
                    if (meleeDamage < 10 && meleeDamage > 0) {
                        if (child.name.Equals("ItemInfoNumberSword3")) {
                            childColor = child.color;
                            childColor.a = 1.0f;
                            child.color = color;
                            child.sprite = spriteArray[(int)(meleeDamage)];
                        }
                    }

                    // Two digit damage
                    else if (meleeDamage >= 10 && meleeDamage < 100) {
                        if (child.name.Equals("ItemInfoNumberSword2") || child.name.Equals("ItemInfoNumberSword4")) {
                            childColor = child.color;
                            childColor.a = 1.0f;
                            child.color = color;
                            if (child.name.Equals("ItemInfoNumberSword2")) {
                                child.sprite = spriteArray[(int)(meleeDamage) / 10 % 10];
                            }
                            else {
                                child.sprite = spriteArray[(int)(meleeDamage) % 10];
                            }
                        }
                    }

                    // Three digit damage
                    else {
                        if (child.name.Equals("ItemInfoNumberSword1") || child.name.Equals("ItemInfoNumberSword3") || child.name.Equals("ItemInfoNumberSword5")) {
                            childColor = child.color;
                            childColor.a = 1.0f;
                            child.color = color;
                            if (child.name.Equals("ItemInfoNumberSword1")) {
                                child.sprite = spriteArray[(int)(meleeDamage) / 100 % 10];
                            }
                            else if (child.name.Equals("ItemInfoNumberSword3")) {
                                child.sprite = spriteArray[(int)(meleeDamage) / 10 % 10];
                            }
                            else {
                                child.sprite = spriteArray[(int)(meleeDamage) % 10];
                            }
                        }
                    }

                    // Bow Damage Numbers

                    // One digit bow damage
                    if (rangedDamage < 10 && rangedDamage > 0) {
                        if (child.name.Equals("ItemInfoNumberBow3")) {
                            childColor = child.color;
                            childColor.a = 1.0f;
                            child.color = color;
                            child.sprite = spriteArray[(int)(rangedDamage)];
                        }
                    }

                    // Two digit bow damage
                    else if (rangedDamage >= 10 && rangedDamage < 100) {
                        if (child.name.Equals("ItemInfoNumberBow2") || child.name.Equals("ItemInfoNumberBow4")) {
                            childColor = child.color;
                            childColor.a = 1.0f;
                            child.color = color;
                            if (child.name.Equals("ItemInfoNumberBow2")) {
                                child.sprite = spriteArray[(int)(rangedDamage) / 10 % 10];
                            }
                            else {
                                child.sprite = spriteArray[(int)(rangedDamage) % 10];
                            }
                        }
                    }

                    // Three digit bow damage
                    else {
                        if (child.name.Equals("ItemInfoNumberBow1") || child.name.Equals("ItemInfoNumberBow3") || child.name.Equals("ItemInfoNumberBow5")) {
                            childColor = child.color;
                            childColor.a = 1.0f;
                            child.color = color;
                            if (child.name.Equals("ItemInfoNumberBow1")) {
                                child.sprite = spriteArray[(int)(rangedDamage) / 100 % 10];
                            }
                            else if (child.name.Equals("ItemInfoNumberBow3")) {
                                child.sprite = spriteArray[(int)(rangedDamage) / 10 % 10];
                            }
                            else {
                                child.sprite = spriteArray[(int)(rangedDamage) % 10];
                            }
                        }
                    }

                    // Armor Numbers

                    

                    // One digit armor
                    if (armor < 10 && armor > 0) {
                        if (child.name.Equals("ItemInfoNumberArmor3")) {
                            childColor = child.color;
                            childColor.a = 1.0f;
                            child.color = color;
                            child.sprite = spriteArray[(int)(armor)];
                        }
                    }

                    // Two digit armor
                    else if (armor >= 10 && armor < 100) {
                        if (child.name.Equals("ItemInfoNumberArmor2") || child.name.Equals("ItemInfoNumberArmor4")) {
                            childColor = child.color;
                            childColor.a = 1.0f;
                            child.color = color;
                            if (child.name.Equals("ItemInfoNumberArmor2")) {
                                child.sprite = spriteArray[(int)(armor) / 10 % 10];
                            }
                            else {
                                child.sprite = spriteArray[(int)(armor) % 10];
                            }
                        }
                    }

                    // Three digit armor
                    else {
                        if (child.name.Equals("ItemInfoNumberArmor1") || child.name.Equals("ItemInfoNumberArmor3") || child.name.Equals("ItemInfoNumberArmor5")) {
                            childColor = child.color;
                            childColor.a = 1.0f;
                            child.color = color;
                            if (child.name.Equals("ItemInfoNumberArmor1")) {
                                child.sprite = spriteArray[(int)(armor) / 100 % 10];
                            }
                            else if (child.name.Equals("ItemInfoNumberArmor3")) {
                                child.sprite = spriteArray[(int)(armor) / 10 % 10];
                            }
                            else {
                                child.sprite = spriteArray[(int)(armor) % 10];
                            }
                        }
                    }
                }
            }
        }
    }
    void OnTriggerExit2D(Collider2D other){
        if (other.CompareTag("Player") && CompareTag("ItemDrop"))
        {
            Color color = sr.color;
            color.a = 0.0f;
            Color childColor;
            foreach(SpriteRenderer child in children){
                if (child.CompareTag("ItemInfo") || child.CompareTag("ItemInfoNum"))
                {
                    childColor = child.color;
                    childColor.a = 0.0f;
                    child.color = color;
                }
            }
        }
    }
}
