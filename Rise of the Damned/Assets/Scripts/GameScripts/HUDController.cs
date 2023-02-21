using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    public Animator animator;

    private SpriteRenderer sr;
    private SpriteRenderer srHealth;

    public Sprite[] spriteArray;
    public float overlapOpacity = 0.2f;

    private SpriteRenderer[] children;
    private bool inProgress = false;
    private bool inMiddleAction = false;
    private bool inEndAction = false;
    private bool endActionTrigger = false;
    private Coroutine co, co2;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        srHealth = transform.Find("Health").GetComponent<SpriteRenderer>();
        children = GetComponentsInChildren<SpriteRenderer>();
    }

    void Update()
    {
        ChangeSprite();
        //TakeDamage();
    }

    void ChangeSprite(){
        if (PlayerController.health >= 0){
            srHealth.sprite = spriteArray[Mathf.Clamp(Mathf.RoundToInt((float)(PlayerController.health) / (float)(PlayerController.maxHealth) * 26f), 0, 100)];
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

    public void OnAction(){
        if (!inProgress){
            inProgress=true;
            StartCoroutine(hudStartAction());
        } else if (inMiddleAction){
            StopCoroutine(co);
            co = StartCoroutine(hudContinueAction());
        } else if (inEndAction){
            endActionTrigger = true;
            StopCoroutine(co2);
            StartCoroutine(hudStartAction());
        }
    }

    IEnumerator hudStartAction(){
        if (endActionTrigger){
            yield return new WaitForSeconds(1);
            endActionTrigger = false;
        }
        animator.SetTrigger("ShowHUD");

        yield return new WaitForSeconds(1);

        inMiddleAction = true;
        co = StartCoroutine(hudContinueAction());
    }

    public IEnumerator hudContinueAction(){
        yield return new WaitForSeconds(2);
        inMiddleAction = false;
        co2 = StartCoroutine(hudEndAction());
    }

    public IEnumerator hudEndAction(){
        inEndAction = true;
        animator.SetTrigger("HideHUD");
        yield return new WaitForSeconds(1);
        inEndAction = false;
        inProgress = false;
    }
}