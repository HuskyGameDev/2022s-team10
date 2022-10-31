using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play(){
        Time.timeScale = 1;
        PlayerController.isPaused = false;
        SceneManager.LoadScene("LevelOne");
    }

    public void Quit(){
        Application.Quit();
    }
}
