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
                DoMove(Random.Range(0, 1));
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
        }
    }
}
