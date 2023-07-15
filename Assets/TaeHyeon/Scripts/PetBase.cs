using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = ZipsAR.Logger;

public enum PetStates
{
    Idle,
    Walk,
}

public abstract class PetBase : MonoBehaviour
{
    [SerializeField] protected PetStatBase stat;
    [SerializeField] private AnimationCurve curve;
    private Animator animator;
    private GameObject playerObj;
    private bool isInitDone;

    // Animation Parameter
    private static readonly int ModeParameter = Animator.StringToHash("Mode");
    
    // Stroll Mode
    private const float SPEED_COEFFICIENT = 0.02f;
    public PetStates PetStates { get; private set; }
    private float rotationSpeed;
    private void Start()
    {
        rotationSpeed = 10f;
        PetStates = PetStates.Idle;
        animator = GetComponent<Animator>();
        StartCoroutine(Init());
        StartCoroutine(MoveCoroutine(new Vector3(1f, 0, 1f)));
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

    public void MoveTo(Vector3 destination)
    {
        StartCoroutine(MoveCoroutine(destination));
    }

    private IEnumerator MoveCoroutine(Vector3 destination)
    {
        Vector3 startPoint = transform.position;
        Vector3 dir = (destination - startPoint).normalized;

        // Set state
        PetStates = PetStates.Walk;

        float t = 0;
        while (transform.position != destination)
        {
            // Set position
            t = Mathf.MoveTowards(t, 1, stat.speed * Time.deltaTime * SPEED_COEFFICIENT);
            transform.position = Vector3.Lerp(startPoint, destination, curve.Evaluate(t));
            
            // Set Rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, 
                Quaternion.LookRotation(dir), 
                Time.deltaTime * rotationSpeed);

            yield return null;
        }

        // Set state
        PetStates = PetStates.Idle;
    }
}
