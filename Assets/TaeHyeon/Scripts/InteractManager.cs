using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Logger = ZipsAR.Logger;
using Random = UnityEngine.Random;

// create : interact mode on enter
// destroy : interact mode on exit
public class InteractManager : Singleton<InteractManager>
{
    public InteractData interactData;
    [SerializeField] private PetBase pet;

    private void Start()
    {
        interactData.Init();
    }

    private void Update()
    {
        // Cannot run another cmd if the pet is running some action
        if (pet.inprogress)
        {
            return;
        }
        
        // Player condition
        if (GameManager.Instance.player.idleTime > interactData.playerIdleTimeThreshold && pet.petStates != PetStates.Sit)
        {
            pet.CmdLookPlayer();
            return;
        }

        // Move to player area if the pet's position is further than strollData.maxDistance from the player
        if (Vector3.Distance(GameManager.Instance.player.gameObject.transform.position, pet.transform.position) >
            interactData.playerPetMaxDistance)
        {
            Vector2 randomCoord = Random.insideUnitCircle * interactData.playerPetMaxDistance;
            Vector3 nextCoord = GameManager.Instance.player.gameObject.transform.position +
                                new Vector3(randomCoord.x, transform.position.y, randomCoord.y);
            pet.CmdMoveTo(nextCoord);
            return;
        }

        // Pet condition
        switch (pet.petStates)
        {
            case PetStates.Idle:
                Vector2 randomCoord = Random.insideUnitCircle * interactData.playerPetMaxDistance;
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