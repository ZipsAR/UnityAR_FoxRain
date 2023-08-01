using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Logger = ZipsAR.Logger;
using Random = UnityEngine.Random;

// create : interact mode on enter
// destroy : interact mode on exit
public class InteractManager : MonoBehaviour
{
    public InteractData interactData;
    [SerializeField] private PetBase pet;

    public Action interactHeadEvent;
    public Action interactJawEvent;
    public Action interactBodyEvent;
    public Action interactHandDetectionEvent;
    
    
    
    
    private void Start()
    {
        pet.SetPetAnimationMode(PlayMode.InteractMode);
        
        interactData.Init();

        // Register interaction action by pet part
        interactHeadEvent -= InteractWithHead;
        interactHeadEvent += InteractWithHead;

        interactBodyEvent -= InteractWithBody;
        interactBodyEvent += InteractWithBody;

        interactJawEvent -= InteractWithJaw;
        interactJawEvent += InteractWithJaw;

        interactHandDetectionEvent -= InteractWithHandDetection;
        interactHandDetectionEvent += InteractWithHandDetection;
    }

    private void Update()
    {
        // Cannot run another cmd if the pet is running some action
        if (pet.inProcess)
        {
            return;
        }
        
        // Run if the user does not move for a certain amount of time
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
                // In the current code, Petstates cannot be idle, it is either "walk" or "sit
                // So this part is not executed
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

/// <summary>
/// 1. Snack script notifies interactManager that the snack has dropped
/// 2. interactManager triggers an event to PetBase 
/// </summary>
/// <param name="snackPos">Dropped snack position</param>
    public void NotifySnackDrop(Vector3 snackPos)
    {
        StartCoroutine(NotifySnackDropSequence(snackPos));
        Logger.Log("notify pet to snack is dropped");
    }

    private IEnumerator NotifySnackDropSequence(Vector3 snackPos)
    {
        // waiting for current command end
        while (pet.inProcess)
        {
            Logger.Log("waiting for current command end");
            yield return null;
        }
        
        // Stop All command 
        pet.AbortAllCmd();
        
        // Move to snack position
        pet.CmdMoveTo(snackPos);
    }
    
    
    private void InteractWithHead()
    {
        pet.InteractHead();
        Logger.Log("interact head in interactManager");
    }
    
    private void InteractWithJaw()
    {
        pet.InteractJaw();
        Logger.Log("interact body in interactManager");
    }
    
    private void InteractWithBody()
    {
        pet.InteractBody();
        Logger.Log("interact jaw in interactManager");
    }
    
    private void InteractWithHandDetection()
    {
        pet.InteractHandDetection();
        Logger.Log("interact HandDetection in interactManager");
    }
}
