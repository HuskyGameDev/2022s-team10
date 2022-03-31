using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    public static GameObject buttonClick;

    public bool isSelected;

    private SpriteRenderer sr;

    void Start(){
        sr = GetComponent<SpriteRenderer>();
    }

    void Update(){
        if (PlayerController.isPaused){
            if (gameObject.name.Equals("OptionsArrow") || gameObject.name.Equals("MenuArrow")){
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
                    if (gameObject.name.Equals("OptionsArrow") && isSelected){
                        //SceneManager.LoadScene("LevelOne");
                        Debug.Log("Options menu prefab here");
                    } else if (gameObject.name.Equals("MenuArrow") && isSelected){
                        SceneManager.LoadScene("MainMenu");
                        PlayerController.isPaused = false;
                        Time.timeScale = 1;
                    }
                }
            }
            else {
                if (gameObject.tag.Equals("PauseItem")){
                    Color color = sr.color;
                    color.a = 1.0f;
                    sr.color = color;
                }
            }
        } else {
            //if (gameObject.tag.Equals("PauseItem")){
                Color color = sr.color;
                color.a = 0.0f;
                sr.color = color;
            //}
        }
    }
}
