using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Audio;
using System;
using Random = UnityEngine.Random;

[SelectionBase]
public class PlayerController : MonoBehaviour
{ 
    public static bool isActive = true;

    private PlayerInput playerInput;
    private InputAction moveAction, jumpAction, pauseAction;

    public GameObject roomController;
    public static GameObject player;    //static variables to be easily referenced elsewhere
    public static float health, maxHealth, meleeDamage, rangedDamage, armor, x;
    public static PlayerController controller;
    public static AttackController attackController;
    public LevelChanger lc;

    [System.NonSerialized]
    public float invuln = 0;
    public float invulnTime;
    private float redTime = 0;

    [System.NonSerialized]
    public Rigidbody2D rb; //unity physics engine
    private SpriteRenderer sr;
    [System.NonSerialized]
    public Collider2D thisCollider;

    [Header("Sprites")]
    public Sprite wallSlide;
    public Sprite regular; // prob change

    [Header("Basic Player Movement")]
    public float speed; // m/s
    public float jumpForce;
    public float jumpTime;

    [Header("Movement")]
    [SerializeField]
    private float max_speed = 6f;
    [SerializeField]
    private float max_acceleration = 80f;
    private Vector2 direction, desired_velocity, velocity;
    private float acceleration;
    [SerializeField]
    private float max_speed_change;
    [SerializeField]
    private float max_air_acceleration = 80f;
    [SerializeField]
    private float max_wall_acceleration = 80f;
    [SerializeField]
    private float knockback_time = .3f; //time in seconds knockback is in effect 


    [Header("For Ground")]
    public float rememberGroundedFor; // help to keep us grounded for a little longer, smooth out jumps just after leaving ground
    public float checkGroundRadius; // is going to tell us whats the radius of our GroundChecker
    public Transform isGroundedChecker; // Transform of an empty object that is going to be placed bellow player
    public LayerMask groundLayer;
    float lastTimeGrounded; // when was the last time we were standing on the ground


    [Header("For Walls")]
    public Transform WallChecker1;
    public Transform WallChecker2;
    public int WallDirection; // -1 is no wall, 0 is left, 1 is right
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
    public bool nearAWall = false; 
    public bool hasWallJump = true; // must start true
    public bool wallJumping;
    public bool wallSliding;
    public bool falling;
    public bool fastFalling;
    public bool walking;
    private bool isJumping = false; // is true when player is trying to jump
    private bool receivingKnockback = false;
    


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

    public static int roomNum; // current room the player is in, starts at 0

    private Coroutine walk;

    public GameObject pauseMenu;
    public GameObject optionsMenu;

    //Sound
    [SerializeField] AudioMixer mixer;

    [SerializeField]
    private PlayerSO PlayerData; //stored player data to persist between scenes


    // Start is called before the first frame update
    void Start()
    {
        player = gameObject;
        controller = this;
        //default player stats
        health = PlayerData.Health;
        maxHealth = 100;
        meleeDamage = 0;
        rangedDamage = 0;
        armor = 0;
        
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        thisCollider = GetComponent<Collider2D>();
        gravityStore = rb.gravityScale;
     
        playerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        pauseAction = playerInput.actions["Pause"];

        roomNum = (int)((transform.position.y + 12)/18);
        //Debug.Log(roomNum); it spamming it bruh 
        if (!isPaused && isActive){
            MoveInput();
            Move();
            Jump();
            CheckIfGrounded();
            CheckIfNearAWall();
            BetterJump();
            Animations();
            iFrames();
            GameOver();
        }
        Pause();
        //Debug.Log(GetComponent<AttackController>().enabled);
    }

    public void MoveInput(){
        direction = moveAction.ReadValue<Vector2>();

        if (direction.x == 1){
            if(facingLeft && isGrounded)
            {
                CreateDust();
            }
            facingRight = true;
            facingLeft = false;
            gameObject.transform.localScale = new Vector2(1, 1);
        } else if (direction.x == -1) {
            if (facingRight && isGrounded)
            {
                CreateDust();
            }
            facingLeft = true;
            facingRight = false;
            gameObject.transform.localScale = new Vector2(-1, 1);
        }
        if (jumpAction.triggered){
            isJumping = true;
        }
    }

