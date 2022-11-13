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

    public GameObject throwBlock;

    private float moveTimer;
    public Vector2 moveRange;
    
    // Start is called before the first frame update
    void Start()
    {
        boss = gameObject;
        script = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(isActive)
        {
            moveTimer -= Time.deltaTime;
            
            if(moveTimer <= 0)
            {
                DoMove(Random.Range(0, 2));
                moveTimer = Random.Range(moveRange.x, moveRange.y);
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
    }

    //tells lucifer to do the specified move
    void DoMove(int moveNum)
    {
        switch(moveNum)
        {
            case 0: //spike wave
                spikeWave.GetComponent<SpikeWave>().Run();
                //Debug.Log("Doing Move 0");
                //do animation here
                break;
            case 1: //block throw
                GameObject block = Instantiate(throwBlock, transform.position, Quaternion.identity);
                float diffX = PlayerController.player.transform.position.x - transform.position.x;
                block.GetComponent<Rigidbody2D>().velocity = new Vector2(diffX * 1.5f, 0);
                block.GetComponent<Rigidbody2D>().angularVelocity = diffX * -15;
                Debug.Log("DiffX: " + diffX);
                break;
            case 2:

                break;
        }
    }
}
