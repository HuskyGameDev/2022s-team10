using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randomRooms2 : MonoBehaviour
{
    public List<GameObject> rooms = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        int rand = Random.Range(0, rooms.Count);
        Instantiate(rooms[rand], transform.position, Quaternion.identity);
        rooms.Remove(rooms[rand]);
    }

    
}
