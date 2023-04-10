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
    private Transform[] transformChildren;
    private Transform itemDrop;
    private TextMesh[] textChildren;

    private PlayerController pcontroller;

    // Floating item variables
    private static float amplitude = 0.1f;
    private static float frequency = 1f;
    Vector3 posOffset = new Vector3 ();
    Vector3 tempPos = new Vector3 ();


    void Start(){
        transformChildren = GetComponentsInChildren<Transform>();
        textChildren = GetComponentsInChildren<TextMesh>();
        pcontroller = GameObject.Find("Player").GetComponent<PlayerController>();

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
                if (pcontroller.touchingItem){
                    return;
                }  
                pcontroller.touchingItem = true;
                foreach (Transform child in transformChildren){
                    if (child.tag == "ItemInfo"){
                        child.localScale = new Vector3(1,1,1);
                    }
                }
                foreach (TextMesh child in textChildren){
                    switch(child.name){
                        case "SwordATKText":
                            child.text = "ATK: " + meleeDamage.ToString();
                            break;
                        case "SwordSPDText":
                            child.text = "SPD: " + ((int)(meleeSpeed/100.0)).ToString();
                            break;
                        case "BowATKText":
                            child.text = "ATK: " + rangedDamage.ToString();
                            break;
                        case "BowSPDText":
                            child.text = "SPD: " + (drawbackTime*10).ToString();
                            break;
                        case "ArrowSPDText":
                            child.text = "SPD: " + projVelocity.ToString();
                            break;
                        case "ArmorDEFText":
                            child.text = "DEF: " + linearArmor.ToString();
                            break;
                        case "ArmorRESText":
                            child.text = "RES: " + constantArmor.ToString();
                            break;
                    }
                }
            }
        }
    }
    void OnTriggerExit2D(Collider2D other){
        if (other.CompareTag("Player") && CompareTag("ItemDrop"))
        {
            pcontroller.touchingItem = false;
            foreach (Transform child in transformChildren){
                if (child.tag == "ItemInfo"){
                    child.localScale = new Vector3(0,0,0);
                }
            }
        }
    }
}
