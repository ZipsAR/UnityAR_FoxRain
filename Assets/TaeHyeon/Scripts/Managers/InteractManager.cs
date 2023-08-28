using System;
using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using Structs;
using UnityEngine;
using Logger = ZipsAR.Logger;
using PlayMode = EnumTypes.PlayMode;
using Random = UnityEngine.Random;

public class InteractManager : MonoBehaviour
{
    // Pet
    private PetBase pet;
    private bool isPetInitialized;
    
    // Manager
    [SerializeField] private InteractData interactData;
    [SerializeField] private Transform selectedPetSpawnTransform;
    
    // Cmd
    private Queue<CmdDetail> cmdQueue;
    private CmdDetail nextCmd;

    // Stat
    private Vector3 prevPetPos;
    private StatChangeCriteria fullnessCriteria;
    private StatChangeCriteria tirednessCriteria;
    private StatChangeCriteria cleanlinessCriteria;
    
    private void Awake()
    {
        InteractEventManager.OnPetSelected -= OnPetSelected;
        InteractEventManager.OnPetSelected += OnPetSelected;
        InteractEventManager.OnPetInitializedToManager -= OnPetInitializedToManager;
        InteractEventManager.OnPetInitializedToManager += OnPetInitializedToManager; 
        
        cmdQueue = new Queue<CmdDetail>();

        fullnessCriteria = new StatChangeCriteria(2, 3, 0f, 0f, 3f, 2f);
        tirednessCriteria = new StatChangeCriteria(1, 2, 0f, 0f, 3f, 2f);
        cleanlinessCriteria = new StatChangeCriteria(1, 2, 0f, 0f, 3f, 2f);
        
        InitializeInteractData();
        
        SetInitialCmd();
    }

    private void OnEnable()
    {
        LoadMoney();
    }

    private void OnDisable()
    {
        InteractEventManager.OnPetSelected -= OnPetSelected;
        InteractEventManager.OnPetInitializedToManager -= OnPetInitializedToManager;

        if (GameManager.Instance.curPetType != PetType.None)
        {
            FirebaseDBManager.Instance.SaveDataToAsync(
                new List<string>{ UserData.UserId, "pet", GameManager.Instance.curPetType.ToString(), "stat" }, 
                pet.GetStat());
            SaveMoney();
        }
    }

