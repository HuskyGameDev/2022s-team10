using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRoom : MonoBehaviour
{
    public GameObject[] objects;

    public GameObject previousRoom;

    public GameObject nextRoom;

    private Vector3 shift;

    private int nextRoomShift; //based off of the previous rooms x shift value

    private int getTotShift;

    public GameObject controller;
    
    // Start is called before the first frame update
    void Start() // call mainroomgov
    {
        int rand = Random.Range(0, objects.Length);

        nextRoom = objects[rand];

        // move x ?
        getTotShift = MainRoomGovernor.tot; // all shifts combined
        previousRoom = controller.GetComponent<MainRoomGovernor>().getPreviousRoom();

        nextRoomShift = previousRoom.GetComponent<RoomXShift>().shift; // the spawn point this script is attached too will spawn a room, the shift is based off of the previous room
        shift = new Vector3(nextRoomShift + getTotShift, 0, 0); // must add total 
        this.transform.Translate(shift); // shift the spawn point

        Debug.Log("Spawning: " + nextRoom.ToString() + " shifting: " + shift.ToString() );

        Instantiate(nextRoom, transform.position, Quaternion.identity);

        controller.GetComponent<MainRoomGovernor>().addRoom(nextRoom, nextRoomShift); //add room to main list

    }

}
