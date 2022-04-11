using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainRoomGovernor : MonoBehaviour
{
    public List<GameObject> allRooms = new List<GameObject>();

    private List<GameObject> allRoomsSpawned = new List<GameObject>(); // list of all rooms spaned so far

    public GameObject spawnRoom;

    public static int tot; // same across all rooms

    void Start(){ //add spawn room
        allRoomsSpawned.Add(spawnRoom);
    }

    public void addRoom( GameObject room, int shift ){

        allRoomsSpawned.Add(room);

        tot = tot + shift;

        Debug.Log("Total shift is " + tot);

        Debug.Log("total rooms size is " + allRoomsSpawned.Count );
    }

    public GameObject getPreviousRoom(){
        
        GameObject previousRoom = allRoomsSpawned[allRoomsSpawned.Count-1];

        return previousRoom;

    }


}
