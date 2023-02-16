using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSettings : MonoBehaviour
{
    public void OnBack(){
        PlayerPrefs.Save();
        Debug.Log("saved");
    }
}
