using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class crossbowTrap : MonoBehaviour
{
    public Transform firePoint;
    [SerializeField] private GameObject[] arrows;


    [Header("crossbow string speeds")]
    public float attackCooldown = 0.83333f; // time it takes for the animation to pull back the bow and shoot an arrow
    public float stringReturnTime = .5f; // after an arrow is shot the string moves back to neutral position 


    [Header("coroutine bools")]
    [SerializeField] private bool loadingCoroutine = false; // so update doesnt spam the couroutine and screw it up. 1 at a time
   

    [Header("bools")]
    public bool isTriggered = false; // triggered trap
    

    private Animator anim;

    void Awake() // get things ready
    {
        anim = GetComponent<Animator>();
    }

    void Update(){

        if (isTriggered){

            // pulls back bow string and then fires arrow
            anim.SetBool("active",true);
            if (!loadingCoroutine)
            StartCoroutine(ReadyCrossbow()); 
        }
    }

    private IEnumerator ReadyCrossbow(){ //animation to pull string back, also the delay befor it first shoots 
        loadingCoroutine = true;
        yield return new WaitForSeconds(attackCooldown);
        if (isTriggered){ // player left plate before it could finish drawing back an arrow, dont fire it
            Attack();
        }
        yield return new WaitForSeconds(stringReturnTime);
        loadingCoroutine = false;
        anim.SetBool("active",false);
    }

    private void Attack(){ //fire arrow
        int pos = FindArrow();
        arrows[pos].transform.SetPositionAndRotation(firePoint.position, transform.rotation); //yes
        arrows[pos].GetComponent<arrowProjectile>().ActivateProjectile();
        //Debug.Log("firePoint is " + firePoint.position + " rotation is " + transform.rotation);
    }

    private int FindArrow(){ // find arrow that is not currently active in the game 
        for (int i = 0; i < arrows.Length; i++){
            if (!arrows[i].activeInHierarchy)
                return i;
        }
        return 0;
    }

    


}
