using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    Resolution[] resolutions;
    public Dropdown resolutionDropdown;

    void Start(){
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        int currentResolutionIndex = 0;
        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; i++){
            string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRate + "hz";
            options.Add(option);
            if (resolutions[i].width  == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height ){
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);

        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex){
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void ToggleFullscreen(bool isFullscreen){
        Screen.fullScreen = isFullscreen;
    }

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
