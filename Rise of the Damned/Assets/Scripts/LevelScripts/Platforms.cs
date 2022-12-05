using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platforms : MonoBehaviour
{
    Collider2D coll;
    
    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Physics2D.IgnoreCollision(coll,     
            PlayerController.controller.thisCollider, 
            PlayerController.controller.rb.velocity.y > 0);

    }
}
