using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainRoomGovernor : MonoBehaviour
{
    private List<GameObject> allRoomsSpawned = new List<GameObject>();

    private GameObject roomSpawned;

    public GameObject spawnRoom;

    public static int tot;

    private int shift;

    void Start(){ //add spawn room
        allRoomsSpawned.Add(spawnRoom);
    }

    public void addRoom( GameObject room, int shift ){

        allRoomsSpawned.Add(room);

        Debug.Log("MainRoomGov added " + room.ToString() );

        tot = tot + shift;

        Debug.Log("Total shift is " + tot);

        Debug.Log("total rooms size is " + allRoomsSpawned.Count );
    }

    public GameObject getPreviousRoom(){
        
        GameObject previousRoom = allRoomsSpawned[allRoomsSpawned.Count-1];

        return previousRoom;

    }


}
