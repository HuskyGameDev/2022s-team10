using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class TriggerBoss : MonoBehaviour
{
    public GameObject boss;
    private Vector3 bossToPoint;
    [System.NonSerialized]
    public bool isTriggered = false;
    [System.NonSerialized]
    public bool isActive = false;
    public Transform toPoint;
    public float moveSpeed;

    public GameObject entranceBlocker;
    public GameObject transLight;
    public GameObject ewalls;
    [SerializeField]
    private float cameraShakeMagnitude;

    private Vector3 blockerToPoint;
    private Light2D lightSource;

    private float timeStart = 4;
    private float timeLeft;

    // Start is called before the first frame update
    void Start()
    {
        blockerToPoint = entranceBlocker.transform.position;
        blockerToPoint.x += 2;
        bossToPoint = boss.transform.position;
        bossToPoint.y += 7;
        lightSource = transLight.GetComponent<Light2D>();

        boss.GetComponent<Rigidbody2D>().sleepMode = RigidbodySleepMode2D.StartAsleep;
        boss.GetComponent<Rigidbody2D>().Sleep();
        boss.GetComponent<Collider2D>().enabled = false;
        boss.SetActive(false);

        ewalls.SetActive(false);

        timeLeft = timeStart;
    }

    // Update is called once per frame
    void Update()
    {
        if(isTriggered)
        {
            if (Mathf.Abs(PlayerController.player.GetComponent<Rigidbody2D>().velocity.y) < .01)
                toPoint.position = new Vector3(toPoint.position.x, PlayerController.player.transform.position.y, 0);


            PlayerController.player.transform.position = Vector3.MoveTowards(PlayerController.player.transform.position, toPoint.position, Time.deltaTime * moveSpeed);

            if (Vector3.Distance(toPoint.position, PlayerController.player.transform.position) < 0.01)
            {
                isTriggered = false;
                isActive = true;

                ewalls.SetActive(true);

                cameraShake init = FindObjectOfType<cameraShake>();
                init.shakeCamera(cameraShakeMagnitude, timeLeft);

                Debug.Log("active");

                boss.SetActive(true);

                PlayerController.controller.enabled = true;
            }
        }
        else if(isActive)
        {
            timeLeft -= Time.deltaTime;

            PlayerController.controller.enabled = true;

            entranceBlocker.transform.position = Vector3.MoveTowards(entranceBlocker.transform.position, blockerToPoint, Time.deltaTime * (2f/timeStart));
            boss.transform.position = Vector3.MoveTowards(boss.transform.position, bossToPoint, Time.deltaTime * (7f / timeStart));

            lightSource.intensity = (timeLeft / timeStart) * 10;

            if (timeLeft <= 0)
            {
                isActive = false;

                PlayerController.controller.enabled = true;
                PlayerController.player.GetComponent<AttackController>().enabled = true;

                boss.GetComponent<Rigidbody2D>().WakeUp();
                boss.GetComponent<Collider2D>().enabled = true;
                Debug.Log("starting");

                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //disable player controls
        PlayerController.controller.enabled = false;
        PlayerController.player.GetComponent<AttackController>().enabled = false;

        //turn off normal music here


        isTriggered = true;

        Debug.Log("triggered");
    }
}
