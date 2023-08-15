using System;
using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Logger = ZipsAR.Logger;
using PlayMode = EnumTypes.PlayMode;


/// <summary>
/// How to add a pet
/// 1.  Create Stat data to pet
/// 2.  Create an override animator for a pet by inheriting petController
/// 3.  Pet connection added to InteractManager
/// 4.  Add SitEnd event to sitting animation
/// 5.  Add EatEnd event to sitting animation
/// 6.  Add BiteEnd event to sitting animation
/// 7.  create bite position to mouth
/// 8.  Add AttachToyToMouth event to bite animation
/// 9.  Add DetachToyFromMouth event to spit animation
/// 10. Add petSounds in inspector
/// 11. Set InteractTerminated Event to Interact animation in last frame
/// </summary>

public abstract class PetBase : MonoBehaviour
{    
    public PetStates petStates { private set; get; }
    public bool inProcess { private set; get; } // If the pet is executing a command, it's false
    [SerializeField] protected PetStatBase stat;
    private Animator animator;
    private bool isInitDone;
    private List<bool> isCoroutinePlayingList; // list index is Cmd enum 
    
    // Move
    private const float SPEED_COEFFICIENT = 0.05f;
    private Vector3 moveDir;
    private float rotationSpeed;

    // Interact Object
    private GameObject snackObj;
    private GameObject toyObj;
    private bool isBiting;
    public Transform toyAttachPoint;

    // Effects
    [SerializeField] private Transform levelEffectAttachPoint;
    private GameObject curEmotionObj;
    [SerializeField] private Transform emotionMarkPosition;
    [SerializeField] private GameObject exclamationMark;
    
    // Sounds
    [SerializeField] private List<Sound> petSoundList;
    
    
    // Animation Parameter
    private static readonly int Mode = Animator.StringToHash("Mode");
    private static readonly int Running = Animator.StringToHash("Running");
    private static readonly int Sit = Animator.StringToHash("Sit");
    private static readonly int Spit = Animator.StringToHash("Spit");
    private static readonly int Bite = Animator.StringToHash("Bite");
    private static readonly int Eat = Animator.StringToHash("Eat");
    private static readonly int Brush = Animator.StringToHash("Brush");
    private static readonly int Interact = Animator.StringToHash("Interact");
    
    private void Awake()
    {
        rotationSpeed = 10f;
        petStates = PetStates.Idle;
        isBiting = false;
        curEmotionObj = null;
        
        // Audio validation check
        if (petSoundList.Count != Enum.GetNames(typeof(PetSounds)).Length)
            throw new Exception("Number of petSoundList and number of PetSounds do not match");
    }

    private void Start()
    {
        isCoroutinePlayingList = new List<bool>();
        for (int i = 0; i < Enum.GetValues(typeof(Cmd)).Length; i++)
        {
            isCoroutinePlayingList.Add(false);
        }
                    
        // Check Initialization Completed
        isInitDone = true;
                
        InteractEventManager.NotifyPetInitializedToManager(gameObject);
    }

