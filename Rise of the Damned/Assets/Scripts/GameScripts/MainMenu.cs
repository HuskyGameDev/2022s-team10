using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static GameObject buttonClick;

    public bool isSelected;

    private SpriteRenderer sr;

    int selectNum = 0;
    public GameObject roomController;

    public bool optionsOpen;

    void Start(){
        sr = GetComponent<SpriteRenderer>();
        Time.timeScale = 1;
        optionsOpen = false;
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            selectNum = (selectNum + 4) % 3;
        
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            selectNum = (selectNum + 2)%3;

        switch (selectNum){
            case 0:
                if (gameObject.name.Equals("SelectPlay")){
                    isSelected = true;
                } else {
                    isSelected = false;
                }
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)){
                    //MainRoomGovernor.tot = 0;
                    Time.timeScale = 1;
                    PlayerController.isPaused = false;
                    SceneManager.LoadScene("LevelOne");
                }

                break;
            case 1:
                if (gameObject.name.Equals("SelectOptions")){
                    isSelected = true;
                } else {
                    isSelected = false;
                }
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)){
                    optionsOpen = !optionsOpen;

                    Debug.Log("Options menu here");
                }

                break;
            case 2:
                if (gameObject.name.Equals("SelectQuit")){
                    isSelected = true;
                } else {
                    isSelected = false;
                }
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)){

                    Debug.Log("Quit executed");
                    Application.Quit();
                }

                break;
        }

        if (isSelected){
            Color color = sr.color;
            color.a = 1.0f;
            sr.color = color;
        }
        else {
            Color color = sr.color;
            color.a = 0.0f; 
            sr.color = color;
        }
    }
}
