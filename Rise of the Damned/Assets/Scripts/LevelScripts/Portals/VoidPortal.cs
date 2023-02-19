using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidPortal : MonoBehaviour
{

    public bool bottom; // to distinguish between portals at the bottom of the level and top of the level

    private void Start()
    {
        if(bottom)
        {
            PortalManager.portals.bottomportals.Add(new PortalManager.Portal { xvalue = transform.position.x, yvalue = transform.position.y });
        } else
        {
            PortalManager.portals.topportals.Add(new PortalManager.Portal { xvalue = transform.position.x, yvalue = transform.position.y });
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        { //if player collides with portal

            if (bottom)
            {
                //teleport to topportal underneath this one
                
                //find topportal with the closest y value below this portals y value (also taking x into account as well now)
                PortalManager.Portal closestTopPortal = null;
                float closestDistance = float.MaxValue;

                foreach (PortalManager.Portal topPortal in PortalManager.portals.topportals) // iterates through top portals
                {
                    //Debug.Log(topPortal.yvalue);
                    if (topPortal.yvalue < transform.position.y && (transform.position.y - topPortal.yvalue) + Mathf.Abs(transform.position.x - topPortal.xvalue) < closestDistance)
                    { //if top portal is below the current portals position and is the closest so far to the current portal
                        closestTopPortal = topPortal;
                        closestDistance = transform.position.y - topPortal.yvalue + Mathf.Abs(transform.position.x - topPortal.xvalue);
                    }
                }

                if (closestTopPortal != null)
                {
                    //teleport the player to the closest top portal underneath this portal
                    collision.transform.position = new Vector2(closestTopPortal.xvalue, closestTopPortal.yvalue);
                }
            }
        }
    }

}