using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSwap : MonoBehaviour
{
   
    public GameObject virtualCam;

    /*
    private void Update()
    {
        if(NewRoomGovernor.currentRoom == transform.parent.gameObject)
        {
            if (!virtualCam.activeSelf)
            {
                virtualCam.SetActive(true);
                if (PlayerController.controller.rb.velocity.y > 0)
                    PlayerController.controller.rb.velocity += new Vector2(0, 5);
            }
        }
        else if(virtualCam.activeSelf)
        {
            virtualCam.SetActive(false);
        }
    }
    */

    
    private void OnTriggerEnter2D(Collider2D other)
    {

        if(other.CompareTag("Player") && !other.isTrigger)
        {
            virtualCam.SetActive(true);
            if(other.attachedRigidbody.velocity.y > 0)
                PlayerController.controller.rb.velocity += new Vector2(0, 5);
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