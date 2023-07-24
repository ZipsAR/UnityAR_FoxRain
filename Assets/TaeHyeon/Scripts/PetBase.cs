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
}

public enum PetParts
{
    None,
    Head,
    Jaw,
    Body,
}

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
    
    [SerializeField] private AnimationCurve curve;
    private const float SPEED_COEFFICIENT = 0.02f;
    private Vector3 moveDir;
    private float rotationSpeed;
    public bool inprogress { get; private set; }
    private List<bool> isCoroutinePlayingList; // list index is Cmd enum 
    
    private void Start()
    {
        rotationSpeed = 10f;
        petStates = PetStates.Idle;
        animator = GetComponent<Animator>();

        animator.SetInteger(ModeParameter, (int)PlayMode.StrollMode);
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
                break;
            case PlayMode.AgilityMode:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private IEnumerator Init()
    {
        while (true)
        {
            if (GameManager.Instance.player != null)
            {
                playerObj = GameManager.Instance.player.gameObject;
                isInitDone = true;
                isCoroutinePlayingList = new List<bool>();
                for (int i = 0; i < Enum.GetValues(typeof(Cmd)).Length; i++)
                {
                    isCoroutinePlayingList.Add(false);
                }
                break;
            }
            yield return null;
        }
    }

    public void SetPetAnimationMode(PlayMode playMode)
    {
        // animator.SetInteger(ModeParameter, (int)playMode);
        animator.SetInteger("Mode", 1);
    }

    private void UpdateStrollMode()
    {
        inprogress = CheckCoroutinePlaying();
    }

    #region Move
    
    public void CmdMoveTo(Vector3 destination)
    {
        if (CheckCoroutinePlaying())
        {
            return;
        }
        StartCoroutine(MoveSequence(destination));
    }

    private IEnumerator MoveSequence(Vector3 destination)
    {
        isCoroutinePlayingList[(int)Cmd.Move] = true;
        
        Vector3 startPoint = transform.position;
        moveDir = (destination - startPoint).normalized;

        // Set state
        petStates = PetStates.Walk;

        // Set animation
        animator.SetBool(RunningParameter, true);
        
        float t = 0;
        while (transform.position != destination && petStates == PetStates.Walk)
        {
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
        
        Vector3 targetDir = GameManager.Instance.player.gameObject.transform.position - transform.position;
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
        CmdSit();
    }

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
        if (CheckCoroutinePlaying())
        {
            return;
        }

        isCoroutinePlayingList[(int)Cmd.Sit] = true;
        petStates = PetStates.Sit;
        animator.SetTrigger(SitParameter);
        
        // isCoroutinePlayingList[(int)Cmd.Sit] = false; // This part will be executed in the animation part
    }
    #endregion


    // Use in Animator
    public void SitEnd()
    {
        isCoroutinePlayingList[(int)Cmd.Sit] = false;
    }

    // Function to check if there is currently a coroutine running
    private bool CheckCoroutinePlaying()
    {
        if (isCoroutinePlayingList == null) return false;
        
        foreach (bool isPlaying in isCoroutinePlayingList)
        {
            if (isPlaying) return true;
        }

        return false;
    }
}
