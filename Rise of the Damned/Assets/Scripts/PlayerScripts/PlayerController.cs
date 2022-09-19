using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[SelectionBase]
public class PlayerController : MonoBehaviour
{
    public GameObject roomController;
    public static GameObject player;    //static variables to be easily referenced elsewhere
    public static float health, maxHealth, meleeDamage, rangedDamage, armor;
    public static PlayerController controller;

    [System.NonSerialized]
    public float invuln = 0;
    public float invulnTime;
    private float redTime = 0;

    [System.NonSerialized]
    public Rigidbody2D rb; //unity physics engine
    private SpriteRenderer sr;
    private Collider2D thisCollider;

    [Header("Sprites")]
    public Sprite wallSlide;
    public Sprite regular; // prob change

    [Header("Basic Player Movement")]
    public float speed; // m/s
    public float jumpForce;
    public float jumpTime;

    
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
    public float gravityChangeNearWall;
    private float gravityStore; // set gravity scale in rigid body 2D
    public float timeItTakesToWallJump; // how long until a wall jump is finished jumping, you set it
    public float wallJumpCoolDown; // how often you can wall jump, prevents spamming
   

    [Header("Bools")]
    public bool facingLeft; 
    public bool facingRight;
    public bool isGrounded = false;
    public bool isJumping = false;
    public bool nearAWall = false; 
    public bool hasWallJump = true; // must start true
    public bool wallJumping;
    public bool wallSliding;
    public bool fastFalling;
    


    [Header("Better Jump")]
    public float fallMultiplier;  
    public float lowJumpMultiplier;
    public float cameraShakeBarrier;
    public float cameraShakeMagnitude;


    [Header("Particle Effects")]
    public ParticleSystem dust;

    [Header("Drag")]
    public float dragPower;
    public float dragCoefficient;

    [Header("Animations")]
    public Animator animator;

    [Header("Game Pause")]
    public static bool isPaused = false;

    public static int roomNum;


    // Start is called before the first frame update
    void Start()
    {
        player = gameObject;
        controller = this;
        //default player stats
        health = 100;
        maxHealth = 100;
        meleeDamage = 0;
        rangedDamage = 0;
        armor = 0;
        
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        thisCollider = GetComponent<Collider2D>();
        gravityStore = rb.gravityScale;
     
    }

    // Update is called once per frame
    void Update()
    {
        roomNum = (int)((transform.position.y + 12)/18);
        //Debug.Log(roomNum); it spamming it bruh 
        if (!isPaused){
            Move();
            Jump();
            CheckIfGrounded();
            CheckIfNearAWall();
            BetterJump();
            Animations();
        }
        Pause();

        if (invuln > 0)
        {
            invuln -= Time.deltaTime;
        }

        if (health <= 0){
            SceneManager.LoadScene("GameOver"); 
            // redo rooms on death
            roomController.GetComponent<MainRoomGovernor>().redoRooms();
        }

        if (redTime > 0)
        {
            redTime -= Time.deltaTime;
            if (redTime <= 0)
                sr.color = Color.white;
        }
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

        if(wallSliding) {
            sr.sprite = wallSlide;
        }
        else {
            sr.sprite = regular;
        }

        animator.SetFloat("Speed", Mathf.Abs(x)); //tells animator if player is moving
    } 

    void Jump() {
        bool jumpButton = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W);

        if(jumpButton)
        {
            if ((nearAWall || Time.time - lastTimewalled <= rememberwalledFor) && hasWallJump)
            { // Wall Jumps
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);

                hasWallJump = false;
                Invoke("SetHasWallJumpToTrue", wallJumpCoolDown); // delay that you set

                wallJumping = true;
                Invoke("SetWallJumpingToFalse", timeItTakesToWallJump); // could vary

                CreateDust();
            }
            else if (isGrounded || Time.time - lastTimeGrounded <= rememberGroundedFor)
            { // Ground Jumps. checks if player is grounded or they just moved past a groud object
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                isGrounded = false;
                isJumping = true;
            }
            else
            {
                //float timeFactor = Mathf.Max(0, (lastTimeGrounded - Time.time + jumpTime) / jumpTime);
                //rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + jumpForce * timeFactor);
            }


        }

        if ( rb.velocity.y <= -7 || (wallSliding && rb.velocity.y < -1.5) ) // -7 is after a normal ground level jump, adds realistic wall slide getting faster too
        { // fall like a sack of potatoes if you're already falling for a while
            rb.drag = (float)(Mathf.Pow(dragPower, dragCoefficient) / (double)(rb.velocity.y * -1 + Mathf.Pow(dragPower, dragCoefficient - 1) + dragPower));
            //Debug.Log("potatoes activated " + rb.velocity.y );
        }
        else {
            rb.drag = 3;
        }

        //camera shake 
        cameraShake init = FindObjectOfType<cameraShake>();
        
        if(!isGrounded && rb.velocity.y <= cameraShakeBarrier){ 
            fastFalling = true;
        }

        if(fastFalling && isGrounded){
                init.shakeCamera(cameraShakeMagnitude, .3f);
                fastFalling = false;
        }
        
    }

    void CheckIfNearAWall(){ // can change gravity scale when near a wall here
        Collider2D collider2 = Physics2D.OverlapCircle(isNearAWallChecker.position, checkWallRadius, wallLayer);

        if (collider2 != null) { 
            nearAWall = true; 

            if( Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A) ){
                rb.gravityScale = gravityChangeNearWall; 
                wallSliding = true;
            }

            else{ wallSliding = false;}

        } 

        else {
            if(nearAWall) { // just left the wall and hit it, grab time
                lastTimewalled = Time.time;
            }
            nearAWall = false;
            wallSliding = false;
            rb.gravityScale = gravityStore;
        }
         
    }

    void CheckIfGrounded() { 
        Collider2D collider = Physics2D.OverlapCircle(isGroundedChecker.position, checkGroundRadius, groundLayer); 

        if ( collider != null ) { 
            isGrounded = true;
            isJumping = false;
        } 
        else {
            unGround();
        } 
    }

    void unGround()
    {
        if (isGrounded)
        {
            lastTimeGrounded = Time.time; // Time.time holds how much time has passed since we are running our game
        }
        isGrounded = false;
        isJumping = true;
    }

    void BetterJump()
    {
        bool jumpButton = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W);

        if (nearAWall && (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)))
        { // when near a wall dont screw with the jump
            return;
        }

        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !jumpButton)
        {
            rb.velocity += (lowJumpMultiplier - 1) * Time.deltaTime * Physics2D.gravity * Vector2.up;
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

    public void CreateDust() {
        dust.Play();
    }

    public static bool TakeDamage(float damage)
    {
        if (PlayerController.controller.invuln > 0 || damage <= 0)
            return false;
        PlayerController.health -= damage - PlayerController.armor;
        PlayerController.controller.invuln = PlayerController.controller.invulnTime;
        PlayerController.controller.redTime = .2f;
        PlayerController.controller.sr.color = Color.red;
        return true;
    }

    void Animations() {
        if (isJumping) {
            animator.SetBool("IsJumping", true);
        } else {
            animator.SetBool("IsJumping", false);
        }

        if (wallSliding) {
            animator.SetBool("IsWallsliding", true);
        } else {
            animator.SetBool("IsWallsliding", false);
        }
    }

    void Pause(){
        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused){
            isPaused = true;
            Time.timeScale = 0;
        } else if (Input.GetKeyDown(KeyCode.Escape) && isPaused){
            isPaused = false;
            Time.timeScale = 1;
        }
    }
}