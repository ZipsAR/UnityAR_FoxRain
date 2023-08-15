using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = ZipsAR.Logger;


// Set AudioSource volume to 0.5
public class InteractAudioManager : MonoBehaviour
{
    private AudioSource petAudioSource;
    [SerializeField] private AudioSource effectSource;
    
    private void Awake()
    {
        InteractEventManager.OnPetInitializedToAll -= OnPetInitialized;
        InteractEventManager.OnPetInitializedToAll += OnPetInitialized;
    }
    
    private void OnDisable()
    {
        InteractEventManager.OnPetInitializedToAll -= OnPetInitialized;
    }
    private void OnPetInitialized(object sender, PetArgs e)
    {
        petAudioSource = e.petObj.GetComponent<AudioSource>();
        petAudioSource.playOnAwake = false;
        Logger.Log("petAudioSource init clear");
    }
    
    public void PlayPetSound(AudioClip clip)
    {
        petAudioSource.clip = clip;
        petAudioSource.Play();
    }

    public void PlayEffectSound(AudioClip clip)
    {
        effectSource.clip = clip;
        effectSource.Play();
    }
}
