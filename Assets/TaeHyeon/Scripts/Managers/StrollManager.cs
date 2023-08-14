using System;
using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;
using UnityEngine.Serialization;
using Logger = ZipsAR.Logger;
using Random = UnityEngine.Random;

// create : stroll mode on enter
// destroy : stroll mode on exit
public class StrollManager : MonoBehaviour
{
    public StrollData strollData;
    [SerializeField] private PetBase pet;

    private void Start()
    {
        strollData.Init();
    }

    private void Update()
    {
        strollData.strollTime += Time.deltaTime;

        // Cannot run another cmd if the pet is running some action
        if (pet.inProcess)
        {
            return;
        }
        
        // Player condition
        if (GameManager.Instance.player.idleTime > strollData.playerIdleTimeThreshold && pet.petStates != PetStates.Sit)
        {
            
            // pet.CmdLook();
            return;
        }

        // Move to player area if the pet's position is further than strollData.maxDistance from the player
        if (Vector3.Distance(GameManager.Instance.player.gameObject.transform.position, pet.transform.position) >
            strollData.playerPetMaxDistance)
        {
            Vector2 randomCoord = Random.insideUnitCircle * strollData.playerPetMaxDistance;
            Vector3 nextCoord = GameManager.Instance.player.gameObject.transform.position +
                                new Vector3(randomCoord.x, transform.position.y, randomCoord.y);
            pet.CmdMoveTo(nextCoord);
            return;
        }

        // Pet condition
        switch (pet.petStates)
        {
            case PetStates.Idle:
                Vector2 randomCoord = Random.insideUnitCircle * strollData.playerPetMaxDistance;
                Vector3 nextCoord = GameManager.Instance.player.gameObject.transform.position +
                                    new Vector3(randomCoord.x, transform.position.y, randomCoord.y);
                pet.CmdMoveTo(nextCoord);
                break;
            case PetStates.Walk:
                break;
            case PetStates.Sit:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
}
