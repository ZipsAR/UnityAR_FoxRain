using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class SoundSystem : Singleton<SoundSystem>
{
    [SerializeField]
    private GameObject audiosound;
    [SerializeField]    
    private AudioClip audioclip;

    private GameObject Createobj;

    // Start is called before the first frame update
    void Start(){
        Createobj = Instantiate(audiosound);
    }


    public void PlayAudio(Vector3 position)
    {
        AudioSource audio = Createobj.GetComponent<AudioSource>();
        audio.clip = audioclip;
        audio.time = 0.0635f;
        audio.volume = 1;
        audio.Play();  

      
    }
}
