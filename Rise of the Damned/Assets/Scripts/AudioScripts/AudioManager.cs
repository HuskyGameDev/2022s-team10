using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.UI;


public class AudioManager : MonoBehaviour
{

    [SerializeField] AudioMixer mixer;

    public Sound[] sounds;

    public static AudioManager instance;

    [HideInInspector]
    public String CurrentSong = ""; // holds the name of the song that is currently playing

    private Coroutine switchMusic;

    private float musicFactor;
    private float sfxFactor;
    private float masterFactor;

    void Update(){
        //Debug.Log(volumeFactor);
        // Instead of calling every frame, only call when changing scene and ???
        mixer.SetFloat("MusicVolume", musicFactor);
        mixer.SetFloat("SFXVolume", sfxFactor);
        mixer.SetFloat("MasterVolume", masterFactor);
    }

    void Awake()
    {
        //if an audio manager already exists then get rid of the new one
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
            return;
        }
        //audio manager persists between scenes
        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;
            s.source.outputAudioMixerGroup = s.output;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void Play(string name)
    {

        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " was not found");
            return;
        }
        if (s.music) // if trying to play music stop the already playing music
        {
            if(CurrentSong != "") // if a song is playing
            {
                if (switchMusic != null) { StopCoroutine(switchMusic); }
                switchMusic = StartCoroutine(Fade(mixer, "MusicVolume", 2, 1 / 100, s));
            } else
            {
                s.source.Play();
                CurrentSong = s.name;
            }
        } else //if playing a sound effect
        {
            s.source.Play();
        }
    }

    public void StopPlaying (string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " was not found");
            return;
        }

        s.source.Stop();

    }

    IEnumerator Fade(AudioMixer audioMixer, string exposedParam, float duration, float targetVolume, Sound newSong)
    {
        //Fade out the current song
        float currentTime = 0;
        float currentVol;
        //mixer.GetFloat(exposedParam, out currentVol);
        currentVol = Mathf.Pow(10, musicFactor / 20);

        float targetValue2 = currentVol; //target for the fade in

        float targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
            audioMixer.SetFloat(exposedParam, Mathf.Log10(newVol) * 20);
            yield return null;
        }

        //Stop playing the current song after it had been faded

        StopPlaying(CurrentSong);

        //Start playing new song
        newSong.source.Play();
        CurrentSong = newSong.name;
        //Fade in the new song

        audioMixer.SetFloat(exposedParam, targetValue);


        yield break;
    }

    void Start()
    {
        mixer.SetFloat("MusicVolume", Mathf.Log10(PlayerPrefs.GetFloat("musicVol", 1.0f)*20));
        mixer.SetFloat("SFXVolume", Mathf.Log10(PlayerPrefs.GetFloat("sfxVol", 1.0f)*20));
        mixer.SetFloat("MasterVolume", Mathf.Log10(PlayerPrefs.GetFloat("masterVol", 1.0f)*20));
        Play("MainTheme");
    }


    // This part of the Volume Slider functionality currently sorta works.
    // Volume will update when slider value changes but not when scene is loaded.
    // i.e. If you load scene with volume slider at 10%, music still plays at 100% until you change slider value.
    // Code at the start of Awake() is trying to account for this :)
    // - Tyler
    public void ChangeMusicVol(float volumeVal){
        //Debug.Log("changing " + volumeVal);
        musicFactor = Mathf.Log10(volumeVal)*20;
    }

    public void ChangeSFXVol(float volumeVal){
        //Debug.Log("changing " + volumeVal);
        sfxFactor = Mathf.Log10(volumeVal)*20;
    }

    public void ChangeMasterVol(float volumeVal){
        //Debug.Log("changing " + volumeVal);
        masterFactor = Mathf.Log10(volumeVal)*20;
    }

    // to play a sound from anywhere, call "FindObjectOfType<AudioManager>().Play(name);"
}
