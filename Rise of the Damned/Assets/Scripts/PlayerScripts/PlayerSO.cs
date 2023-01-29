using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerSO : ScriptableObject
{
    //Scriptable Object that holds information about the player
    //that can change and should persist between scenes

    [SerializeField]
    private float _health = 100;
    [SerializeField]
    private GameObject _equippedWeapon = null, _equippedBow = null, _equippedArmor = null;

    public float Health {
        get { return _health; }
        set { _health = value; }
    }

    public GameObject EquippedWeapon
    {
        get {
            Debug.Log("Returned weapon");
            return _equippedWeapon;
        }
        set { _equippedWeapon = value;
            Debug.Log("Stored: " + value.name);
        }
    }

    public GameObject EquippedBow {
        get { return _equippedBow; }
        set { _equippedBow = value; }
    }

    public GameObject EquippedArmor {
        get { return _equippedArmor; }
        set { _equippedArmor = value; }
    }

}