    private void Update()
    {
        if (!isInitDone) return;

        switch (GameManager.Instance.currentPlayMode)
        {
            case PlayMode.StrollMode:
                UpdateStrollMode();
                break;
            case PlayMode.InteractMode:
                UpdateInteractMode();
                break;
            case PlayMode.AgilityMode:
                break;
            case PlayMode.None:
                break;
            case PlayMode.StoreMode:
                break;
            case PlayMode.HousingMode:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SetPetAnimationMode(PlayMode playMode)
    {
        animator = GetComponent<Animator>();
        animator.SetInteger(Mode, (int)playMode);
        animator.SetInteger(Interact, (int)PetParts.None);
    }

    private void UpdateStrollMode()
    {
        inProcess = CheckCoroutinePlaying();
    }
    
    private void UpdateInteractMode()
    {
        inProcess = CheckCoroutinePlaying();
    }
    
    
    /// <summary>
    /// Function to check if there is currently a coroutine running
    /// </summary>
    /// <returns></returns>
    private bool CheckCoroutinePlaying()
    {
        if (isCoroutinePlayingList == null) return false;
        
        foreach (bool isPlaying in isCoroutinePlayingList)
        {
            if (isPlaying) return true;
        }

        return false;
    }
    
    // Sound
    private void PlaySound(PetSounds sound)
    {
        if(GameManager.Instance.currentPlayMode == PlayMode.InteractMode)
            GameManager.Instance.interactAudioManager.PlayPetSound(petSoundList[(int)sound].clip);
    }

    
    #region InteractPart

        public void InteractHead()
        {
            PlaySound(PetSounds.Bark3);
            animator.SetInteger(Interact, (int)PetParts.Head);
            
            // Stat
            UpdateStat(PetStatNames.Tiredness, -7);
            UpdateStat(PetStatNames.Exp, 10);
                
            // Sound
            PlaySound(PetSounds.Gasps);
        }

        public void InteractJaw()
        {
            PlaySound(PetSounds.Bark3);
            animator.SetInteger(Interact, (int)PetParts.Jaw);
            
            // Stat
            UpdateStat(PetStatNames.Tiredness, -7);
            UpdateStat(PetStatNames.Exp, 7);
        }

        public void InteractBody()
        {
            PlaySound(PetSounds.Sniff);
            animator.SetInteger(Interact, (int)PetParts.Body);
            
            // Stat
            UpdateStat(PetStatNames.Tiredness, -10);
            UpdateStat(PetStatNames.Exp, 15);

        }

        public void InteractHandDetection()
        {
            PlaySound(PetSounds.Whines);
            animator.SetInteger(Interact, (int)PetParts.HandDetection);
            
            UpdateStat(PetStatNames.Tiredness, -15);
            UpdateStat(PetStatNames.Exp, 20);
        }

        public void InteractTerminated()
        {
            animator.SetInteger(Interact, (int)PetParts.None);
        }
        
    #endregion
    
    
    #region Cmds
    
        /// <summary>
        /// Abort all cmd of the current pet
        /// </summary>
        public void AbortAllCmd()
        {
            Logger.Log("Abort all cmd of the current pet");
            StopAllCoroutines();
            for (int i = 0; i < isCoroutinePlayingList.Count; i++)
            {
                isCoroutinePlayingList[i] = false;
            }
            inProcess = false;
        }
        
        #region Move
        
            /// <summary>
            /// if purpose is true, it means pet is move to snack or toy
            /// so, exclamationMark will appear, until pet bite object
            /// </summary>
            /// <param name="destination">Pet's Destination</param>
            /// <param name="purpose">Whether the pet is directed to an object or not</param>
            public void CmdMoveTo(Vector3 destination, bool purpose = default)
            {
                Logger.Log("[Cmd] Move To " + destination);
    
                // This cmd will not run if another cmd is running
                if (CheckCoroutinePlaying())
                {
                    return;
                }

                if (purpose && curEmotionObj == null)
                {
                    curEmotionObj = Instantiate(exclamationMark, emotionMarkPosition);
                }
                
                StartCoroutine(MoveSequence(destination));
            }
    
            private IEnumerator MoveSequence(Vector3 destination)
            {
                isCoroutinePlayingList[(int)Cmd.Move] = true;
                
                // Sound
                PlaySound(PetSounds.Gasps);
                
                // Pets must move only on the xz plane
                destination = new Vector3(destination.x, GameData.floorHeight, destination.z);
                
                Vector3 startPoint = transform.position;
                moveDir = (destination - startPoint).normalized;
    
                // Set state
                petStates = PetStates.Walk;
    
                // Set animation
                animator.SetBool(Running, true);
                
                float t = 0;
                while (transform.position != destination && petStates == PetStates.Walk)
                {
                    // If it reaches as close as the distance x, it is considered to have arrived
                    if (Vector3.Distance(transform.position, destination) < 0.02f)
                    {
                        Logger.Log("force stop");
                        break;
                    }
                    
                    Vector3 velocity = moveDir.normalized * (stat.speed * SPEED_COEFFICIENT);
                    transform.position += velocity * Time.deltaTime;
                    
                    // Set Rotation
                    transform.rotation = Quaternion.Lerp(transform.rotation, 
                        Quaternion.LookRotation(moveDir), 
                        Time.deltaTime * rotationSpeed);
    
                    yield return null;
                }
    
                // Set state
                petStates = PetStates.Idle;
                
                // Set animation
                animator.SetBool(Running, false);
                
                isCoroutinePlayingList[(int)Cmd.Move] = false;
            }
            
        #endregion
        
        #region Look
        
            public void CmdLook(Vector3 targetPos)
            {
                Logger.Log("[Cmd] Look " + targetPos);
                
                // This cmd will not run if another cmd is running
                if (CheckCoroutinePlaying())
                {
                    return;
                }
                petStates = PetStates.Idle;
                StartCoroutine(LookSequence(targetPos));
            }
            
            private IEnumerator LookSequence(Vector3 targetPos)
            {
                isCoroutinePlayingList[(int)Cmd.Look] = true;
    
                // Pet rotates only on the y-axis
                Vector3 targetDir = targetPos - transform.position;
                targetDir.y = 0;
                Quaternion targetQuaternion = Quaternion.LookRotation(targetDir);
                
                while (transform.rotation != targetQuaternion)
                {
                    // 현재 rotation과 targetQuaternion의 각도 차이가 5도 이하인 경우 모두 회전한 것으로 판단
                    if (AngleDiffBetween(transform.rotation, targetQuaternion) < 5f)
                    {
                        transform.rotation = targetQuaternion;
                        break;
                    }
                    transform.rotation = Quaternion.Lerp(transform.rotation, 
                        targetQuaternion,
                        Time.deltaTime * rotationSpeed);
                    yield return null;
                }
                
                isCoroutinePlayingList[(int)Cmd.Look] = false;
            }
    
            /// <summary>
            /// Calculate the difference in angle between two quaternions
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            private float AngleDiffBetween(Quaternion a, Quaternion b)
            {
                Quaternion diff = Quaternion.Inverse(a) * b;
                float angleDiff = Quaternion.Angle(Quaternion.identity, diff);
                return angleDiff; // Degree notation
            }
    
        #endregion
        
        #region Sit
    
            public void CmdSit()
            {
                Logger.Log("[Cmd] Sit");
                
                // Sound
                PlaySound(PetSounds.Bark3);

                // This cmd will not run if another cmd is running
                if (CheckCoroutinePlaying())
                {
                    return;
                }
    
                isCoroutinePlayingList[(int)Cmd.Sit] = true;
                petStates = PetStates.Sit;
                animator.SetTrigger(Sit);
                
                // isCoroutinePlayingList[(int)Cmd.Sit] = false; // This part will be executed in the animation part
            }
            
            /// <summary>
            /// Use in Animator
            /// Check that the sitting motion is over  
            /// </summary>
            public void SitEnd()
            {
                isCoroutinePlayingList[(int)Cmd.Sit] = false;
                Logger.Log("SitEnd is activate");
            }
    
        #endregion
        
        #region Eat
    
            public void CmdEat(GameObject frontSnack)
            {
                Logger.Log("[Cmd] Eat");
                
                // This cmd will not run if another cmd is running
                if (CheckCoroutinePlaying())
                {
                    return;
                }
                isCoroutinePlayingList[(int)Cmd.Eat] = true;
                animator.SetTrigger(Eat);
    
                // Sound
                PlaySound(PetSounds.Eat);
                
                // EmotionMark
                if (curEmotionObj != null)
                {
                    Destroy(curEmotionObj);
                    curEmotionObj = null;
                }

                snackObj = frontSnack;
                // isCoroutinePlayingList[(int)Cmd.Eat] = false; This part will be executed in the animation part
            }
    
            public void EatEnd()
            {
                // Tutorial
                if (snackObj != null && snackObj.TryGetComponent(out TutorialItem tutorialItem))
                {
                    tutorialItem.EndItemTutorial(TutorialType.Snack);
                }
                
                isCoroutinePlayingList[(int)Cmd.Eat] = false;
                Destroy(snackObj);
                snackObj = null;
                
                // Stat
                UpdateStat(PetStatNames.Fullness, 15);
                UpdateStat(PetStatNames.Exp, 40);
                UpdateStat(PetStatNames.Tiredness, -10);

                Logger.Log("EatEnd is activate");
            }
        
        #endregion
    
        #region Brush
    
            public void CmdBrush()
            {
                Logger.Log("[Cmd] Brush");
                
                // This cmd will not run if another cmd is running
                if (CheckCoroutinePlaying())
                {
                    return;
                }
                isCoroutinePlayingList[(int)Cmd.Brush] = true;
                animator.SetTrigger(Brush);
                
                // Sound
                PlaySound(PetSounds.Bark3);
                
                // Stat
                UpdateStat(PetStatNames.Cleanliness, 15);
                UpdateStat(PetStatNames.Tiredness, -15);
                UpdateStat(PetStatNames.Exp, 15);
                
                // isCoroutinePlayingList[(int)Cmd.Brush] = false; This part will be executed in the animation part
            }
    
            public void BrushEnd()
            {
                isCoroutinePlayingList[(int)Cmd.Brush] = false;
                Logger.Log("BrushEnd is activate");
            }
    
        #endregion
    
        #region Bite
    
            public void CmdBite(GameObject frontToy)
            {
                Logger.Log("[Cmd] Bite");
                
                // This cmd will not run if another cmd is running
                if (CheckCoroutinePlaying())
                {
                    return;
                }

                if (curEmotionObj != null)
                {
                    Destroy(curEmotionObj);
                    curEmotionObj = null;
                }

                isCoroutinePlayingList[(int)Cmd.Bite] = true;
                animator.SetTrigger(Bite);
                toyObj = frontToy;

                // isCoroutinePlayingList[(int)Cmd.Bite] = false; This part will be executed in the animation part
            }
    
            public void AttachToyToMouth()
            {
                Logger.Log("AttachToyToMouth");
                isBiting = true;
                toyObj.transform.SetParent(toyAttachPoint);
                toyObj.transform.localPosition = Vector3.zero;
                toyObj.transform.localRotation = Quaternion.identity;
                toyObj.GetComponent<Rigidbody>().isKinematic = true;
            }
    
            public void BiteEnd()
            {
                isCoroutinePlayingList[(int)Cmd.Bite] = false;
                Logger.Log("BiteEnd is activate");
            }
    
        #endregion
    
        #region Spit
    
            public void CmdSpit()
            {
                Logger.Log("[Cmd] Spit");
                
                // This cmd will not run if another cmd is running
                if (CheckCoroutinePlaying())
                {
                    return;
                }
    
                if (!isBiting) throw new Exception("no toys in pet's mouth");
                
                isCoroutinePlayingList[(int)Cmd.Spit] = true;
                animator.SetTrigger(Spit);
                
                // isCoroutinePlayingList[(int)Cmd.Eat] = false; This part will be executed in the animation part
            }
    
            public void DetachToyFromMouth()
            {
                Logger.Log("DetachToyFromMouth");
                isBiting = false;
                toyObj.transform.SetParent(null);
                
                // Enable this object to be grabbed
                toyObj.GetComponent<XRGrabInteractable>().enabled = true;
                toyObj.GetComponent<Rigidbody>().isKinematic = false;
                Invoke(nameof(SetIsKinematicFalse), 1f);
                
                // Sound
                PlaySound(PetSounds.Bark2);
                
                // Stat
                UpdateStat(PetStatNames.Tiredness, 5);
                UpdateStat(PetStatNames.Exp, 60);
                Logger.Log("exp update plus 60");
                
                
                // Tutorial
                if (toyObj.TryGetComponent(out TutorialItem tutorialItem))
                {
                    tutorialItem.EndItemTutorial(TutorialType.Toy);
                }
            }
    
            public void SpitEnd()
            {
                isCoroutinePlayingList[(int)Cmd.Spit] = false;
                Logger.Log("SpitEnd is activate");
            }

            private void SetIsKinematicFalse() => toyObj.GetComponent<Rigidbody>().isKinematic = false;

        #endregion

        
        
    #endregion

    
    #region Stat
     
        public PetStatBase GetStat() => stat;

        public void SetPetStatBase(PetStatBase loadedStat) => stat = loadedStat;

        public void UpdateStat(PetStatNames statName, int amountOfChange)
        {
            if (amountOfChange == 0) throw new Exception("Stat change value must not always be zero");
            int preStatValue;
            int postStatValue;
            switch (statName)
            {
                case PetStatNames.Fullness:
                    preStatValue = stat.fullness;
                    postStatValue = Mathf.Clamp(stat.fullness + amountOfChange, stat.statMin, stat.statMax);
                    stat.fullness = postStatValue;
                    break;
                
                case PetStatNames.Tiredness:
                    preStatValue = stat.tiredness;
                    postStatValue = Mathf.Clamp(stat.tiredness + amountOfChange, stat.statMin, stat.statMax);
                    stat.tiredness = postStatValue;
                    break;
                
                case PetStatNames.Cleanliness:
                    preStatValue = stat.cleanliness;
                    postStatValue = Mathf.Clamp(stat.cleanliness + amountOfChange, stat.statMin, stat.statMax);
                    stat.cleanliness = postStatValue;
                    break;
                
                case PetStatNames.Exp:
                    preStatValue = stat.exp;
                    int combinedExp = stat.exp + amountOfChange;
                    
                    if (combinedExp >= stat.expMax)
                    {
                        int preLevel = stat.level;
                        int levelGain = combinedExp / stat.expMax;
                        int expGain = combinedExp % stat.expMax;
                        int postLevel = Mathf.Clamp(stat.level + levelGain, 1, stat.levelMax);
                        stat.level = postLevel;
                        InteractEventManager.NotifyPetStatChanged(PetStatNames.Level, preLevel, postLevel);

                        postStatValue = expGain;
                        stat.exp = postStatValue;
                        Logger.Log("levelGain : " + levelGain);
                        Logger.Log("expGain : " + expGain);
                    }
                    else
                    {
                        postStatValue = combinedExp;
                        stat.exp = postStatValue;
                        Logger.Log("current exp after increase : " + stat.exp);
                    }
                    break;
                
                case PetStatNames.Level:
                    preStatValue = stat.level;
                    postStatValue = Mathf.Clamp(stat.level + amountOfChange, 1, stat.levelMax);
                    stat.level = postStatValue;
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(statName), statName, null);
            }

            // Notify to InteractUIManager
            InteractEventManager.NotifyPetStatChanged(statName, preStatValue, postStatValue);
            
            // ShowCurPetStat();
        }

        public abstract void InitializeStatByDefault();
        
        private void ShowCurPetStat()
        {
            string str = "";
            str += "fullness : " + stat.fullness + "\n";
            str += "tiredness : " + stat.tiredness + "\n";
            str += "cleanliness : " + stat.cleanliness + "\n";
            str += "exp : " + stat.exp + "\n";
            str += "level : " + stat.level + "\n";
            
            Logger.Log(str);
        }

    #endregion

    
    #region Effects

        public Transform GetEffectPosition() => levelEffectAttachPoint;

        #endregion
}
