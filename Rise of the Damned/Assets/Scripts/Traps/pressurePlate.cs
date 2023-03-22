using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pressurePlate : MonoBehaviour
{
    public GameObject[] crossBows;
    private Animator anim;

    void Awake() // get things ready
    {
        anim = GetComponent<Animator>();
    } 

     void OnTriggerEnter2D(Collider2D collision){ //steps on plates

        if (collision.tag == "Player"){
            anim.SetBool("activated", true);

            foreach (GameObject bow in crossBows){ // use with multiple crossbows on 1 pressure plate
                bow.GetComponent<crossbowTrap>().isTriggered = true;
            }
        }
        
    }

    void OnTriggerExit2D(Collider2D collision){ // small delay just to get a shot off if you run over the plate 
        Invoke("TurnBowOff", .75f); 
    }

    void TurnBowOff(){

        foreach (GameObject bow in crossBows){ // use with multiple crossbows on 1 pressure plate
                bow.GetComponent<crossbowTrap>().isTriggered = false;
                bow.GetComponent<Animator>().SetBool("active", false);
            }
            
        anim.SetBool("activated", false);
    }

    
}
