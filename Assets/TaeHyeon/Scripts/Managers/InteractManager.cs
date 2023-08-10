using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = ZipsAR.Logger;
using Random = UnityEngine.Random;

public class InteractManager : MonoBehaviour
{

    private struct StatChangeCriteria
    {
        // Stat values that vary every unitTime
        public int fluctuatingValuePerTime; 
        // Stat value that changes each time the pet moves a certain distance
        public int fluctuatingValuePerDistance;
        
        // Time elapsed and distance traveled since stat value was updated
        public float curTime;
        public float curDistance;
        
        // Stat changes at this time and distance
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

    private PetBase pet;
    private bool isPetInitialized;
    [SerializeField] private InteractData interactData;
    [SerializeField] private Transform selectedPetSpawnTransform;
    
    private Queue<CmdDetail> cmdQueue;
    private CmdDetail nextCmd;

    // Stat
    private Vector3 prevPetPos;
    private StatChangeCriteria fullnessCreteria;
    private StatChangeCriteria tirednessCreteria;
    private StatChangeCriteria cleanlinessCreteria;
    
    private void Awake()
    {
        InteractEventManager.OnPetSelected -= OnPetSelected;
        InteractEventManager.OnPetSelected += OnPetSelected;
        InteractEventManager.OnPetInitializedToManager -= OnPetInitializedToManager;
        InteractEventManager.OnPetInitializedToManager += OnPetInitializedToManager; 
        isPetInitialized = false;
        
        cmdQueue = new Queue<CmdDetail>();

        fullnessCreteria = new StatChangeCriteria(2, 3, 0f, 0f, 3f, 2f);
        tirednessCreteria = new StatChangeCriteria(1, 2, 0f, 0f, 3f, 2f);
        cleanlinessCreteria = new StatChangeCriteria(1, 2, 0f, 0f, 3f, 2f);
        
        InitializeInteractData();
        
        SetInitialCmd();
    }

    private void OnDisable()
    {
        InteractEventManager.OnPetSelected -= OnPetSelected;
        InteractEventManager.OnPetInitializedToManager -= OnPetInitializedToManager;

        SaveStat();
    }

    private void Update()
    {
        if(!isPetInitialized) return;
        
        if(cmdQueue.Count != 0) CmdQueueManager.ShowCurQueue(cmdQueue);
        
        // Do not run other commands if the pet is running a command
        if(pet.inProcess) return;
        
        if (CmdQueueManager.DequeCmd(cmdQueue, out nextCmd)) CmdQueueManager.ExecuteCmd(pet, nextCmd); // if queue is not empty, execute cmd
        else AddMatchingConditionCmd(); // if queue is empty, Add commands that meet the current conditions
        
        // Track stat
        StatUpdateByDistance();
        StatUpdateByTime();
        
        // Check the time player brushing pet
        if (interactData.isBrushing)
        {
            interactData.brushingTime += Time.deltaTime;
            if (interactData.brushingTime > interactData.brushingTimeThreshold)
            {
                CmdQueueManager.ClearCmdQueue(cmdQueue);
                CmdQueueManager.EnqueueCmd(cmdQueue, Cmd.Brush);
                interactData.isBrushing = false;
                interactData.brushingTime = 0f;
            }
        }
    }
    
    
    private void OnPetSelected(object sender, PetArgs e)
    {
        Instantiate(e.petObj, selectedPetSpawnTransform.position, selectedPetSpawnTransform.rotation);
    }
    
    private void OnPetInitializedToManager(object sender, PetArgs e)
    {
        pet = e.petObj.GetComponent<PetBase>();
        
        // File exist
        if (FileIOSystem.Instance.IsFileExist(FileIOSystem.StatFilename))
        {
            PetStatBase localStat = LoadStat((int)GameManager.Instance.curPetType);
            
            // File exists but no stat information corresponding to the current pet
            if (CheckStatIsNull(localStat))
            {
                pet.InitializeStatByDefault();
                SaveStat();
                Logger.Log("file exist, but no this pet stat");   
            }
            // Saved file has current pet information
            else
            {
                pet.SetPetStatBase(localStat);
                Logger.Log("Load Saved stat file");    
            }
        }
        // No File
        else
        {
            pet.InitializeStatByDefault();
            SetEmptyListToDatabase();
            SaveStat();
            Logger.Log("there is no saved stat file");
        }
        
        pet.SetPetAnimationMode(PlayMode.InteractMode);
     
        // Stat for distance
        prevPetPos = pet.gameObject.transform.position;
        
        InteractEventManager.NotifyPetInitializedToAll(pet.gameObject);

        isPetInitialized = true;
    }
    
