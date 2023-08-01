using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

/// <summary>
/// How to add a pet
/// 1. Create Stat data to pet
/// 2. Create an override animator for a pet by inheriting petController
/// 3. Pet connection added to InteractManager
/// 4. Add SitEnd event to sitting animation
/// 5. Add EatEnd event to sitting animation
/// 6. Add BiteEnd event to sitting animation
/// 7. create bite position to mouth
/// 8. Add AttachToyToMouth event to bite animation
/// 9. Add DetachToyFromMouth event to spit animation
/// </summary>

public abstract class PetBase : MonoBehaviour
{
    [SerializeField] protected PetStatBase stat;
    protected Animator animator;
    private GameObject playerObj;
    private bool isInitDone;

    // Animation Parameter
    private static readonly int ModeParameter = Animator.StringToHash("Mode");
    private static readonly int RunningParameter = Animator.StringToHash("Running");
    private static readonly int SitParameter = Animator.StringToHash("Sit");
    
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
    
    private void Start()
    {
        rotationSpeed = 10f;
        petStates = PetStates.Idle;
        animator = GetComponent<Animator>();
        isBiting = false;
        // The position y value of the pet is fixed to the initial y value
        fixedPosY = transform.position.y;
        
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
                playerObj = GameManager.Instance.player.gameObject;
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
        animator.SetInteger(ModeParameter, (int)playMode);
    }

    private void UpdateStrollMode()
    {
        inProcess = CheckCoroutinePlaying();
    }
    
    private void UpdateInteractMode()
    {
        inProcess = CheckCoroutinePlaying();
    }
    
    #region Move
    
    public void CmdMoveTo(Vector3 destination)
    {
        Logger.Log("[Cmd] Move To " + destination);

        // This cmd will not run if another cmd is running
        if (CheckCoroutinePlaying())
        {
            return;
        }
        StartCoroutine(MoveSequence(destination));
    }

    private IEnumerator MoveSequence(Vector3 destination)
    {
        isCoroutinePlayingList[(int)Cmd.Move] = true;
        
        // Pets must move only on the xz plane
        destination = new Vector3(destination.x, fixedPosY, destination.z);
        
        Vector3 startPoint = transform.position;
        moveDir = (destination - startPoint).normalized;

        // Set state
        petStates = PetStates.Walk;

        // Set animation
        animator.SetBool(RunningParameter, true);
        
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
            transform.position = Vector3.Lerp(startPoint, destination, curve.Evaluate(t));
            
            // Set Rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, 
                Quaternion.LookRotation(moveDir), 
                Time.deltaTime * rotationSpeed);

            yield return null;
        }

        // Set state
        petStates = PetStates.Idle;
        
        // Set animation
        animator.SetBool(RunningParameter, false);
        
        isCoroutinePlayingList[(int)Cmd.Move] = false;
    }
    #endregion
    
    #region Look
    public void CmdLookPlayer()
    {
        Logger.Log("[Cmd] Look player");
        
        // This cmd will not run if another cmd is running
        if (CheckCoroutinePlaying())
        {
            return;
        }
        petStates = PetStates.Idle;
        StartCoroutine(LookPlayerSequence());
    }
    
    private IEnumerator LookPlayerSequence()
    {
        isCoroutinePlayingList[(int)Cmd.Look] = true;

        // Pet rotates only on the y-axis
        Vector3 targetDir = GameManager.Instance.player.gameObject.transform.position - transform.position;
        targetDir.y = 0;
        Quaternion targetQuaternion = Quaternion.LookRotation(targetDir);
        
        while (transform.rotation != targetQuaternion)
        {
            // 현재 rotation과 taretQuaternion의 각도 차이가 5도 이하인 경우 모두 회전한 것으로 판단
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
        
        
        // CmdSit();
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
        
        // This cmd will not run if another cmd is running
        if (CheckCoroutinePlaying())
        {
            return;
        }

        isCoroutinePlayingList[(int)Cmd.Sit] = true;
        petStates = PetStates.Sit;
        animator.SetTrigger(SitParameter);
        
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
        animator.Play("Eat");

        snackObj = frontSnack;
        // isCoroutinePlayingList[(int)Cmd.Eat] = false; This part will be executed in the animation part
    }

    public void EatEnd()
    {
        isCoroutinePlayingList[(int)Cmd.Eat] = false;
        Destroy(snackObj);
        snackObj = null;
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
        animator.Play("Brush");

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
        isCoroutinePlayingList[(int)Cmd.Bite] = true;
        animator.Play("PreBite");
        Logger.Log("play prebite animation");
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
        isCoroutinePlayingList[(int)Cmd.Spit] = true;
        animator.Play("PreSpit");

        // isCoroutinePlayingList[(int)Cmd.Eat] = false; This part will be executed in the animation part
    }

    public void DetachToyFromMouth()
    {
        Logger.Log("DetachToyFromMouth");
        isBiting = false;
        toyObj.transform.SetParent(null);
        toyObj.GetComponent<Rigidbody>().isKinematic = false;
    }

    public void SpitEnd()
    {
        isCoroutinePlayingList[(int)Cmd.Spit] = false;
        Logger.Log("SpitEnd is activate");
    }

    #endregion


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
    
    
    
    
    public abstract void InteractHead();
    public abstract void InteractJaw();
    public abstract void InteractBody();
    public abstract void InteractHandDetection();
}
