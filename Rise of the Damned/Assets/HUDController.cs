using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    private AttackController playerScript;
    private SpriteRenderer sr;
    public Sprite[] spriteArray;

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
        sr.sprite = spriteArray[Mathf.RoundToInt((float)(playerScript.health) / (float)(playerScript.maxHealth) * 14f)];
    }

    void TakeDamage()
    {
        if (Input.GetKeyDown(KeyCode.G) || ){
            playerScript.health -= 5;
        }
    }
}