using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Audio;
using UnityEngine;


public class AudioManager : MonoBehaviour
{

    public Sound[] sounds;

    public static AudioManager instance;

    [HideInInspector]
    public String CurrentSong = ""; // holds the name of the song that is currently playing

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
                StopPlaying(CurrentSong);
            }
            CurrentSong = s.name;
        }

        s.source.Play();
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


    void Start()
    {
        Play("HellTheme");
    }

    // to play a sound from anywhere, call "FindObjectOfType<AudioManager>().Play(name);"

}
