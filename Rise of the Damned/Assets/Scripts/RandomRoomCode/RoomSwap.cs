using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSwap : MonoBehaviour
{
   
public GameObject virtualCam;

private void OnTriggerEnter2D(Collider2D other)
{

    if(other.CompareTag("Player") && !other.isTrigger)
    {
        virtualCam.SetActive(true);
        PlayerController.controller.rb.velocity += new Vector2(0, 4);
    }
}

private void OnTriggerExit2D(Collider2D other)
{

    if(other.CompareTag("Player") && !other.isTrigger)
    {
        virtualCam.SetActive(false);
    }
}

}