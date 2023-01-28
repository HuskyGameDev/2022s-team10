using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TriggerBoss : MonoBehaviour
{
    public GameObject boss;
    private Vector3 bossToPoint;

    enum State {idle, triggered, active, done, dead, won}

    private State state = State.idle;
    [System.NonSerialized]
    public bool isTriggered = false;
    [System.NonSerialized]
    public bool isActive = false;
    [System.NonSerialized]
    public bool isDead = false;
    public Transform toPoint;
    public float moveSpeed;

    public GameObject entranceBlocker;
    public GameObject rightSpikes;
    public GameObject transLight;
    public GameObject rlight;
    public GameObject elight;
    public GameObject ewalls;
    [SerializeField]
    private float cameraShakeMagnitude;

    private Vector3 blockerToPoint;
    private Vector3 spikeToPoint;
    private Light2D lightSource;
    private Light2D elightSource;
    private Light2D rlightSource;

    private float timeStart = 4;
    private float timeLeft;

    private PlayerInput playerInput;

    public GameObject winText;

    // Start is called before the first frame update
    void Start()
    {
        blockerToPoint = entranceBlocker.transform.position;
        blockerToPoint.x += 2;
        spikeToPoint = rightSpikes.transform.position;
        spikeToPoint.x -= 2;
        bossToPoint = boss.transform.position;
        bossToPoint.y += 7;
        lightSource = transLight.GetComponent<Light2D>();
        elightSource = elight.GetComponent<Light2D>();
        rlightSource = rlight.GetComponent<Light2D>();

        boss.GetComponent<Rigidbody2D>().sleepMode = RigidbodySleepMode2D.StartAsleep;
        boss.GetComponent<Rigidbody2D>().Sleep();
        boss.GetComponent<Collider2D>().enabled = false;
        boss.SetActive(false);

        ewalls.SetActive(false);
        elight.SetActive(false);

        timeLeft = timeStart;

        playerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.triggered)
        {
            if (Mathf.Abs(PlayerController.player.GetComponent<Rigidbody2D>().velocity.y) < .01)
                toPoint.position = new Vector3(toPoint.position.x, PlayerController.player.transform.position.y, 0);


            PlayerController.player.transform.position = Vector3.MoveTowards(PlayerController.player.transform.position, toPoint.position, Time.deltaTime * moveSpeed);

            if (Vector3.Distance(toPoint.position, PlayerController.player.transform.position) < 0.01)
            {
                state = State.active;

                ewalls.SetActive(true);
                elight.SetActive(true);
                rlight.SetActive(false);

                cameraShake init = FindObjectOfType<cameraShake>();
                init.shakeCamera(cameraShakeMagnitude, timeLeft);

                //Debug.Log("active");

                boss.SetActive(true);

                PlayerController.isActive = true;
                //PlayerController.player.GetComponent<AttackController>().enabled = true;
            }
        }
        else if (state == State.active)
        {
            timeLeft -= Time.deltaTime;

            PlayerController.isActive = false;

            entranceBlocker.transform.position = Vector3.MoveTowards(entranceBlocker.transform.position, blockerToPoint, Time.deltaTime * (2f / timeStart));
            rightSpikes.transform.position = Vector3.MoveTowards(rightSpikes.transform.position, spikeToPoint, Time.deltaTime * (2f / timeStart));
            boss.transform.position = Vector3.MoveTowards(boss.transform.position, bossToPoint, Time.deltaTime * (7f / timeStart));

            lightSource.intensity = (timeLeft / timeStart) * 10;
            elightSource.intensity = 0.6f + (1 - (timeLeft / timeStart)) * .30f;
            //Debug.Log("elight intensity:" + elightSource.intensity);

            if (timeLeft <= 0)
            {
                state = State.done;

                PlayerController.isActive = true;

                boss.GetComponent<Rigidbody2D>().WakeUp();
                boss.GetComponent<Collider2D>().enabled = true;
                //Debug.Log("starting");
                Lucifer.StartBoss();

                //Destroy(gameObject);
            }
        }
        else if (state == State.dead)
        {
            if (timeLeft > 0)
            {
                //Debug.Log("test");
                timeLeft -= Time.deltaTime;

                entranceBlocker.transform.position = Vector3.MoveTowards(entranceBlocker.transform.position, blockerToPoint, Time.deltaTime * (2f / timeStart));
                rightSpikes.transform.position = Vector3.MoveTowards(rightSpikes.transform.position, spikeToPoint, Time.deltaTime * (2f / timeStart));
                boss.transform.position = Vector3.MoveTowards(boss.transform.position, bossToPoint, Time.deltaTime * (7f / timeStart));

                lightSource.intensity = ((timeStart - timeLeft) / timeStart) * 10;
                elightSource.intensity = 0.6f + (1 - ((timeStart - timeLeft) / timeStart)) * .30f;

            }

            if (PlayerController.player.transform.position.y > transform.position.y + 8)
            {
                state = State.won;
                PlayerController.isActive = false;

                //Destroy(PlayerController.controller.rb);

                //GameObject.Find("HUD").SetActive(false);

                SceneManager.LoadScene("LevelTwo");

                //Debug.Log("Won");
            }
        }
        else if (state == State.won)
        {

            if (elightSource.intensity >= 100)
            {
                //SceneManager.LoadScene("GameWin");
                winText.SetActive(true);
                winText.GetComponentInParent<Canvas>().worldCamera = Camera.main;

                Time.timeScale = 0;
            }
            else
                elightSource.intensity += Time.deltaTime * Mathf.Max(elightSource.intensity, 3);
        }
        else if (state == State.idle)
        {
            if (playerInput.actions["Boss"].triggered)
                PlayerController.player.transform.position = transform.position + Vector3.down * 6;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (state == State.idle)
        {
            //disable player controls
            PlayerController.isActive = false;

            //turn off normal music here

            NewRoomGovernor.killRooms();

            state = State.triggered;

        //Debug.Log("triggered");
        }
    }

    public void killBoss()
    {
        Destroy(boss.GetComponent<Rigidbody2D>());
        //boss.GetComponent<Collider2D>().enabled = false;

        cameraShake init = FindObjectOfType<cameraShake>();

        timeLeft = timeStart;

        init.shakeCamera(cameraShakeMagnitude, timeStart);

        blockerToPoint.x -= 2;
        spikeToPoint.x += 2;

        bossToPoint.x = boss.transform.position.x;
        bossToPoint.y -= 5.62f;

        state = State.dead;
    }
}
