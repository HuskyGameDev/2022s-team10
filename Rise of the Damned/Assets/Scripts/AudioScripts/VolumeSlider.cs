using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider masterSlider;

    void Awake(){
        Debug.Log("started?");
        musicSlider.value = PlayerPrefs.GetFloat("musicVol", 1.0f);
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVol", 1.0f);
        masterSlider.value = PlayerPrefs.GetFloat("masterVol", 1.0f);
    }

    public void SetMusic(float sliderValue){
        FindObjectOfType<AudioManager>().ChangeMusicVol(sliderValue);
        PlayerPrefs.SetFloat("musicVol", sliderValue);
        //Debug.Log("set to " + PlayerPrefs.GetFloat("musicVol"));
    }

    public void SetSFX(float sliderValue){
        FindObjectOfType<AudioManager>().ChangeSFXVol(sliderValue);
        PlayerPrefs.SetFloat("sfxVol", sliderValue);
        //Debug.Log("set to " + PlayerPrefs.GetFloat("sfxVol"));
    }

    public void SetMaster(float sliderValue){
        FindObjectOfType<AudioManager>().ChangeMasterVol(sliderValue);
        PlayerPrefs.SetFloat("masterVol", sliderValue);
        //Debug.Log("set to " + PlayerPrefs.GetFloat("masterVol"));
    }
}
