using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRoom : MonoBehaviour
{
    public GameObject[] objects;

    public GameObject previousRoom;

    public GameObject nextRoom;

    private Vector3 shift;

    private int nextRoomShift;

    private int getTotShift;

    public GameObject controller;
    
    // Start is called before the first frame update
    void Start() // call mainroomgov
    {
        int rand = Random.Range(0, objects.Length);

        nextRoom = objects[rand];

        // move x ?
        getTotShift = MainRoomGovernor.tot;
        previousRoom = controller.GetComponent<MainRoomGovernor>().getPreviousRoom();

        nextRoomShift = previousRoom.GetComponent<RoomXShift>().shift + getTotShift;
        shift = new Vector3(nextRoomShift, 0, 0);
        this.transform.Translate(shift);

        Debug.Log("Spawning: " + nextRoom.ToString() + " shifting: " + shift.ToString() );

        Instantiate(nextRoom, transform.position, Quaternion.identity);

        controller.GetComponent<MainRoomGovernor>().addRoom(nextRoom, nextRoomShift); //add room to main list

    }

}
