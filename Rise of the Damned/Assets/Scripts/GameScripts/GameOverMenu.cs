using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public static GameObject buttonClick;

    public bool isSelected;

    private SpriteRenderer sr;

    void Start(){
        sr = GetComponent<SpriteRenderer>();
    }

    void Update(){
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

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)){
            isSelected = !isSelected;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)){
            if (gameObject.name.Equals("SelectRetry") && isSelected){
                Time.timeScale = 1;
                PlayerController.isPaused = false;
                SceneManager.LoadScene("LevelOne");
            } else {
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
}
