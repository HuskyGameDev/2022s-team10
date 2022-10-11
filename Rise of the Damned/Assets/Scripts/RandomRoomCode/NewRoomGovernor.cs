using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRoomGovernor : MonoBehaviour
{
    public RoomSet[] roomSets;  //holds each set of rooms to be generated in the current level

    [System.Serializable]
    /*
     * holds an individual set of rooms (ex. all red hell rooms) and the number of rooms that should be generated
     */
    public class RoomSet    
    {
        public GameObject[] rooms;
        public int totalRooms;  //actual number of rooms from the set to actually be generated
    }

    public float roomHeight = 18;
    
    // Start is called before the first frame update
    void Start()
    {
        Vector3 offset = transform.position;
        offset.x -= 0.0129f;
        offset.y += roomHeight - 3.15f; //spawn room location is at (15, 6). I hate all of you people. If you're reading this I hate you too.

        foreach(RoomSet set in roomSets)
        {
            List<int> usedRooms = new List<int>(); //list of used room indices
            for(int i = 0; i < Mathf.Min(set.totalRooms, set.rooms.Length); i++)
            {
                //get a room index not yet used
                int rand = Random.Range(0, set.rooms.Length);
                while(usedRooms.Contains(rand)) { rand = Random.Range(0, set.rooms.Length); }

                //adjust offset for room floor
                offset.x -= set.rooms[rand].GetComponent<Room>().floorShift;

                //instantiate the room
                Instantiate(set.rooms[rand], offset, Quaternion.identity);
                usedRooms.Add(rand);

                //adjust offset to the roof of the current room and the height of the next room
                offset.x += set.rooms[rand].GetComponent<Room>().roofShift;
                offset.y += roomHeight;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}