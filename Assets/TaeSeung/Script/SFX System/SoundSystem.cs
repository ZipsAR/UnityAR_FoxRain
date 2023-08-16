using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class SoundSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject audiosound;
    [SerializeField]    
    private AudioClip audioclip;
    private GameObject Createobj;

    public static SoundSystem Instance;

    // Start is called before the first frame update
    void Start(){
        if (SoundSystem.Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }


    public void PlayAudio(Vector3 position, AudioClip Audio, int volume, float starttime)
    {
        if (!Createobj) Createobj = Instantiate(audiosound);
        AudioSource audio = Createobj.GetComponent<AudioSource>();
        audio.clip = Audio;
        audio.time = starttime;
        audio.volume = volume;
        audio.Play();
    }


    [Obsolete]
    public void PlayAudio(Vector3 position)
    {
        if(!Createobj) Createobj = Instantiate(audiosound);

        AudioSource audio = Createobj.GetComponent<AudioSource>();
        audio.clip = audioclip;
        audio.time = 0.0635f;
        audio.volume = 1;
        audio.Play();  
    }



}
