using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slider : MonoBehaviour
{
    void Start(){
        //am = GetComponent<AudioManager>();
    }

    public void SetVolume(float sliderValue){
        //PlayerPrefs.SetFloat("MusicVolume", sliderValue);
        FindObjectOfType<AudioManager>().ChangeVolume(sliderValue);
        //am.ChangeVolume(sliderValue);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
