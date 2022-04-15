using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class ItemController : MonoBehaviour
{
    public enum ItemType {Sword, Bow, Armor, Health}
    public ItemType type;
    public float damage, bowDamage, armor;
    public GameObject arrow;
    public Sprite[] spriteArray;
    private SpriteRenderer sr;
    private SpriteRenderer[] children;
   

    void Start(){
        sr = GetComponent<SpriteRenderer>();
        children = GetComponentsInChildren<SpriteRenderer>();

    }

    void Update()
    {
        if (!CompareTag("HealthDrop"))
        {
            transform.Rotate(new Vector3(0, Time.deltaTime * 100, 0), Space.Self);
            foreach (Transform t in GetComponentsInChildren<Transform>())
                if (t.CompareTag("ItemInfo") || t.CompareTag("Untagged"))
                    t.eulerAngles = new Vector3(t.eulerAngles.x, 0, t.eulerAngles.z);
            //t.Rotate(new Vector3(0, Time.deltaTime * -100, 0), Space.World);
        }
    }

    void OnTriggerEnter2D(Collider2D other){
        if (other.gameObject.name.Equals("Player"))
        { 
            if (CompareTag("HealthDrop")) {
                if (PlayerController.health < PlayerController.maxHealth - 4) {
                    PlayerController.health += 5;
                } else {
                    PlayerController.health = PlayerController.maxHealth;
                }
                Destroy(gameObject);
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
                    if (damage < 10 && damage > 0) {
                        if (child.name.Equals("ItemInfoNumberSword3")) {
                            childColor = child.color;
                            childColor.a = 1.0f;
                            child.color = color;
                            child.sprite = spriteArray[(int)(damage)];
                        }
                    }

                    // Two digit damage
                    else if (damage >= 10 && damage < 100) {
                        if (child.name.Equals("ItemInfoNumberSword2") || child.name.Equals("ItemInfoNumberSword4")) {
                            childColor = child.color;
                            childColor.a = 1.0f;
                            child.color = color;
                            if (child.name.Equals("ItemInfoNumberSword2")) {
                                child.sprite = spriteArray[(int)(damage) / 10 % 10];
                            }
                            else {
                                child.sprite = spriteArray[(int)(damage) % 10];
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
                                child.sprite = spriteArray[(int)(damage) / 100 % 10];
                            }
                            else if (child.name.Equals("ItemInfoNumberSword3")) {
                                child.sprite = spriteArray[(int)(damage) / 10 % 10];
                            }
                            else {
                                child.sprite = spriteArray[(int)(damage) % 10];
                            }
                        }
                    }

                    // Bow Damage Numbers

                    // One digit bow damage
                    if (bowDamage < 10 && bowDamage > 0) {
                        if (child.name.Equals("ItemInfoNumberBow3")) {
                            childColor = child.color;
                            childColor.a = 1.0f;
                            child.color = color;
                            child.sprite = spriteArray[(int)(bowDamage)];
                        }
                    }

                    // Two digit bow damage
                    else if (bowDamage >= 10 && bowDamage < 100) {
                        if (child.name.Equals("ItemInfoNumberBow2") || child.name.Equals("ItemInfoNumberBow4")) {
                            childColor = child.color;
                            childColor.a = 1.0f;
                            child.color = color;
                            if (child.name.Equals("ItemInfoNumberBow2")) {
                                child.sprite = spriteArray[(int)(bowDamage) / 10 % 10];
                            }
                            else {
                                child.sprite = spriteArray[(int)(bowDamage) % 10];
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
                                child.sprite = spriteArray[(int)(bowDamage) / 100 % 10];
                            }
                            else if (child.name.Equals("ItemInfoNumberBow3")) {
                                child.sprite = spriteArray[(int)(bowDamage) / 10 % 10];
                            }
                            else {
                                child.sprite = spriteArray[(int)(bowDamage) % 10];
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
