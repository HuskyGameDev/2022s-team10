using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRoomsBottomToTop : MonoBehaviour
{
    public GameObject room2Spawn;

    public GameObject room3Spawn;


    // Start is called before the first frame update
    void Start() //unity is stupid
    {
        room2Spawn.SetActive(true);
        room3Spawn.SetActive(true);

        
    }

}
