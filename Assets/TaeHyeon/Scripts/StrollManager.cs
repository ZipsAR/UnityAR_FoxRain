using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = ZipsAR.Logger;

// create : stroll mode on enter
// destroy : stroll mode on exit
public class StrollManager : Singleton<StrollManager>
{
    [SerializeField] private StrollData strollData;
    [SerializeField] private PetBase pet;

    private void Start()
    {
        strollData.Init();
        pet.SetPetAnimationMode(PlayMode.StrollMode);
    }

    private void Update()
    {
        strollData.strollTime += Time.deltaTime;
    }
}
