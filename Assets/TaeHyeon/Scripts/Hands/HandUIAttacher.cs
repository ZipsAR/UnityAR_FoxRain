using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HandUIAttacher : MonoBehaviour
{
    public GameObject handUIObject;

    public Vector3 targetPos;
    public Quaternion targetRot;
    
    private void Awake()
    {
        targetPos = new Vector3(0, -0.05f, 0.14f);
        targetRot = Quaternion.Euler(new Vector3(-45.0f,-85.0f,-75.0f));
    }

    private void Update()
    {
        if (GameManager.Instance.leftHand != null)
        {
            Instantiate(handUIObject, targetPos, targetRot, GameManager.Instance.leftHand.gameObject.transform);
            Destroy(gameObject);
        }
    }
}
