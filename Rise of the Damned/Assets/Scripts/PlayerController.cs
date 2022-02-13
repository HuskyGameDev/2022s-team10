using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb; //unity physics engine

    Collider2D thisCollider;

    [Header("Basic Player Movement")]
    public float speed; // m/s
    public float jumpForce;

    // for ground
    [Header("For Ground")]
    public float rememberGroundedFor; // help to keep us grounded for a little longer, smooth out jumps just after leaving ground
    public float checkGroundRadius; // is going to tell us whats the radius of our GroundChecker
    bool isGrounded = false; 
    public Transform isGroundedChecker; // Transform of an empty object that is going to be placed bellow player
    public LayerMask groundLayer;

    float lastTimeGrounded; // when was the last time we were standing on the ground


    [Header("For Walls")]
    public Transform isNearAWallChecker; 
    public float checkWallRadius; 
    public float rememberwalledFor; 
    float lastTimewalled; 
    public float gravityChangeNearWall = 0.5f;
    private float gravityStore; // set gravity scale in rigid body 2D


     //for walls
    bool nearAWall = false; 
    public LayerMask wallLayer;

    bool hasJump = false;


    [Header("Better Jump")]
    // for better jump
    public float fallMultiplier = 2.5f;  
    public float lowJumpMultiplier = 2f;


    // movement 
    public bool facingLeft; 
    public bool facingRight;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        thisCollider = GetComponent<Collider2D>();
        gravityStore = rb.gravityScale;
     
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Jump();
        CheckIfGrounded();
        CheckIfNearAWall();
        BetterJump();
        PickupItem();

    }

    void Move() { 

        float x = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0); // if player pressed D or right arrow x will have the value of 1
        float moveBy = x * speed; 
        rb.velocity = new Vector2(moveBy, rb.velocity.y); // will move at a certain m/s

        if (x > 0) { // moved right, flip 
            gameObject.transform.localScale = new Vector2(1, 1);
            facingRight = true;
            facingLeft = false;
        }

        if (x < 0) { // moved left, flip 
            gameObject.transform.localScale = new Vector2(-1, 1);
            facingLeft = true;
            facingRight = false;
        }

    } 

    void Jump() {

        if ( (Input.GetKeyDown(KeyCode.Space)) && (isGrounded || Time.time - lastTimeGrounded <= rememberGroundedFor || nearAWall || Time.time - lastTimewalled <= rememberwalledFor)) { // checks if player is grounded or they just moved past a groud object
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void CheckIfNearAWall(){ // can change gravity scale when near a wall here
        Collider2D collider2 = Physics2D.OverlapCircle(isNearAWallChecker.position, checkWallRadius, wallLayer);

        if (collider2 != null) { 
            nearAWall = true; 
            if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.A)){
                rb.gravityScale = gravityChangeNearWall;
            }
            
        } 

        else {
            if(nearAWall) { // just left the wall, grab time
                lastTimewalled = Time.time;
            }
            nearAWall = false;
            rb.gravityScale = gravityStore;
           
            }
         
    }

    void CheckIfGrounded() { 
        Collider2D collider = Physics2D.OverlapCircle(isGroundedChecker.position, checkGroundRadius, groundLayer); 

        if (collider != null) { 
            isGrounded = true; 
        } 

        else { 
             if (isGrounded) {
                lastTimeGrounded = Time.time; // Time.time holds how much time has passed since we are running our game
                }
                isGrounded = false; 
            } 
        }

    void BetterJump() {
    if (nearAWall && Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)) { // when near a wall dont screw with the jump
        return;
    }

    if (rb.velocity.y < 0) {
            rb.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
        } else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space)) {
            rb.velocity += Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }   
    }

    void PickupItem(){ //tylers shit
        List<Collider2D> results = new List<Collider2D>();
        thisCollider.OverlapCollider(new ContactFilter2D(), results);
        foreach(Collider2D collision in results){
            if ((collision.gameObject.tag == "ItemDrop") && (Input.GetKey(KeyCode.R))){
                Destroy(collision.gameObject);
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collider){
        Debug.Log("Triggered");
    }

}