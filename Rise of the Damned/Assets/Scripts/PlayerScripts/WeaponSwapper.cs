using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwapper : MonoBehaviour
{
    // Start is called before the first frame update
    public Sprite meleeSprite;
    public Sprite rangedSprite;
    private SpriteRenderer sr;
    private static WeaponSwapper weaponSwapper;

    private float disableTime = -1;


    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;
        weaponSwapper = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(sr.enabled == true)
        {
            disableTime -= Time.deltaTime;
            if (disableTime <= 0)
                sr.enabled = false;
        }
    }

    public static void SwapWeapon(bool toMelee)
    {
        weaponSwapper.sr.enabled = true;
        weaponSwapper.sr.sprite = toMelee ? weaponSwapper.meleeSprite : weaponSwapper.rangedSprite;
        //Debug.Log("Swapping to " + (toMelee ? "Melee" : "Ranged"));
        weaponSwapper.disableTime = 1f;
    }
}
