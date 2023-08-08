using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;
using Logger = ZipsAR.Logger;

public enum PetStates
{
    Idle,
    Walk,
    Sit,
}

public enum Cmd
{
    Move = 0,
    Look = 1,
    Sit = 2,
    Eat = 3,
    Brush = 4,
    Bite = 5,
    Spit = 6,
}

public enum PetParts
{
    None,
    Head,
    Jaw,
    Body,
    HandDetection,
}

// If this order is changed, the order of sound in the inspector window must be changed accordingly
public enum PetSounds
{
    Bark1,
    Bark2,
    Bark3,
    Gasps,
    Sniff,
    Whines,
    Eat,
}

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
    [SerializeField] protected PetStatBase stat;
    protected Animator animator;
    private bool isInitDone;

    // Animation Parameter
    private static readonly int Mode = Animator.StringToHash("Mode");
    private static readonly int Running = Animator.StringToHash("Running");
    private static readonly int Sit = Animator.StringToHash("Sit");
    private static readonly int Spit = Animator.StringToHash("Spit");
    private static readonly int Bite = Animator.StringToHash("Bite");
    private static readonly int Eat = Animator.StringToHash("Eat");
    private static readonly int Brush = Animator.StringToHash("Brush");
    private static readonly int Interact = Animator.StringToHash("Interact");

    // Sounds
    [SerializeField] private List<Sound> petSoundList;

    // Stroll Mode
    public PetStates petStates { private set; get; }
    
    [SerializeField] private AnimationCurve curve; // Curve indicating where the pet is moving
    private const float SPEED_COEFFICIENT = 0.02f;
    private Vector3 moveDir;
    private float rotationSpeed;
    private List<bool> isCoroutinePlayingList; // list index is Cmd enum 
    private float fixedPosY; // Pet always moves at the height of this value
    public bool inProcess { private set; get; } // If the pet is executing a command, it's false
    private GameObject snackObj;
    private GameObject toyObj;
    private bool isBiting;
    public Transform toyAttachPoint;

    // Effects
    [SerializeField] private Transform levelEffectAttachPoint;

    private GameObject curEmotionObj;
    [SerializeField] private Transform emotionMarkPosition;
    [SerializeField] private GameObject exclamationMark;
    
    private void Awake()
    {
        rotationSpeed = 10f;
        petStates = PetStates.Idle;
        animator = GetComponent<Animator>();
        isBiting = false;
        curEmotionObj = null;
        
        // Audio validation check
        if (petSoundList.Count != Enum.GetNames(typeof(PetSounds)).Length)
            throw new Exception("Number of petSoundList and number of PetSounds do not match");

        // Init pet stat individually
        // PetStatInitialize();
        ShowCurPetStat();
        Logger.Log("pet stat initialized");
        
        // The position y value of the pet is fixed to the initial y value
        fixedPosY = transform.position.y;
    }

    private void Start()
    {
        StartCoroutine(Init());
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

    /// <summary>
    /// What to initialize using other script references
    /// Update portion runs after this initialization is complete
    /// </summary>
    /// <returns></returns>
    private IEnumerator Init()
    {
        while (true)
        {
            if (GameManager.Instance.player != null)
            {
                isCoroutinePlayingList = new List<bool>();
                for (int i = 0; i < Enum.GetValues(typeof(Cmd)).Length; i++)
                {
                    isCoroutinePlayingList.Add(false);
                }
                    
                // Check Initialization Completed
                isInitDone = true;
                break;
            }
            yield return null;
        }
    }
    
    public void SetPetAnimationMode(PlayMode playMode)
    {
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
    public void PlaySound(PetSounds sound)
    {
        GameManager.Instance.interactAudioManager.PlayPetSound(petSoundList[(int)sound].clip);
    }

    
    #region InteractPart

        public void InteractHead() => animator.SetInteger(Interact, (int)PetParts.Head);
        public void InteractJaw() => animator.SetInteger(Interact, (int)PetParts.Jaw);
        public void InteractBody() => animator.SetInteger(Interact, (int)PetParts.Body);
        public void InteractHandDetection() => animator.SetInteger(Interact, (int)PetParts.HandDetection);

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
                destination = new Vector3(destination.x, fixedPosY, destination.z);
                
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
                    
                    // Set position
                    t = Mathf.MoveTowards(t, 1, stat.speed * Time.deltaTime * SPEED_COEFFICIENT);
                    Transform trans;
                    (trans = transform).position = Vector3.Lerp(startPoint, destination, curve.Evaluate(t));
                    
                    // Set Rotation
                    transform.rotation = Quaternion.Lerp(trans.rotation, 
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
                isCoroutinePlayingList[(int)Cmd.Eat] = false;
                Destroy(snackObj);
                snackObj = null;
                
                // Stat
                UpdateStat(PetStatNames.Fullness, 10);
                UpdateStat(PetStatNames.Exp, 10);
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
                UpdateStat(PetStatNames.Cleanliness, 10);
                UpdateStat(PetStatNames.Exp, 7);
                
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
                toyObj.GetComponent<Rigidbody>().isKinematic = false;
                
                // Enable this object to be grabbed
                toyObj.GetComponent<XRGrabInteractable>().enabled = true;
                toyObj.GetComponent<Rigidbody>().isKinematic = false;
                Invoke(nameof(SetIsKinematicFalse), 1f);
                
                // Sound
                PlaySound(PetSounds.Bark2);
                
                // Stat
                UpdateStat(PetStatNames.Tiredness, 5);
                UpdateStat(PetStatNames.Exp, 40);
                Logger.Log("exp update plus 40");
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
                        InteractEventManager.RaisePetStatChanged(stat, PetStatNames.Level, preLevel, postLevel);

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
            InteractEventManager.RaisePetStatChanged(stat, statName, preStatValue, postStatValue);
            
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
