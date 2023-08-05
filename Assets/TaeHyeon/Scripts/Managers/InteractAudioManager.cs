using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = ZipsAR.Logger;


// Set AudioSource volume to 0.5
public class InteractAudioManager : MonoBehaviour
{
    private AudioSource petAudioSource;
    
    private IEnumerator Start()
    {
        yield return null;
        petAudioSource = GameManager.Instance.interactManager.GetCurPet().gameObject.GetComponent<AudioSource>();
        petAudioSource.playOnAwake = false;
        Logger.Log("petAudioSource init clear");
    }

    public void PlayPetSound(AudioClip clip)
    {
        petAudioSource.clip = clip;
        petAudioSource.Play();
    }
}
