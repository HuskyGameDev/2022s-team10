using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb; //unity physics engine

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


     //for walls
    bool nearAWall = false; 
    public LayerMask wallLayer;


    [Header("Better Jump")]
    // for better jump
    public float fallMultiplier = 2.5f;  
    public float lowJumpMultiplier = 2f;


    // movement 
    bool facingLeft; 
    bool facingRight;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
     
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Jump();
        CheckIfGrounded();
        CheckIfNearAWall();
        BetterJump();

    }

    void Move() { 

        float x = Input.GetAxisRaw("Horizontal"); // if player pressed D or right arrow x will have the value of 1
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

    void CheckIfNearAWall(){
        Collider2D collider2 = Physics2D.OverlapCircle(isNearAWallChecker.position, checkWallRadius, wallLayer);

        if (collider2 != null) { 
            nearAWall = true; 
        } 

        else {
            if(nearAWall) { // just left the wall, grab time
                lastTimewalled = Time.time;
            }
            nearAWall = false;}

         
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
    if (nearAWall) { // when near a wall dont screw with the jump
        return;
    }

    if (rb.velocity.y < 0) {
            rb.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
        } else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space)) {
            rb.velocity += Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }   
    }

    

    


    


}
