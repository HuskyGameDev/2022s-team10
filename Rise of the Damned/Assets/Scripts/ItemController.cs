using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class ItemController : MonoBehaviour
{
    public string type;
    public float damage, bowDamage, armor;
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
