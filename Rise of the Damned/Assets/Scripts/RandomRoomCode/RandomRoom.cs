using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRoom : MonoBehaviour
{
    public GameObject[] objects;

    public GameObject previousRoom; // room spawned before this room

    public GameObject thisRoom; // room about to be spawned on this spawn point

    private Vector3 shift;

    private int thisRoomShift; //based off of the previous rooms x shift value

    private int negShift; // the bottomRoomShift value or the entrance to the rooms value 

    private int getTotShift; // all shifts that previously happened combined

    public GameObject controller;
    
    // Start is called before the first frame update
    void Start() // call mainroomgov
    {
        int rand = Random.Range(0, objects.Length);

        thisRoom = objects[rand]; // get random rooms about to be spawned at this spawn point

        // move in the x axis ?
        getTotShift = MainRoomGovernor.tot; // all shifts combined
        previousRoom = controller.GetComponent<MainRoomGovernor>().getPreviousRoom(); 

        thisRoomShift = previousRoom.GetComponent<TopRoomShift>().shift; // the spawn point this script is attached too will spawn a room, the shift here is based off of the previous rooms TopRoomShift value
        negShift = thisRoom.GetComponent<BottomRoomShift>().negShift; // this room about to be spawned entrance might have an offset, that is its BottomRoomShift
        thisRoomShift = thisRoomShift + negShift; // combine the offsets together 

        // perform room shift
        shift = new Vector3(thisRoomShift + getTotShift, 0, 0); // must add total 
        this.transform.Translate(shift); // shift the spawn point

        Debug.Log("Spawning: " + thisRoom.ToString() + " shifting: " + shift.ToString() );

        // spawn room
        Instantiate(thisRoom, transform.position, Quaternion.identity);

        // add spawned room to list of spawned rooms
        controller.GetComponent<MainRoomGovernor>().addRoom(thisRoom, thisRoomShift); //add room to main list

    }

}
