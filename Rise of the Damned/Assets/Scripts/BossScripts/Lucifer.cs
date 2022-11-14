using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lucifer : MonoBehaviour
{
    public static bool isActive = false;
    public static GameObject boss;
    public static Lucifer script;

    public GameObject spikeWavePrefab;
    private GameObject spikeWave;
    private SpikeWave SW_scr;

    public GameObject throwBlock;

    private float moveTimer;
    public Vector2 moveRange;
    public float[] moveChance;

    private bool isRightSide = true;
    public float speed;

    private SpriteRenderer sr;
    
    // Start is called before the first frame update
    void Start()
    {
        boss = gameObject;
        script = this;

        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            moveTimer -= Time.deltaTime;

            if (moveTimer <= 0)
            {
                float r = Random.Range(0, 100.0f);
                float sum = 0;
                for (int i = 0; i < moveChance.Length; i++)
                {
                    if (moveChance[i] + sum >= r)
                    {
                        DoMove(i);
                        break;
                    }
                    sum += moveChance[i];
                }
            }
            Vector3 pos = new Vector3(transform.parent.position.x, transform.position.y, transform.position.z);
            pos.x += isRightSide ? 9 : -9;
            transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime * speed);

            if (pos.x == transform.position.x && sr.flipX != isRightSide)
            { 
                sr.flipX = isRightSide;
                SW_scr.ChangeDir(isRightSide);
            }
        }

    }

    //start the boss fight
    public static void StartBoss()
    {
        isActive = true;

        script.spikeWave = Instantiate(script.spikeWavePrefab, Vector3.zero, Quaternion.identity, boss.transform.parent);
        script.spikeWave.transform.localPosition = Vector3.zero;
        script.moveTimer = Random.Range(script.moveRange.x - .5f, script.moveRange.y - 1);
        script.SW_scr = script.spikeWave.GetComponent<SpikeWave>();
    }

    //tells lucifer to do the specified move
    void DoMove(int moveNum)
    {
        Debug.Log("Doing Move " + moveNum);
        moveTimer = Random.Range(moveRange.x, moveRange.y);
        switch (moveNum)
        {
            case 0: //spike wave
                SW_scr.Run();
                //do animation here
                break;
            case 1: //block throw
                GameObject block = Instantiate(throwBlock, transform.position, Quaternion.identity);
                float diffX = PlayerController.player.transform.position.x - transform.position.x;
                block.GetComponent<Rigidbody2D>().velocity = new Vector2(diffX * 1.5f, 0);
                block.GetComponent<Rigidbody2D>().angularVelocity = diffX * -15;
                //Debug.Log("DiffX: " + diffX);
                break;
            case 2: //swap side of the room
                moveTimer += 4;
                isRightSide = !isRightSide;
                break;
        }
    }
}
