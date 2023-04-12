using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fallingRocks : MonoBehaviour
{

    [Header("Raycast")]
    public float distance = 10f;
    public LayerMask mask; 
    public bool wasAlreadyTriggered = false;
    public bool trapFinished = false;
    
    [Header("Rock")]
    public float damage = 5f;
    public float fallSpeed = .5f;
    public float bounce = 2f;
    //public float knockback = 5f;

    private Animator anim;
    private Rigidbody2D rigid;

    void Awake(){ // get things ready

        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
    }

    void Start(){
        //tell the raycast to only look at the player layer.
        mask = LayerMask.GetMask("Player");
    }

    void FixedUpdate()
    {   
        if(!trapFinished)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.down), distance, mask);
            RaycastHit2D hit2 = Physics2D.Raycast(transform.position + Vector3.left * 0.25f, transform.TransformDirection(Vector2.down), distance, mask);
            RaycastHit2D hit3 = Physics2D.Raycast(transform.position + Vector3.right * 0.25f, transform.TransformDirection(Vector2.down), distance, mask);

            if(hit || hit2 || hit3 && !wasAlreadyTriggered){

                    if(hit){
                        StartCoroutine(playerUnderRock());
                        //Debug.Log("hit player with raycast");
                    }

                    else if(hit2){
                        StartCoroutine(playerUnderRock());
                        //Debug.Log("hit player with raycast");
                    }

                    else if(hit3){
                        StartCoroutine(playerUnderRock());
                        //Debug.Log("hit player with raycast");
                    }
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.down) * distance, Color.white);
                }
        
            else{
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.down) * distance, Color.green);
                Debug.DrawRay(transform.position + Vector3.left * 0.25f, transform.TransformDirection(Vector2.down) * distance, Color.green);
                Debug.DrawRay(transform.position + Vector3.right * 0.25f, transform.TransformDirection(Vector2.down) * distance, Color.green);
                //Debug.Log("did not hit");
            }

        }
    }

    private IEnumerator playerUnderRock(){

        wasAlreadyTriggered = true;
        anim.SetBool("playerUnderRock", true);
        yield return new WaitForSeconds(1.33333f);
        anim.SetBool("playerUnderRock", false);

        RaycastHit2D isPlayerStillThere = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.down), distance, mask);
        RaycastHit2D isPlayerStillThere2 = Physics2D.Raycast(transform.position + Vector3.left * 0.25f, transform.TransformDirection(Vector2.down), distance, mask);
        RaycastHit2D isPlayerStillThere3 = Physics2D.Raycast(transform.position + Vector3.right * 0.25f, transform.TransformDirection(Vector2.down), distance, mask);

        if(isPlayerStillThere){
            if(isPlayerStillThere.collider.tag == "Player"){
                rigid.gravityScale = fallSpeed;
                trapFinished = true;
                //Debug.Log(rigid.velocity);
            }

            else{
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.down) * distance, Color.red);
                //Debug.Log("did not stay or hit");
            }
        }

        else if(isPlayerStillThere2){
            if(isPlayerStillThere2.collider.tag == "Player"){
                rigid.gravityScale = fallSpeed;
                trapFinished = true;
                //Debug.Log(rigid.velocity);
            }

            else{
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.down) * distance, Color.red);
                //Debug.Log("did not stay or hit");
            }
        }

        else if(isPlayerStillThere3){
            if(isPlayerStillThere3.collider.tag == "Player"){
                rigid.gravityScale = fallSpeed;
                trapFinished = true;
                //Debug.Log(rigid.velocity);
            }

            else{
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.down) * distance, Color.red);
                //Debug.Log("did not stay or hit");
            }
        }
        
        wasAlreadyTriggered = false;
    }

    void exitAnim(){
        anim.SetBool("exit", true);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D coll){
        //Debug.Log("rock triggered by " + coll.name);
        
        if (coll.tag == "Ground" || coll.tag == "Enemy"){
            rigid.gravityScale = 0f;
            rigid.AddForce(Vector2.up * bounce, ForceMode2D.Impulse);  
            anim.SetBool("shatter", true);
            Invoke("exitAnim", 2f);
        }

        else if (coll.tag == "Player"){
            PlayerController.TakeDamage(damage);
            //TrapKnockback(coll); buggy
            rigid.gravityScale = 0f;
            rigid.AddForce(Vector2.up * bounce, ForceMode2D.Impulse);  
            anim.SetBool("shatter", true);
            Invoke("exitAnim", 2f);
        }
    }

    //private void TrapKnockback(Collider2D collision){ //after a 2dboxcollider is triggered, use that collider to get the Rigidbody2D, then yeet
  
        // create knoockback by referencing their rigid body 
        //Rigidbody2D player = collision.GetComponent<Rigidbody2D>();
        //Vector2 direction = player.transform.position - transform.position;
        //direction = direction.normalized * knockback; //normalized sets vector to a max length of 1, we need direction times the force we want
        //player.AddForce(direction, ForceMode2D.Impulse);   
    //}

    

}
