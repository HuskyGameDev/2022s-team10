using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// OLD SCRIPT. Replaced by MenuController.cs
public class MainMenu : MonoBehaviour
{

    public void Play(){
        Time.timeScale = 1;
        PlayerController.isPaused = false;
        SceneManager.LoadScene("LevelOne");
        // play the in game music
        //FindObjectOfType<AudioManager>().Play("HellTheme");
    }

    public void Quit(){
        Application.Quit();
    }
}
