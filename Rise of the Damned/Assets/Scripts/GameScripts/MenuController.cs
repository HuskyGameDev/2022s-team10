using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void GameOverRetry(){
        Time.timeScale = 1;
        PlayerController.isPaused = false;
        SceneManager.LoadScene("LevelOne");
    }

    public void GameOverMenu(){
        Time.timeScale = 1;
        PlayerController.isPaused = false;
        // Done broken vv
        FindObjectOfType<AudioManager>().Play("MainTheme");
        SceneManager.LoadScene("MainMenu");
    }

    public void MainMenuPlay(){
        Time.timeScale = 1;
        PlayerController.isPaused = false;
        SceneManager.LoadScene("LevelOne");
    }

    public void MainMenuQuit(){
        Application.Quit();
    }

}
