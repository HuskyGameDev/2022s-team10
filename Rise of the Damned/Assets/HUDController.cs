using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    private AttackController playerScript;
    public SpriteRenderer sr;
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
    }

    void ChangeSprite(){
        sr.sprite = spriteArray[Mathf.Clamp(0, spriteArray.Length - 1, playerScript.health / 7)];
    }
}