    private void InitializeInteractData()
    {
        // interactData.floorHeight = -0.595f;
        
        interactData.playerPetMaxDistance = 2f;
        interactData.playerIdleTimeThreshold = 3f;
        
        interactData.bitingDistance = 0.1f;
        // interactData.playerFrontDistance = 0.5f;

        interactData.isBrushing = false;
        interactData.brushingTime = 0f;
        interactData.brushingTimeThreshold = 1f;

        interactData.isColliding = false;
        interactData.collisionTimer = 0f;
        interactData.collisionTimeLimit = 2f;
        interactData.curPetCollisionPart = PetParts.None;
        interactData.checkPetCollisionTimeCoroutine = null;

        interactData.isInteractionIgnored = false;
        interactData.interactionCoolTime = 1f;
    }
    
    public PetBase GetCurPet() => pet;

    public InteractData GetInteractData() => interactData;
    
    #region Stat
    
        private bool CheckStatIsNull(PetStatBase localStat)
        {
            if (localStat.fullness == 0
                && localStat.tiredness == 0
                && localStat.cleanliness == 0
                && localStat.exp == 0
                && localStat.level == 1
               )
            {
                return true;
            }

            return false;
        }
    
        private void SetEmptyListToDatabase()
        {
            List<PetStatBase> emptyList = new List<PetStatBase>();

            for (int i = 0; i < Enum.GetValues(typeof(PetType)).Length; i++)
            {
                emptyList.Add(new PetStatBase());
            }

            FileIOSystem.Instance.statdatabase.savedStats = emptyList;
        }

        private void SaveStat()
        {
            PetType curPetType = GameManager.Instance.curPetType;
            
            List<PetStatBase> existingStatList = FileIOSystem.Instance.statdatabase.savedStats;
            existingStatList[(int)curPetType] =  pet.GetStat();
            FileIOSystem.Instance.statdatabase.savedStats = existingStatList;
            FileIOSystem.Instance.Save(FileIOSystem.Instance.statdatabase, FileIOSystem.StatFilename);
        }

        private PetStatBase LoadStat(int idx)
        {
            FileIOSystem.Instance.Load(FileIOSystem.Instance.statdatabase, FileIOSystem.StatFilename);
            return FileIOSystem.Instance.statdatabase.savedStats[idx];
        }
    
