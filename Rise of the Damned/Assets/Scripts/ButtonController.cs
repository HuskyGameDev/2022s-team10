using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    public static GameObject buttonClick;

    void Update(){
        if (Input.GetKeyDown(KeyCode.R)){
            SceneManager.LoadScene("LevelOne");
        }
    }
}
