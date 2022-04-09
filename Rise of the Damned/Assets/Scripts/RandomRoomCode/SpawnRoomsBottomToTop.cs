using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRoomsBottomToTop : MonoBehaviour
{
    public GameObject[] rooms;

    // Start is called before the first frame update
    void Start() //unity is stupid
    {
        foreach (GameObject room in rooms){
            room.SetActive(true);
        }
        
    }

}
