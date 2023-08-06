using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXCon : MonoBehaviour
{
    public AudioSource audiosc;
    public AudioClip audioclip;
    public void Start()
    {
        audiosc = GetComponent<AudioSource>();
        audiosc.clip = audioclip;
    }
    public void GetButton()
    {
        audiosc.PlayOneShot(audioclip);
    }
}
