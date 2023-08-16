using System;
using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;
using Logger = ZipsAR.Logger;

public class HandAttacher : MonoBehaviour
{
    private GameObject leftHandObj;
    private GameObject rightHandObj;
    private GameObject trackables;

    private void Update()
    {
        trackables = GameObject.Find("Trackables");
        if (trackables == null)
        {
            Debug.Log("zipsar : trackables is null");
            return;
        }
        leftHandObj = trackables.transform.Find("QC Hand Left").gameObject;
        rightHandObj = trackables.transform.Find("QC Hand Right").gameObject;

        if (leftHandObj != null)
        {
            leftHandObj.AddComponent<HandController>().handSide = HandSide.Left;
            rightHandObj.AddComponent<HandController>().handSide = HandSide.Right;
            AddCollider(leftHandObj, rightHandObj);
            AddRigidBody(leftHandObj, rightHandObj);
            GameManager.Instance.leftHand = leftHandObj.GetComponent<HandController>();
            GameManager.Instance.rightHand = rightHandObj.GetComponent<HandController>();
            
            Logger.Log("Attach Complete");
            Destroy(gameObject);
        }
        else
        {
            Logger.Log("there is no left hand obj");
        }
    }

    private void AddCollider(GameObject LHandObj, GameObject RHandObj)
    {
        CapsuleCollider leftCollider = LHandObj.AddComponent<CapsuleCollider>();
        
        leftCollider.center = new Vector3(0, 0, 0.07f);
        leftCollider.radius = 0.05f;
        leftCollider.height = 0.2f;
        leftCollider.direction = 2; // Z-Axis
        
        CapsuleCollider rightCollider = RHandObj.AddComponent<CapsuleCollider>();
        
        rightCollider.center = new Vector3(0, 0, 0.07f);
        rightCollider.radius = 0.05f;
        rightCollider.height = 0.2f;
        rightCollider.direction = 2; // Z-Axis
    }

    private void AddRigidBody(GameObject LHandObj, GameObject RHandObj)
    {
        Rigidbody leftRB = LHandObj.AddComponent<Rigidbody>();
        leftRB.useGravity = false;
        
        Rigidbody rightRB = RHandObj.AddComponent<Rigidbody>();
        rightRB.useGravity = false;
    }
}
