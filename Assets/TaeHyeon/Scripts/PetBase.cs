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

public abstract class PetBase : MonoBehaviour
{
    [SerializeField] protected PetStatBase stat;
    private Animator animator;
    private GameObject playerObj;
    private bool isInitDone;

    // Animation Parameter
    private static readonly int ModeParameter = Animator.StringToHash("Mode");
    private static readonly int RunningParameter = Animator.StringToHash("Running");
    private static readonly int SitParameter = Animator.StringToHash("Sit");
    
    // Stroll Mode
    public PetStates PetStates { private set; get; }
    
    [SerializeField] private AnimationCurve curve;
    private const float SPEED_COEFFICIENT = 0.02f;
    private Vector3 moveDir;
    private float rotationSpeed;
    
    private void Start()
    {
        rotationSpeed = 10f;
        PetStates = PetStates.Idle;
        animator = GetComponent<Animator>();
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
            if (GameManager.Instance.Player != null)
            {
                playerObj = GameManager.Instance.Player.gameObject;
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
    }

    #region Move
    
    public void CmdMoveTo(Vector3 destination)
    {
        StartCoroutine(MoveSequence(destination));
    }

    private IEnumerator MoveSequence(Vector3 destination)
    {
        Vector3 startPoint = transform.position;
        moveDir = (destination - startPoint).normalized;

        // Set state
        PetStates = PetStates.Walk;

        // Set animation
        animator.SetBool(RunningParameter, true);
        
        float t = 0;
        while (transform.position != destination && PetStates == PetStates.Walk)
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
        PetStates = PetStates.Idle;
        
        // Set animation
        animator.SetBool(RunningParameter, false);
    }
    #endregion

    #region Look
    public void CmdLookPlayer()
    {
        PetStates = PetStates.Idle;
        StartCoroutine(LookPlayerSequence());
    }
    
    private IEnumerator LookPlayerSequence()
    {
        Vector3 targetDir = GameManager.Instance.Player.gameObject.transform.position - transform.position;
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
        animator.SetTrigger(SitParameter);
    }
    #endregion
}
