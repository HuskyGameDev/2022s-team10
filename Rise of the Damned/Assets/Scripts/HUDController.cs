using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    private AttackController playerScript;
    private SpriteRenderer sr;
    public Sprite[] spriteArray;
    public float overlapOpacity = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        playerScript = GameObject.Find("Player").GetComponent<AttackController>();
    }

    // Update is called once per frame
    void Update()
    {
        ChangeSprite();
        TakeDamage();
    }

    void ChangeSprite(){
        //sr.sprite = spriteArray[Mathf.Clamp(0, spriteArray.Length - 1, playerScript.health / 7)];
        sr.sprite = spriteArray[Mathf.RoundToInt((float)(playerScript.health) / (float)(playerScript.maxHealth) * 13f)];
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
            playerScript.health -= 5;
        }
    }
}