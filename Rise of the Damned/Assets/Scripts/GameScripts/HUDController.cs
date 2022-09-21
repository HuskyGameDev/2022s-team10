using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    private SpriteRenderer sr;
    private SpriteRenderer srHealth;

    public Sprite[] spriteArray;
    public float overlapOpacity = 0.2f;

    private SpriteRenderer[] children;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        srHealth = transform.Find("Health").GetComponent<SpriteRenderer>();
        children = GetComponentsInChildren<SpriteRenderer>();
    }

    void Update()
    {
        ChangeSprite();
        TakeDamage();
    }

    void ChangeSprite(){
        if (PlayerController.health >= 0){
            srHealth.sprite = spriteArray[Mathf.RoundToInt((float)(PlayerController.health) / (float)(PlayerController.maxHealth) * 26f)];
        }
        else {
            srHealth.sprite = spriteArray[0];
        }
    }

    void OnTriggerEnter2D(Collider2D other){
        if (other.gameObject.tag.Equals("Player")){
            Color color = sr.color;
            color.a = overlapOpacity;
            sr.color = color;
            SpriteRenderer[] children = GetComponentsInChildren<SpriteRenderer>();
            Color childColor;
            foreach(SpriteRenderer child in children){
                childColor = child.color;
                childColor.a = overlapOpacity;
                child.color = color;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other){
        if (other.gameObject.tag.Equals("Player")){
            Color color = sr.color;
            color.a = 1.0f;
            sr.color = color;
            SpriteRenderer[] children = GetComponentsInChildren<SpriteRenderer>();
            Color childColor;
            foreach(SpriteRenderer child in children){
                childColor = child.color;
                childColor.a = 1.0f;
                child.color = color;
            }
        }
    }

    void TakeDamage()
    {
        if (Input.GetKeyDown(KeyCode.G)){
            PlayerController.health -= 5;
        }
    }
}