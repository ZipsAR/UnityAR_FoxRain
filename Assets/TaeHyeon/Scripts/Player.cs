using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = ZipsAR.Logger;

public class Player : MonoBehaviour
{
    private float idleMoveThreshold;
    private Vector3 previousPos;
    public float idleTime { get; private set; }
    public float headToChestDistance;
    private void Start()
    {
        idleMoveThreshold = 0.005f;
        idleTime = 0f;
        headToChestDistance = 0.4f;
        
        previousPos = transform.position;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, previousPos) < idleMoveThreshold)
        {
            idleTime += Time.deltaTime;
        }
        else
        {
            idleTime = 0f;
        }

        previousPos = transform.position;
    }

    public Vector3 GetChestPosition() => transform.position + Vector3.down * headToChestDistance;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        switch (GameManager.Instance.currentPlayMode)
        {
            case PlayMode.None:
                break;
            case PlayMode.InteractMode:
                Gizmos.DrawWireSphere(transform.position, GameManager.Instance.interactManager.GetInteractData().playerPetMaxDistance);
                break;
            case PlayMode.StrollMode:
                Gizmos.DrawWireSphere(transform.position, GameManager.Instance.strollManager.strollData.playerPetMaxDistance);
                break;
            case PlayMode.AgilityMode:
                break;
            case PlayMode.StoreMode:
                break;
            case PlayMode.HousingMode:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
