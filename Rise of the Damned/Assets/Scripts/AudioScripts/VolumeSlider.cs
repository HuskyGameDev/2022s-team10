using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Slider vSlider;

    void Start(){
        //am = GetComponent<AudioManager>();
        vSlider.value = PlayerPrefs.GetFloat("volume", 0.3f);
    }

    public void SetVolume(float sliderValue){
        //PlayerPrefs.SetFloat("MusicVolume", sliderValue);
        FindObjectOfType<AudioManager>().ChangeVolume(sliderValue);
        PlayerPrefs.SetFloat("volume", sliderValue);
        Debug.Log("set to " + PlayerPrefs.GetFloat("volume"));
        //am.ChangeVolume(sliderValue);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
