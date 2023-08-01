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

    private Queue<Tuple<int, Vector3, GameObject>> cmdQueue;
    private Tuple<int, Vector3, GameObject> nextCmd;

    // Brushing
    private float brushingTime;
    private float brushingTimeThreshold;
    private bool isBrushing;
    
    private void Start()
    {
        pet.SetPetAnimationMode(PlayMode.InteractMode);
        cmdQueue = new Queue<Tuple<int, Vector3, GameObject>>();

        brushingTime = 0f;
        brushingTimeThreshold = 1f;
        isBrushing = false;
        
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
        if(cmdQueue.Count != 0) ShowCurQueue();
        if(pet.inProcess) return;

        // if queue is not empty
        if (DequeCmd(out nextCmd))
        {
            switch (nextCmd.Item1)
            {
                case (int)Cmd.Move:
                    Vector3 nextCoord;
                    if (nextCmd.Item2 == default)
                    {
                        Vector2 randomCoord = Random.insideUnitCircle * interactData.playerPetMaxDistance;
                        nextCoord = GameManager.Instance.player.gameObject.transform.position +
                                            new Vector3(randomCoord.x, transform.position.y, randomCoord.y);
                    }
                    else
                    {
                        nextCoord = nextCmd.Item2;
                    }

                    pet.CmdMoveTo(nextCoord);
                    
                    break;
                
                
                case (int)Cmd.Look:
                    pet.CmdLookPlayer();
                    break;
                case (int)Cmd.Sit:
                    pet.CmdSit();
                    break;
                case (int)Cmd.Eat:
                    pet.CmdEat(nextCmd.Item3);
                    break;
                case (int)Cmd.Brush:
                    pet.CmdBrush();
                    break;
                case (int)Cmd.Bite:
                    Logger.Log("bite obj name : " + nextCmd.Item3);
                    pet.CmdBite(nextCmd.Item3);
                    break;
                case (int)Cmd.Spit:
                    pet.CmdSpit();
                    break;
                default:
                    throw new Exception("Unimplemented command");
                
            }
        }
        // if queue is empty
        else
        {
            // if player stay for a while
            if (GameManager.Instance.player.idleTime > interactData.playerIdleTimeThreshold && pet.petStates != PetStates.Sit)
            {
                EnqueueCmd(Cmd.Look);
                EnqueueCmd(Cmd.Sit);
            }
            // if player-pet distance increase
            else if(Vector3.Distance(GameManager.Instance.player.gameObject.transform.position, pet.transform.position) >
                    interactData.playerPetMaxDistance)
            {
                EnqueueCmd(Cmd.Move);
            }
        }

        if (isBrushing)
        {
            brushingTime += Time.deltaTime;
            if (brushingTime > brushingTimeThreshold)
            {
                ClearCmdQueue();
                EnqueueCmd(Cmd.Brush);
                isBrushing = false;
                brushingTime = 0f;
            }
        }
    }

    private void ShowCurQueue()
    {
        string str = "";
        foreach (Tuple<int, Vector3, GameObject> val in cmdQueue)
        {
            str += $"{val}\n";
        }

        str += "\n";
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
/// <param name="snackTransform">Dropped snack position</param>
    public void NotifySnackDrop(Transform snackTransform)
    {
        StartCoroutine(NotifySnackDropSequence(snackTransform));
        Logger.Log("notify pet to snack is dropped");
    }

    private IEnumerator NotifySnackDropSequence(Transform snackTransform)
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
        // pet.CmdMoveTo(snackPos);
        ClearCmdQueue();
        EnqueueCmd(Cmd.Move, snackTransform.position);
        EnqueueCmd(Cmd.Look);
        EnqueueCmd(Cmd.Sit);
        EnqueueCmd(Cmd.Eat, targetObj: snackTransform.gameObject);
    }
    
    #endregion

    
    
    
    #region Toy
    
    /// <summary>
    /// 1. Toy script notifies interactManager that the toy has dropped
    /// 2. interactManager triggers an event to PetBase 
    /// </summary>
    /// <param name="toyTransform">Dropped toy position</param>
    public void NotifyToyDrop(Transform toyTransform)
    {
        StartCoroutine(NotifyToyDropSequence(toyTransform));
        Logger.Log("notify pet to toy is dropped");
    }

    private IEnumerator NotifyToyDropSequence(Transform toyTransform)
    {
        // waiting for current command end
        while (pet.inProcess)
        {
            Logger.Log("waiting for current command end");
            yield return null;
        }
        
        // Stop All command 
        pet.AbortAllCmd();
        
        // Move to toy position
        ClearCmdQueue();
        EnqueueCmd(Cmd.Move, toyTransform.position);
        EnqueueCmd(Cmd.Look);
        EnqueueCmd(Cmd.Bite, targetObj: toyTransform.gameObject);
        EnqueueCmd(Cmd.Move, pos: GameManager.Instance.player.gameObject.transform.position);
        EnqueueCmd(Cmd.Spit);
    }
    
    #endregion

    
    
    
    public void EnqueueCmd(Cmd cmd, Vector3 pos = default, GameObject targetObj = default)
    {
        if (cmd == Cmd.Eat && targetObj == default) throw new Exception("eat cmd must include targetObj");
        if (cmd == Cmd.Bite && targetObj == default) throw new Exception("bite cmd must include targetObj");
        
        cmdQueue.Enqueue(new Tuple<int, Vector3, GameObject>((int)cmd, pos, targetObj));
    }

    /// <summary>
    /// Get top of cmd
    /// </summary>
    /// <param name="result">dequeue command value</param>
    /// <returns>Whether deqeue is successful</returns>
    private bool DequeCmd(out Tuple<int, Vector3, GameObject> result)
    {
        // if cmdQueue is empty
        if (cmdQueue.Count == 0)
        {
            // Meaningless data
            result = new Tuple<int, Vector3, GameObject>((int)Cmd.Move, Vector3.zero, null);
            return false;
        }

        result = cmdQueue.Dequeue();
        return true;
    }

    public void ClearCmdQueue()
    {
        cmdQueue.Clear();
        Logger.Log("cmdQueue clear");
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

    /// <summary>
    /// Called when the comb touches or falls on the pet
    /// </summary>
    /// <param name="collisionState">true : onTriggerEnter, false : onTriggerExit</param>
    public void CombCollision(bool collisionState)
    {
        isBrushing = collisionState;

        if (!collisionState)
        {
            isBrushing = false;
            brushingTime = 0f;
        }
    }
}