    private void Update()
    {
        // Do not run other commands if the pet is running a command
        // or if the pet is not initialized
        if(pet.inProcess || !isPetInitialized) return;
        
        if (CmdQueueController.DequeCmd(cmdQueue, out nextCmd)) CmdQueueController.ExecuteCmd(pet, nextCmd); // if queue is not empty, execute cmd
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
                CmdQueueController.ClearCmdQueue(cmdQueue);
                CmdQueueController.EnqueueCmd(cmdQueue, Cmd.Brush);
                interactData.isBrushing = false;
                interactData.brushingTime = 0f;
            }
        }
    }
    
    private void InitializeInteractData()
    {
        interactData.playerPetMaxDistance = 2f;
        interactData.playerIdleTimeThreshold = 3f;
        
        interactData.bitingDistance = 0.1f;

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
    
    #region Event
    
    private void OnPetSelected(object sender, PetArgs e)
    {
        Instantiate(e.petObj, selectedPetSpawnTransform.position, selectedPetSpawnTransform.rotation);
    }
        
    private void OnPetInitializedToManager(object sender, PetArgs e)
    {
        pet = e.petObj.GetComponent<PetBase>();

        // Stat for distance
        prevPetPos = pet.gameObject.transform.position;
        pet.SetPetAnimationMode(PlayMode.InteractMode);

        // Load stat from firebase
        FirebaseDBManager.Instance.IsDataExistInAsync(
            new List<string>{ UserData.UserId, "pet", GameManager.Instance.curPetType.ToString(), "stat" }, 
            CheckPetStatInDB);
    }

    #endregion

    #region Stat
    
        private void CheckPetStatInDB(bool isExist, List<string> paths)
        {
            if (isExist)
            {
                Logger.Log("user's selected pet stat is exist");
                FirebaseDBManager.Instance.LoadDataFromAsync<PetStatBase>(paths, OnGetPetStat);
            }
            else
            {
                pet.InitializeStatByDefault();
                FirebaseDBManager.Instance.SaveDataToAsync(paths, pet.GetStat());
                
                isPetInitialized = true;
                InteractEventManager.NotifyPetInitializedToAll(pet.gameObject);
            }
        }
    
        private void OnGetPetStat(bool isDataFound, PetStatBase statDB)
        {
            if (isDataFound)
            {
                Logger.Log("get pet stat from db");
                pet.SetPetStatBase(statDB);
                isPetInitialized = true;
                InteractEventManager.NotifyPetInitializedToAll(pet.gameObject);
            }
            else
            {
                Logger.LogError("cannot get pet stat from db");
            }
        }

        private void SaveMoney()
        {
            FileIOSystem.Instance.Save(FileIOSystem.Instance.invendatabase, FileIOSystem.InvenFilename);
            Logger.Log("Save Money call");
        }

        private void LoadMoney()
        {
            FileIOSystem.Instance.Load(FileIOSystem.Instance.invendatabase, FileIOSystem.InvenFilename);
            Logger.Log("Load Money call");            
        }
        
    
        private void StatUpdateByDistance()
        {
            float distanceMoved = Vector3.Distance(pet.transform.position, prevPetPos);
            
            // Decreasing fullness 
            fullnessCriteria.curDistance += distanceMoved;
            if (fullnessCriteria.curDistance > fullnessCriteria.unitDistance)
            {
                fullnessCriteria.curDistance -= fullnessCriteria.unitDistance;
                pet.UpdateStat(PetStatNames.Fullness, -fullnessCriteria.fluctuatingValuePerDistance);
            }

            // Decreasing cleanliness
            cleanlinessCriteria.curDistance += distanceMoved;
            if (cleanlinessCriteria.curDistance > cleanlinessCriteria.unitDistance)
            {
                cleanlinessCriteria.curDistance -= cleanlinessCriteria.unitDistance;
                pet.UpdateStat(PetStatNames.Cleanliness, -cleanlinessCriteria.fluctuatingValuePerDistance);
            }
            
            // Increasing tiredness 
            tirednessCriteria.curDistance += distanceMoved;
            if (tirednessCriteria.curDistance > tirednessCriteria.unitDistance)
            {
                tirednessCriteria.curDistance -= tirednessCriteria.unitDistance;
                pet.UpdateStat(PetStatNames.Tiredness, tirednessCriteria.fluctuatingValuePerDistance);
            }
            
            // Save the current pet's location to the previous location variable
            prevPetPos = pet.gameObject.transform.position;
        }
        
        private void StatUpdateByTime()
        {
            fullnessCriteria.curTime += Time.deltaTime;
            cleanlinessCriteria.curTime += Time.deltaTime;
            tirednessCriteria.curTime += Time.deltaTime;

            if (fullnessCriteria.curTime > fullnessCriteria.unitTime)
            {
                fullnessCriteria.curTime = 0f;
                pet.UpdateStat(PetStatNames.Fullness, -fullnessCriteria.fluctuatingValuePerTime);
            }
            
            if (cleanlinessCriteria.curTime > cleanlinessCriteria.unitTime)
            {
                cleanlinessCriteria.curTime = 0f;
                pet.UpdateStat(PetStatNames.Cleanliness, -cleanlinessCriteria.fluctuatingValuePerTime);
            }

            if (tirednessCriteria.curTime > tirednessCriteria.unitTime)
            {
                tirednessCriteria.curTime = 0f;
                pet.UpdateStat(PetStatNames.Tiredness, tirednessCriteria.fluctuatingValuePerTime);
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
            if(pet == null) return;
            
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
            CmdQueueController.ClearCmdQueue(cmdQueue);
            CmdQueueController.EnqueueCmd(cmdQueue, Cmd.Move, Utils.GetPointBeforeDistance(transform.position, snackTransform.position, interactData.bitingDistance), targetObj: snackTransform.gameObject);
            CmdQueueController.EnqueueCmd(cmdQueue, Cmd.Look, snackTransform.position);
            CmdQueueController.EnqueueCmd(cmdQueue, Cmd.Eat, targetObj: snackTransform.gameObject);
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
            if(pet == null) return;
            
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
            CmdQueueController.ClearCmdQueue(cmdQueue);
            
            CmdQueueController.EnqueueCmd(cmdQueue, Cmd.Move, pos: Utils.GetPointBeforeDistance(transform.position, toyTransform.position, interactData.bitingDistance), targetObj: toyTransform.gameObject);
            CmdQueueController.EnqueueCmd(cmdQueue, Cmd.Look, pos: toyTransform.position);
            CmdQueueController.EnqueueCmd(cmdQueue, Cmd.Bite, targetObj: toyTransform.gameObject);
            CmdQueueController.EnqueueCmd(cmdQueue, Cmd.Look);
            CmdQueueController.EnqueueCmd(cmdQueue, Cmd.Move);
            CmdQueueController.EnqueueCmd(cmdQueue, Cmd.Look);
            CmdQueueController.EnqueueCmd(cmdQueue, Cmd.Spit);
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
                CmdQueueController.EnqueueCmd(cmdQueue, Cmd.Look, GameManager.Instance.player.gameObject.transform.position);
                CmdQueueController.EnqueueCmd(cmdQueue, Cmd.Sit);
            }
            // if player-pet distance increase
            else if(Vector3.Distance(GameManager.Instance.player.gameObject.transform.position, pet.transform.position) >
                    interactData.playerPetMaxDistance)
            {
                Vector2 randomCoord = Random.insideUnitCircle * interactData.playerPetMaxDistance;
                Vector3 nextCoord = GameManager.Instance.player.gameObject.transform.position +
                            new Vector3(randomCoord.x, transform.position.y, randomCoord.y);
                
                CmdQueueController.EnqueueCmd(cmdQueue, Cmd.Move, nextCoord);
            }
        }
        
    #endregion

    #region Interact

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
                    pet.InteractHead();
                    break;
                case PetParts.Jaw:
                    pet.InteractJaw();
                    break;
                case PetParts.Body:
                    pet.InteractBody();
                    break;
                case PetParts.HandDetection:
                    pet.InteractHandDetection();
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