        private void StatUpdateByDistance()
        {
            float distanceMoved = Vector3.Distance(pet.transform.position, prevPetPos);
            
            // Decreasing fullness 
            fullnessCreteria.curDistance += distanceMoved;
            if (fullnessCreteria.curDistance > fullnessCreteria.unitDistance)
            {
                fullnessCreteria.curDistance -= fullnessCreteria.unitDistance;
                pet.UpdateStat(PetStatNames.Fullness, -fullnessCreteria.fluctuatingValuePerDistance);
            }

            // Decreasing cleanliness
            cleanlinessCreteria.curDistance += distanceMoved;
            if (cleanlinessCreteria.curDistance > cleanlinessCreteria.unitDistance)
            {
                cleanlinessCreteria.curDistance -= cleanlinessCreteria.unitDistance;
                pet.UpdateStat(PetStatNames.Cleanliness, -cleanlinessCreteria.fluctuatingValuePerDistance);
            }
            
            // Increasing tiredness 
            tirednessCreteria.curDistance += distanceMoved;
            if (tirednessCreteria.curDistance > tirednessCreteria.unitDistance)
            {
                tirednessCreteria.curDistance -= tirednessCreteria.unitDistance;
                pet.UpdateStat(PetStatNames.Tiredness, tirednessCreteria.fluctuatingValuePerDistance);
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
                pet.UpdateStat(PetStatNames.Fullness, -fullnessCreteria.fluctuatingValuePerTime);
            }
            
            if (cleanlinessCreteria.curTime > cleanlinessCreteria.unitTime)
            {
                cleanlinessCreteria.curTime = 0f;
                pet.UpdateStat(PetStatNames.Cleanliness, -cleanlinessCreteria.fluctuatingValuePerTime);
            }

            if (tirednessCreteria.curTime > tirednessCreteria.unitTime)
            {
                tirednessCreteria.curTime = 0f;
                pet.UpdateStat(PetStatNames.Tiredness, tirednessCreteria.fluctuatingValuePerTime);
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
            CmdQueueManager.ClearCmdQueue(cmdQueue);
            CmdQueueManager.EnqueueCmd(cmdQueue, Cmd.Move, Utils.GetPointBeforeDistance(transform.position, snackTransform.position, interactData.bitingDistance), targetObj: snackTransform.gameObject);
            CmdQueueManager.EnqueueCmd(cmdQueue, Cmd.Look, snackTransform.position);
            CmdQueueManager.EnqueueCmd(cmdQueue, Cmd.Eat, targetObj: snackTransform.gameObject);
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
            CmdQueueManager.ClearCmdQueue(cmdQueue);
            
            CmdQueueManager.EnqueueCmd(cmdQueue, Cmd.Move, pos: Utils.GetPointBeforeDistance(transform.position, toyTransform.position, interactData.bitingDistance), targetObj: toyTransform.gameObject);
            CmdQueueManager.EnqueueCmd(cmdQueue, Cmd.Look, pos: toyTransform.position);
            CmdQueueManager.EnqueueCmd(cmdQueue, Cmd.Bite, targetObj: toyTransform.gameObject);
            CmdQueueManager.EnqueueCmd(cmdQueue, Cmd.Look);
            CmdQueueManager.EnqueueCmd(cmdQueue, Cmd.Move);
            CmdQueueManager.EnqueueCmd(cmdQueue, Cmd.Look);
            CmdQueueManager.EnqueueCmd(cmdQueue, Cmd.Spit);
        }

        #endregion

    
    #region Cmd

        private void SetInitialCmd()
        {
            // EnqueueCmd(Cmd.Move);
        }
    
        private void AddMatchingConditionCmd()
        {
            // if player stay for a while
            if (GameManager.Instance.player.idleTime > interactData.playerIdleTimeThreshold && pet.petStates != PetStates.Sit)
            {
                CmdQueueManager.EnqueueCmd(cmdQueue, Cmd.Look, GameManager.Instance.player.gameObject.transform.position);
                CmdQueueManager.EnqueueCmd(cmdQueue, Cmd.Sit);
            }
            // if player-pet distance increase
            else if(Vector3.Distance(GameManager.Instance.player.gameObject.transform.position, pet.transform.position) >
                    interactData.playerPetMaxDistance)
            {
                Vector2 randomCoord = Random.insideUnitCircle * interactData.playerPetMaxDistance;
                Vector3 nextCoord = GameManager.Instance.player.gameObject.transform.position +
                            new Vector3(randomCoord.x, transform.position.y, randomCoord.y);
                
                CmdQueueManager.EnqueueCmd(cmdQueue, Cmd.Move, nextCoord);
            }
        }
        
    #endregion

    #region InteractWithPet

        /// <summary>
        /// Called when the comb touches or falls on the pet
        /// </summary>
        /// <param name="collisionState">true : onTriggerEnter, false : onTriggerExit</param>
        public void CombCollision(bool collisionState)
        {
            interactData.isBrushing = collisionState;

            if (!collisionState)
            {
                interactData.isBrushing = false;
                interactData.brushingTime = 0f;
            }
        }
        
        private void InteractWithHead()
        {
            pet.InteractHead();
            
            // Stat
            pet.UpdateStat(PetStatNames.Tiredness, -5);
            pet.UpdateStat(PetStatNames.Exp, 5);
            
            // Sound
            pet.PlaySound(PetSounds.Gasps);
            
            Logger.Log("interact head in interactManager");
        }
        
        private void InteractWithJaw()
        {
            pet.InteractJaw();
            
            // Stat
            pet.UpdateStat(PetStatNames.Tiredness, 4);
            pet.UpdateStat(PetStatNames.Exp, 3);
            
            Logger.Log("interact body in interactManager");
        }
        
        private void InteractWithBody()
        {
            pet.InteractBody();
            
            // Stat
            pet.UpdateStat(PetStatNames.Tiredness, -7);
            pet.UpdateStat(PetStatNames.Exp, 7);
            Logger.Log("interact jaw in interactManager");
        }
        
        private void InteractWithHandDetection()
        {
            pet.InteractHandDetection();
            
            // Stat
            pet.UpdateStat(PetStatNames.Tiredness, -10);
            pet.UpdateStat(PetStatNames.Exp, 10);
            Logger.Log("interact HandDetection in interactManager");
        }

    #endregion

    #region InteractInfoFromHand

        /// <summary>
        /// Called when player start touching the interaction part of the pet in player's hand
        /// </summary>
        /// <param name="petPart">The part of the pet touched by the player</param>
        public void PetPartCollisionEnter(PetParts petPart)
        {
            // If player is already touching another part, exit
            if(interactData.isColliding) return;
            
            // If the interaction is ignored, exit
            if (interactData.isInteractionIgnored || pet.inProcess)
            {
                Logger.Log("interaction is ignored, please wait for a while");
                return;
            }
            
            interactData.isColliding = true;
            interactData.curPetCollisionPart = petPart;

            // If it's a "hand" interaction,
            // make sure player is touching it for a certain period of time 
            if (petPart == PetParts.HandDetection)
            {
                interactData.checkPetCollisionTimeCoroutine = StartCoroutine(CheckPetCollisionTime());
            }
            // The rest of the interaction runs immediately upon touch
            else
            {
                CallInteractEvent(petPart);
                
                StartCoroutine(IgnoreInteractionForSeconds(interactData.interactionCoolTime));
                ResetCollisionInfo();
            }
        }

        private IEnumerator CheckPetCollisionTime()
        {
            while (true)
            {
                interactData.collisionTimer += Time.deltaTime;

                if (interactData.collisionTimer > interactData.collisionTimeLimit)
                {
                    CallInteractEvent(interactData.curPetCollisionPart);

                    StartCoroutine(IgnoreInteractionForSeconds(interactData.interactionCoolTime));
                    ResetCollisionInfo();
                    break;
                }
                
                yield return null;
            }
        }

        /// <summary>
        /// To prevent a player from making a mistake,
        /// interaction is prevented for a certain period of time
        /// after the interaction is executed
        /// </summary>
        /// <param name="coolTime">Time the interaction is ignored</param>
        /// <returns></returns>
        private IEnumerator IgnoreInteractionForSeconds(float coolTime)
        {
            interactData.isInteractionIgnored = true;
            Logger.Log("ignore interaction start");

            while (coolTime > 0)
            {
                coolTime -= Time.deltaTime;
                yield return null;
            }

            interactData.isInteractionIgnored = false;
            Logger.Log("ignore interaction end");
        }

        /// <summary>
        /// Run different events depending on the part of the pet
        /// </summary>
        /// <param name="petPart">the interaction site of a pet</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
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

        /// <summary>
        /// Run when hands fall off the area of the pet
        /// </summary>
        /// <param name="petPart">the falling part of a pet</param>
        public void PetPartCollisionExit(PetParts petPart)
        {
            // If the hand and the pet weren't colliding, exit
            if(!interactData.isColliding) return;
            
            // If the part that fell is not in contact with it
            // Considering the situation where the hand touches several pet parts
            // at the same time and then falls off
            if(interactData.curPetCollisionPart != petPart) return;
            
            StopCoroutine(interactData.checkPetCollisionTimeCoroutine);
            ResetCollisionInfo();
        }
        
        private void ResetCollisionInfo()
        {
            interactData.collisionTimer = 0;
            interactData.isColliding = false;
        }

    #endregion
}
