using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = ZipsAR.Logger;
using Random = UnityEngine.Random;

public class InteractManager : MonoBehaviour
{
    private struct CmdDetail
    {
        public int cmdIdx;
        public Vector3 targetDir; // Position the pet will move to
        public GameObject targetObj; // Snack or Toy
    
        public CmdDetail(int cmdIdx, Vector3 targetDir, GameObject targetObj)
        {
            this.cmdIdx = cmdIdx;
            this.targetDir = targetDir;
            this.targetObj = targetObj;
        }
    }

    private struct StatChangeCriteria
    {
        public int fluctuatingValuePerTime;
        public int fluctuatingValuePerDistance;

        public float curTime;
        public float curDistance;
        
        public float unitTime;
        public float unitDistance;
        
        public StatChangeCriteria(
            int fluctuatingValuePerTime,
            int fluctuatingValuePerDistance,
            
            float curTime,
            float curDistance,
            
            float unitTime,
            float unitDistance)
        {
            this.fluctuatingValuePerTime = fluctuatingValuePerTime;
            this.fluctuatingValuePerDistance = fluctuatingValuePerDistance;

            this.curTime = curTime;
            this.curDistance = curDistance;
            
            this.unitTime = unitTime;
            this.unitDistance = unitDistance;
        }
    }

    public InteractData interactData;
    [SerializeField] private PetBase pet;

    private Queue<CmdDetail> cmdQueue;
    private CmdDetail nextCmd;
    
    // Interact check variable
    private Coroutine checkPetCollisionTimeCoroutine;
    private float collisionTimer;
    private bool isColliding;
    private float collisionTimeLimit;
    private PetParts curPetCollisionPart;
    private float interactionCoolTime;
    private bool isInteractionIgnored;

    // Brushing
    private float brushingTime;
    private float brushingTimeThreshold;
    private bool isBrushing;
    
    // Stat
    private Coroutine distanceCheckCoroutine;
    private Coroutine timeCheckCoroutine;
    // private float distanceCheckPeriod;
    private Vector3 prevPetPos;
    
    // Fullness
    private StatChangeCriteria fullnessCreteria;
    
    // Tiredness
    private StatChangeCriteria tirednessCreteria;

    // Cleanliness
    private StatChangeCriteria cleanlinessCreteria;




    private void Start()
    {
        pet.SetPetAnimationMode(PlayMode.InteractMode);
        cmdQueue = new Queue<CmdDetail>();

        // Interact check variable
        collisionTimer = 0;
        isColliding = false;
        collisionTimeLimit = 2f;
        curPetCollisionPart = PetParts.None;
        interactionCoolTime = 1f;
        isInteractionIgnored = false;
        
        // Brushing
        brushingTime = 0f;
        brushingTimeThreshold = 1f;
        isBrushing = false;
        
        // Stat
        prevPetPos = pet.gameObject.transform.position;

        // Fullness
        fullnessCreteria = new StatChangeCriteria(2, 5, 0f, 0f, 1f, 1f);
        
        // Tiredness
        tirednessCreteria = new StatChangeCriteria(1, 2, 0f, 0f, 1f, 1f);

        // Cleanliness
        cleanlinessCreteria = new StatChangeCriteria(1, 2, 0f, 0f, 1f, 1f);
        
        interactData.Init();

        SetInitialCmd();
    }

