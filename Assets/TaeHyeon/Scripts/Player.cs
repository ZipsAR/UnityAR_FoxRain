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

    private void Start()
    {
        idleMoveThreshold = 0.005f;
        idleTime = 0f;
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


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, StrollManager.Instance.strollData.playerPetMaxDistance);
    }
}
