using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Logger = ZipsAR.Logger;
using Random = UnityEngine.Random;

// create : stroll mode on enter
// destroy : stroll mode on exit
public class StrollManager : Singleton<StrollManager>
{
    public StrollData strollData;
    [SerializeField] private PetBase pet;
    
    private void Start()
    {
        strollData.Init();
        pet.SetPetAnimationMode(PlayMode.StrollMode);
    }

    private void Update()
    {
        strollData.strollTime += Time.deltaTime;
        if (pet.PetStates == PetStates.Idle)
        {
            Vector2 randomCoord = Random.insideUnitCircle * strollData.playerPetMaxDistance;
            Vector3 nextCoord = GameManager.Instance.Player.gameObject.transform.position +
                                new Vector3(randomCoord.x, transform.position.y, randomCoord.y);
            pet.MoveTo(nextCoord);
            Logger.Log("nextCoord : " + nextCoord);
        }
    }
    
}