    private void Update()
    {
        if(cmdQueue.Count != 0) ShowCurQueue();
        
        // Do not run other commands if the pet is running a command
        if(pet.inProcess) return;
        
        if (DequeCmd(out nextCmd)) ExecuteCmd(nextCmd); // if queue is not empty, execute cmd
        else AddMatchingConditionCmd(); // if queue is empty, Add commands that meet the current conditions
        
        // Track stat
        StatUpdateByDistance();
        StatUpdateByTime();
        
        // Check the time player brushing pet
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

    public PetBase GetCurPet() => pet;

    #region TrackStat

        private void StatUpdateByDistance()
        {
            float distanceMoved = Vector3.Distance(pet.transform.position, prevPetPos);
            
            // Decreasing fullness 
            fullnessCreteria.curDistance += distanceMoved;
            if (fullnessCreteria.curDistance > fullnessCreteria.unitDistance)
            {
                fullnessCreteria.curDistance -= fullnessCreteria.unitDistance;
                pet.DecreaseStat(PetStatNames.Fullness, fullnessCreteria.fluctuatingValuePerDistance);
            }

            // Decreasing cleanliness
            cleanlinessCreteria.curDistance += distanceMoved;
            if (cleanlinessCreteria.curDistance > cleanlinessCreteria.unitDistance)
            {
                cleanlinessCreteria.curDistance -= cleanlinessCreteria.unitDistance;
                pet.DecreaseStat(PetStatNames.Cleanliness, cleanlinessCreteria.fluctuatingValuePerDistance);
            }
            
            // Increasing tiredness 
            tirednessCreteria.curDistance += distanceMoved;
            if (tirednessCreteria.curDistance > tirednessCreteria.unitDistance)
            {
                tirednessCreteria.curDistance -= tirednessCreteria.unitDistance;
                pet.IncreaseStat(PetStatNames.Tiredness, tirednessCreteria.fluctuatingValuePerDistance);
            }
            
            // Save the current pet's location to the previous location variable
            prevPetPos = pet.gameObject.transform.position;
        }
        
        private void StatUpdateByTime()
        {
            fullnessCreteria.curTime += Time.deltaTime;
            cleanlinessCreteria.curTime += Time.deltaTime;
            tirednessCreteria.curTime += Time.deltaTime;

            if (fullnessCreteria.curTime > fullnessCreteria.unitTime)
            {
                fullnessCreteria.curTime = 0f;
                pet.DecreaseStat(PetStatNames.Fullness, fullnessCreteria.fluctuatingValuePerTime);
            }
            
            if (cleanlinessCreteria.curTime > cleanlinessCreteria.unitTime)
            {
                cleanlinessCreteria.curTime = 0f;
                pet.DecreaseStat(PetStatNames.Cleanliness, cleanlinessCreteria.fluctuatingValuePerTime);
            }

            if (tirednessCreteria.curTime > tirednessCreteria.unitTime)
            {
                tirednessCreteria.curTime = 0f;
                pet.IncreaseStat(PetStatNames.Tiredness, tirednessCreteria.fluctuatingValuePerTime);
            }
        }

    #endregion

    
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

    
    #region Cmd

        private void SetInitialCmd()
        {
            // EnqueueCmd(Cmd.Move);
        }
        
        private void ExecuteCmd(CmdDetail cmdDetail)
        {
            switch (cmdDetail.cmdIdx)
            {
                case (int)Cmd.Move:
                    Vector3 nextCoord;
                    
                    // Move to a random location around the player if there is no designated location
                    if (nextCmd.targetDir == default)
                    {
                        Vector2 randomCoord = Random.insideUnitCircle * interactData.playerPetMaxDistance;
                        nextCoord = GameManager.Instance.player.gameObject.transform.position +
                                    new Vector3(randomCoord.x, transform.position.y, randomCoord.y);
                    }
                    else
                    {
                        nextCoord = nextCmd.targetDir;
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
                    pet.CmdEat(nextCmd.targetObj);
                    break;
                case (int)Cmd.Brush:
                    pet.CmdBrush();
                    break;
                case (int)Cmd.Bite:
                    Logger.Log("bite obj name : " + nextCmd.targetObj);
                    pet.CmdBite(nextCmd.targetObj);
                    break;
                case (int)Cmd.Spit:
                    pet.CmdSpit();
                    break;
                default:
                    throw new Exception("Unimplemented command");
            }
        }

        private void AddMatchingConditionCmd()
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
        
    #endregion
    
    #region ManageCmdQueue

        private void EnqueueCmd(Cmd cmd, Vector3 pos = default, GameObject targetObj = default)
        {
            if (cmd == Cmd.Eat && targetObj == default) throw new Exception("eat cmd must include targetObj");
            if (cmd == Cmd.Bite && targetObj == default) throw new Exception("bite cmd must include targetObj");
            
            cmdQueue.Enqueue(new CmdDetail((int)cmd, pos, targetObj));
        }
    
        /// <summary>
        /// Get top of cmd
        /// </summary>
        /// <param name="result">dequeue command value</param>
        /// <returns>Whether deqeue is successful</returns>
        private bool DequeCmd(out CmdDetail result)
        {
            // if cmdQueue is empty
            if (cmdQueue.Count == 0)
            {
                // Meaningless data
                result = new CmdDetail((int)Cmd.Move, Vector3.zero, null);
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

        private void ShowCurQueue()
        {
            string str = "";
            foreach (CmdDetail val in cmdQueue)
            {
                str += $"{val}\n";
            }

            str += "\n";
            Logger.Log(str);
        }
        
    #endregion
    
    
    #region InteractWithPet

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
        
        private void InteractWithHead()
        {
            pet.InteractHead();
            
            // Stat
            pet.DecreaseStat(PetStatNames.Tiredness, 5);
            pet.IncreaseStat(PetStatNames.Exp, 5);
            
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
            
            // Stat
            pet.DecreaseStat(PetStatNames.Tiredness, 7);
            pet.IncreaseStat(PetStatNames.Exp, 7);
            
            Logger.Log("interact jaw in interactManager");
        }
        
        private void InteractWithHandDetection()
        {
            pet.InteractHandDetection();
            
            // Stat
            pet.DecreaseStat(PetStatNames.Tiredness, 10);
            pet.IncreaseStat(PetStatNames.Exp, 10);

            Logger.Log("interact HandDetection in interactManager");
        }

    #endregion

    #region InteractInfoFromHand

        public void PetPartCollisionEnter(PetParts petPart)
        {
            if(isColliding) return;
            if (isInteractionIgnored)
            {
                Logger.Log("interaction is ignored, please wait for a while");
                return;
            }
            
            isColliding = true;
            curPetCollisionPart = petPart;

            if (petPart == PetParts.HandDetection)
            {
                checkPetCollisionTimeCoroutine = StartCoroutine(CheckPetCollisionTime());
            }
            else
            {
                CallInteractEvent(petPart);
                
                StartCoroutine(IgnoreInteractionForSeconds(interactionCoolTime));
                ResetCollisionInfo();
            }
        }

        private IEnumerator CheckPetCollisionTime()
        {
            while (true)
            {
                collisionTimer += Time.deltaTime;

                if (collisionTimer > collisionTimeLimit)
                {
                    CallInteractEvent(curPetCollisionPart);

                    StartCoroutine(IgnoreInteractionForSeconds(interactionCoolTime));
                    ResetCollisionInfo();
                    break;
                }
                
                yield return null;
            }
        }

        private IEnumerator IgnoreInteractionForSeconds(float coolTime)
        {
            isInteractionIgnored = true;
            Logger.Log("ignore interaction start");

            while (coolTime > 0)
            {
                coolTime -= Time.deltaTime;
                yield return null;
            }

            isInteractionIgnored = false;
            Logger.Log("ignore interaction end");
        }

        private void CallInteractEvent(PetParts petPart)
        {
            switch (petPart)
            {
                case PetParts.None:
                    break;
                case PetParts.Head:
                    InteractWithHead();
                    break;
                case PetParts.Jaw:
                    InteractWithJaw();
                    break;
                case PetParts.Body:
                    InteractWithBody();
                    break;
                case PetParts.HandDetection:
                    InteractWithHandDetection();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(petPart), petPart, null);
            }

            ResetCollisionInfo();
        }

        public void PetPartCollisionExit(PetParts petPart)
        {
            if(!isColliding) return;
            if(curPetCollisionPart != petPart) return;
            
            StopCoroutine(checkPetCollisionTimeCoroutine);
            ResetCollisionInfo();
        }

        private void ResetCollisionInfo()
        {
            collisionTimer = 0;
            isColliding = false;
        }

    #endregion
    
    private void OnDestroy()
    {
        StopCoroutine(distanceCheckCoroutine);
        StopCoroutine(timeCheckCoroutine);
    }
}
