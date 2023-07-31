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

    private Queue<int> cmdQueue;
    private Cmd nextCmd;
    
    private void Start()
    {
        pet.SetPetAnimationMode(PlayMode.InteractMode);
        cmdQueue = new Queue<int>();
        
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
        
        // Set initial Cmd
        EnqueueCmd(Cmd.Move);
    }

    private void Update()
    {
        ShowCurQueue();
        if(pet.inProcess) return;
        
        // if Queue is empty
        if (cmdQueue.Count == 0)
        {
            if (GameManager.Instance.player.idleTime > interactData.playerIdleTimeThreshold && pet.petStates != PetStates.Sit)
            {
                EnqueueCmd(Cmd.Look);
                EnqueueCmd(Cmd.Sit);
            }
            else
            {
                EnqueueCmd(Cmd.Move);
            }
            return;
        }

        // if queue is not empty
        if (DequeCmd(out nextCmd))
        {
            switch (nextCmd)
            {
                case Cmd.Move:
                    Vector2 randomCoord = Random.insideUnitCircle * interactData.playerPetMaxDistance;
                    Vector3 nextCoord = GameManager.Instance.player.gameObject.transform.position +
                                        new Vector3(randomCoord.x, transform.position.y, randomCoord.y);
                    pet.CmdMoveTo(nextCoord);
                    break;
                case Cmd.Look:
                    pet.CmdLookPlayer();
                    break;
                case Cmd.Sit:
                    pet.CmdSit();
                    break;
            }
        }
    }

    private void ShowCurQueue()
    {
        string str = "";
        foreach (int val in cmdQueue)
        {
            str += $"{val} / ";
        }
        Logger.Log(str);
    }

    // private void Update()
    // {
    //     // Cannot run another cmd if the pet is running some action
    //     if (pet.inProcess)
    //     {
    //         return;
    //     }
    //     
    //     // Run if the user does not move for a certain amount of time
    //     if (GameManager.Instance.player.idleTime > interactData.playerIdleTimeThreshold && pet.petStates != PetStates.Sit)
    //     {
    //         pet.CmdLookPlayer();
    //         return;
    //     }
    //
    //     // Move to player area if the pet's position is further than strollData.maxDistance from the player
    //     if (Vector3.Distance(GameManager.Instance.player.gameObject.transform.position, pet.transform.position) >
    //         interactData.playerPetMaxDistance)
    //     {
    //         Vector2 randomCoord = Random.insideUnitCircle * interactData.playerPetMaxDistance;
    //         Vector3 nextCoord = GameManager.Instance.player.gameObject.transform.position +
    //                             new Vector3(randomCoord.x, transform.position.y, randomCoord.y);
    //         pet.CmdMoveTo(nextCoord);
    //         return;
    //     }
    //
    //     
    //     
    //     
    //     
    //     // Pet condition
    //     switch (pet.petStates)
    //     {
    //         case PetStates.Idle:
    //             // In the current code, Petstates cannot be idle, it is either "walk" or "sit
    //             // So this part is not executed
    //             Vector2 randomCoord = Random.insideUnitCircle * interactData.playerPetMaxDistance;
    //             Vector3 nextCoord = GameManager.Instance.player.gameObject.transform.position +
    //                                 new Vector3(randomCoord.x, transform.position.y, randomCoord.y);
    //             pet.CmdMoveTo(nextCoord);
    //             break;
    //         case PetStates.Walk:
    //             break;
    //         case PetStates.Sit:
    //             break;
    //         default:
    //             throw new ArgumentOutOfRangeException();
    //     }
    // }

    #region Snack
    
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
    
    #endregion

    private void EnqueueCmd(Cmd cmd)
    {
        cmdQueue.Enqueue((int)cmd);
    }

    /// <summary>
    /// Get top of cmd
    /// </summary>
    /// <param name="result">dequeue command value</param>
    /// <returns>Whether deqeue is successful</returns>
    private bool DequeCmd(out Cmd result)
    {
        // if cmdQueue is empty
        if (cmdQueue.Count == 0)
        {
            // Meaningless data
            result = Cmd.Move;
            return false;
        }

        result = (Cmd)cmdQueue.Dequeue();
        return true;
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
