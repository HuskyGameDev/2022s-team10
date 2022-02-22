using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class ItemController : MonoBehaviour
{
    public string type;
    public float damage, bowDamage, armor;
    public Sprite[] spriteArray;
    private SpriteRenderer sr;
    private SpriteRenderer[] children;
    void Start(){
        sr = GetComponent<SpriteRenderer>();
        children = GetComponentsInChildren<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other){
        if (other.gameObject.name.Equals("Player")){
            Color color = sr.color;
            color.a = 1.0f;
            Color childColor;
            foreach(SpriteRenderer child in children){
                if (child.tag.Equals("ItemInfo")){
                childColor = child.color;
                childColor.a = 1.0f;
                child.color = color;
                }
                if (child.name.Equals("ItemInfoNumberArmor")){
                    child.sprite = spriteArray[(int)(armor)-1];
                } else if (child.name.Equals("ItemInfoNumberSword")){
                    child.sprite = spriteArray[(int)(damage)-1];
                } else if (child.name.Equals("ItemInfoNumberBow")){
                    child.sprite = spriteArray[(int)(bowDamage)-1];
                }
            }
        }
    }
    void OnTriggerExit2D(Collider2D other){
        if (other.gameObject.tag.Equals("Player")){
            Color color = sr.color;
            color.a = 0.0f;
            Color childColor;
            foreach(SpriteRenderer child in children){
                if (child.tag.Equals("ItemInfo")){
                    childColor = child.color;
                    childColor.a = 0.0f;
                    child.color = color;
                }
            }
        }
    }
}