    private void Move() {

        desired_velocity = new Vector2(direction.x, 0f) * Mathf.Max(max_speed, 0f);


        velocity = rb.velocity;

        if (wallSliding) {
            acceleration = max_wall_acceleration; //if on wall change to wall acceleration
        } else if (!isGrounded) {
            acceleration = max_air_acceleration; //if in air change to air acceleration
        } else {
            acceleration = max_acceleration;
        }

        max_speed_change = acceleration * Time.deltaTime;

        if(desired_velocity.x != 0 || !receivingKnockback)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, desired_velocity.x, max_speed_change);
        }
        

        rb.velocity = velocity;

        animator.SetFloat("Speed", Mathf.Abs(desired_velocity.x)); //tells animator if player is moving

        if((desired_velocity.x > .01f || desired_velocity.x < -.01f) && isGrounded)
        {
            if(!walking)
            {
                if (walk != null) { StopCoroutine(walk); }
                walk = StartCoroutine(Walk());
            }
            walking = true;
        } else
        {
            if (walk != null) { StopCoroutine(walk); }
            walking = false;
        }
    }

    IEnumerator Walk()
    {
        while(true)
        {
            //Play player step
            int n = (int) Random.Range(1f, 5f);
            FindObjectOfType<AudioManager>().Play("Step" + n);

            yield return new WaitForSeconds(.2f);
        }
    }

    void Jump() {
        //bool jumpButton = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W);

        if(isJumping)
        {
            isJumping = false;
            if ((nearAWall || Time.time - lastTimewalled <= rememberwalledFor) && hasWallJump)
            { // Wall Jumps
                if(WallDirection == 0)
                    rb.velocity = new Vector2(jumpForce, jumpForce * 1.2f);
                else if(WallDirection == 1)
                    rb.velocity = new Vector2(-jumpForce, jumpForce * 1.2f);

                hasWallJump = false;
                Invoke("SetHasWallJumpToTrue", wallJumpCoolDown); // delay that you set

                wallJumping = true;
                Invoke("SetWallJumpingToFalse", timeItTakesToWallJump); // could vary

                CreateDust();

                //Play jump sound
                FindObjectOfType<AudioManager>().Play("Jump");
            }
            else if (isGrounded || Time.time - lastTimeGrounded <= rememberGroundedFor)
            { // Ground Jumps. checks if player is grounded or they just moved past a groud object
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                isGrounded = false;

                //Play jump sound
                FindObjectOfType<AudioManager>().Play("Jump");
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
            
        } else
        {
            rb.drag = 3;
        }

        //camera shake 
        cameraShake init = FindObjectOfType<cameraShake>();
        
        if(!isGrounded && rb.velocity.y <= cameraShakeBarrier)
        { 
            fastFalling = true;
        } else if(!isGrounded && rb.velocity.y < -5)
        {
            falling = true;
        }

        if(fastFalling && isGrounded){
            init.shakeCamera(cameraShakeMagnitude, .3f);
            fastFalling = false;
        } else if (falling && isGrounded)
        {
            falling = false;
        }
        
    }

    void CheckIfNearAWall(){ // can change gravity scale when near a wall here
        Collider2D collider1 = Physics2D.OverlapCircle(WallChecker1.position, checkWallRadius, wallLayer);

        Collider2D collider2 = Physics2D.OverlapCircle(WallChecker2.position, checkWallRadius, wallLayer);

        if (collider1 != null) { 
            nearAWall = true;
            WallDirection = 1;
            if (facingLeft) 
            {
                WallDirection -= 1;
            }
            

            if( moveAction.ReadValue<Vector2>().x != 0 ){
                rb.gravityScale = gravityChangeNearWall; 
                wallSliding = true;
            }

            else{ wallSliding = false;}

        }
        else if (collider2 != null)
        {
            nearAWall = true;
            WallDirection = 0;
            if(facingLeft)
            {
                WallDirection += 1;
            }

            if (moveAction.ReadValue<Vector2>().x != 0)
            {
                rb.gravityScale = gravityChangeNearWall;
                wallSliding = true;
            } else { wallSliding = false; }

        } else {
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
            if (!isGrounded)
            {
                //Play Landing sound
                if (fastFalling)
                {
                    //Hard landing
                    FindObjectOfType<AudioManager>().Play("HardLanding");
                    transform.localScale = new Vector2(1, .7f);
                } else if (falling)
                {
                    //Soft landing
                    //FindObjectOfType<AudioManager>().Play("Landing");
                }
            }
            isGrounded = true;
        } 
        else {
            if (isGrounded)
            {
                lastTimeGrounded = Time.time; // Time.time holds how much time has passed since we are running our game
            }
            isGrounded = false;
        } 
    }

    void BetterJump()
    {
        if (nearAWall && (moveAction.ReadValue<Vector2>().x != 0))
        { // when near a wall dont screw with the jump
            return;
        }

        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !(jumpAction.ReadValue<float>() != 0))
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

    public void CreateDust() 
    {
        dust.Play();
    }

    void iFrames() 
    {
        if (redTime > 0) {
            redTime -= Time.deltaTime;
            if (redTime <= 0)
                sr.color = Color.white;
        }

        if (invuln > 0) {
            invuln -= Time.deltaTime;
        }
    }
    public static bool TakeDamage(float damage)
    {
        if (PlayerController.controller.invuln > 0 || damage <= 0)
            return false;
        PlayerController.health -= Mathf.Max(damage - PlayerController.armor, 0);
        PlayerController.controller.invuln = PlayerController.controller.invulnTime;
        PlayerController.controller.redTime = .2f;
        PlayerController.controller.sr.color = Color.red;
        return true;
    }

    void Animations() 
    {
        if (!isGrounded) {
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

    void Pause()
    {
        if (pauseAction.triggered && !isPaused){
            Paused();
        } 
        else if (pauseAction.triggered && isPaused){
            Resumed();
        }
    }

    void Paused(){
        isPaused = true;
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        optionsMenu.SetActive(false);
        // put low pass on music
        mixer.SetFloat("MusicLowpassFilter", 1500);
    }

    public void Resumed(){
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        isPaused = false;
        Time.timeScale = 1;
        // remove low pass from music
        mixer.SetFloat("MusicLowpassFilter", 22000);
    }

    public void Quit(){
        armor = 999;
        Time.timeScale = 1;
        lc.FadeToLevel();
        Invoke("OnQuit", 1.0f);
    }

    public void OnQuit(){
        isActive = true;
        SceneManager.LoadScene("MainMenu");
        // play main menu music
        mixer.SetFloat("MusicLowpassFilter", 22000);
        FindObjectOfType<AudioManager>().Play("MainTheme");
    }

    void GameOver()
    {
        if (health <= 0)
        {
            transform.localScale = new Vector3(transform.localScale.x, 1, 0);
            SceneManager.LoadScene("GameOver");
            // redo rooms on death
            //roomController.GetComponent<MainRoomGovernor>().redoRooms();
        }
    }


    public void Knockback(float knockback, Transform knockback_location)
    {
        receivingKnockback = true;
        float horizontal_enemy_direction = (knockback_location.position.x - rb.position.x) / Mathf.Abs(knockback_location.position.x - rb.position.x); // horizontal vector distance from player to enemy
        float vertical_enemy_direction = (knockback_location.position.y - (rb.position.y + (float).45)) / Mathf.Abs(knockback_location.position.y - (rb.position.y + (float).45)); 

        rb.AddForce(new Vector2(knockback * -horizontal_enemy_direction, knockback * -vertical_enemy_direction * (float).75), ForceMode2D.Impulse);
        Invoke("SetReceivingKnockback", knockback_time);
    }

    public void SpikeKnockback(float knockback, Vector2 knockbackdir)
    {
        receivingKnockback = true;

        rb.AddForce(new Vector2(knockback * knockbackdir.x, knockback * knockbackdir.y), ForceMode2D.Impulse);
        Invoke("SetReceivingKnockback", knockback_time);
    }

    private void SetReceivingKnockback() //called when knockback is over
    {
        receivingKnockback = false;
    }

}