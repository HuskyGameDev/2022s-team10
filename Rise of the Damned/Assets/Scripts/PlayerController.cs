using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb; //unity physics engine

    SpriteRenderer sr;
    Collider2D thisCollider;

    [Header("Basic Player Movement")]
    public float speed; // m/s
    public float jumpForce;

    
    [Header("For Ground")]
    public float rememberGroundedFor; // help to keep us grounded for a little longer, smooth out jumps just after leaving ground
    public float checkGroundRadius; // is going to tell us whats the radius of our GroundChecker
    public Transform isGroundedChecker; // Transform of an empty object that is going to be placed bellow player
    public LayerMask groundLayer;
    float lastTimeGrounded; // when was the last time we were standing on the ground


    [Header("For Walls")]
    public Transform isNearAWallChecker; 
    public LayerMask wallLayer;
    public float checkWallRadius; 
    public float rememberwalledFor; 
    float lastTimewalled; 
    public float gravityChangeNearWall = 0.5f;
    private float gravityStore; // set gravity scale in rigid body 2D
    public float timeItTakesToWallJump = 0.5f; // how long until a wall jump is finished jumping, you set it
    public float wallJumpCoolDown = 0.5f; // how often you can wall jump, prevents spamming
   

    [Header("Bools")]
    public bool facingLeft; 
    public bool facingRight;
    public bool isGrounded = false; 
    public bool nearAWall = false; 
    public bool hasWallJump = true; // must start true
    public bool wallJumping;


    [Header("Better Jump")]
    public float fallMultiplier = 2.5f;  
    public float lowJumpMultiplier = 2f;


    [Header("Particle Effects")]
    public ParticleSystem dust;



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

        if( x == 0 && (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.A)) && isGrounded ){ // hard turn create dust
            CreateDust();
        }

    } 

    void Jump() {

        if ( (Input.GetKeyDown(KeyCode.Space)) && (isGrounded || Time.time - lastTimeGrounded <= rememberGroundedFor) ) { // Ground Jumps. checks if player is grounded or they just moved past a groud object
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if ( (Input.GetKeyDown(KeyCode.Space)) && ( (nearAWall || Time.time - lastTimewalled <= rememberwalledFor) && hasWallJump ) ) { // Wall Jumps
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        hasWallJump = false;
        Invoke ("SetHasWallJumpToTrue", wallJumpCoolDown); // delay that you set

        wallJumping = true;
        Invoke ("SetWallJumpingToFalse", timeItTakesToWallJump); // could vary
        CreateDust();
        }


        if (rb.velocity.y <= -6.0) { // fall like a sack of potatoes if your already falling for a while
            rb.drag = 2;
        }

        else {rb.drag = 3;}
        
    }

    void CheckIfNearAWall(){ // can change gravity scale when near a wall here
        Collider2D collider2 = Physics2D.OverlapCircle(isNearAWallChecker.position, checkWallRadius, wallLayer);

        if (collider2 != null) { 
            nearAWall = true; 

                if( Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A) ){
                    rb.gravityScale = gravityChangeNearWall; 
                    }

                if ( Input.GetKeyDown(KeyCode.Space) ) {
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

        if ( collider != null ) { 
            isGrounded = true; 
        } 

        else { 
                if (isGrounded) {
                lastTimeGrounded = Time.time; // Time.time holds how much time has passed since we are running our game
                }
                isGrounded = false; 
            } 
        }

    void BetterJump()
    {
        if (nearAWall && (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)))
        { // when near a wall dont screw with the jump
            return;
        }

        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    /*public void OnTriggerEnter2D(Collider2D collider){
        Debug.Log("Triggered");
    }*/

    void SetWallJumpingToFalse() {
        wallJumping = false; 
    }

    void SetHasWallJumpToTrue() {
        hasWallJump = true;
    }

    void CreateDust(){
        dust.Play();
    }


}