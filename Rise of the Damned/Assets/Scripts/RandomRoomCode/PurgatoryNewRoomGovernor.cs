using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurgatoryNewRoomGovernor : MonoBehaviour
{
    public RoomSet[] roomSets;  //holds each set of rooms to be generated in the current level
    public GameObject bossRoom; //hold the boss room to be put at the end of the level generation
    public GameObject spawnRoom;
    public static List<GameObject> spawnedRooms;
    private static GameObject spawn;

    private static int roomNum; //current room the player is in starting at 0
    public static int section; //holds the section that the player is currently in
                               //0 is hell, 1 is caves, 2 is the first boss, 3 is purgatory

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
        spawnedRooms = new List<GameObject>();
        spawn = spawnRoom;
        section = -1;

        Vector3 offset = transform.position;
        offset.y = roomHeight;
        offset.x = 26;

        int curRoom = 0;    //room number of the room being insantiated

        foreach (RoomSet set in roomSets)
        {
            List<int> usedRooms = new List<int>(); //list of used room indices
            for (int i = 0; i < Mathf.Min(set.totalRooms, set.rooms.Length); i++)
            {
                //get a room index not yet used
                int rand = Random.Range(0, set.rooms.Length);
                while (usedRooms.Contains(rand)) { rand = Random.Range(0, set.rooms.Length); }

                //adjust offset for room floor
                offset.x -= set.rooms[rand].GetComponent<Room>().floorShift;

                //instantiate the room
                curRoom++;
                GameObject room = Instantiate(set.rooms[rand], offset, Quaternion.identity);
                spawnedRooms.Add(room);
                usedRooms.Add(rand);

                //adjust offset to the roof of the current room and the height of the next room
                offset.x += set.rooms[rand].GetComponent<Room>().roofShift;
                offset.y += roomHeight;

                //scaling difficulty
                foreach (EnemyController e in room.GetComponentsInChildren<EnemyController>())
                {
                    e.damage *= 1 + (curRoom * 0.05f);
                    e.health *= 1 + (curRoom * 0.10f);
                }
            }
        }

        //Instantiate(bossRoom, offset, Quaternion.identity); //Instantiate the bossroom at the top
    }

    // Update is called once per frame
    void Update()
    {
        //Controls the music changes for each section
        roomNum = (int)((PlayerController.controller.rb.position.y + 12) / 18);
        if (roomNum < roomSets[0].totalRooms + 1)
        {
            if (section == -1)
            {
                //play section 1 music
                FindObjectOfType<AudioManager>().Play("PurgatoryTheme");
            }
            section = 0;
        } else if (roomNum < roomSets[0].totalRooms + roomSets[1].totalRooms + 1)
        {
            if (section == 0)
            {
                //play section 2 music
                //FindObjectOfType<AudioManager>().Play("CaveTheme");
            }
            section = 1;
        } else if (roomNum < roomSets[0].totalRooms + roomSets[1].totalRooms + 1 + 1)
        {
            if (section == 1)
            {
                //play section 3 music
                //FindObjectOfType<AudioManager>().Play("PurgatoryTheme");
            }
            section = 2;
        }
        //Debug.Log(section);
    }

    public static void killRooms()
    {
        foreach (GameObject room in spawnedRooms)
            Destroy(room);
        Destroy(spawn);
        Destroy(GameObject.Find("Templates"));
    }
}
