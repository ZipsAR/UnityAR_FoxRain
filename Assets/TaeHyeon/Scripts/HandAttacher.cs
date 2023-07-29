using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = ZipsAR.Logger;

public enum HandSide
{
    Left,
    Right,
}
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
            AddCollider(leftHandObj);
            AddCollider(rightHandObj);
            AddRigidBody(leftHandObj);
            AddRigidBody(rightHandObj);
            ZipsAR.Logger.Log("Attach Complete");
            Destroy(gameObject);
        }
        else
        {
            Logger.Log("there is no left hand obj");
        }
    }

    private void AddCollider(GameObject obj)
    {
        CapsuleCollider collider = obj.AddComponent<CapsuleCollider>();
        
        collider.center = new Vector3(0, 0, 0.07f);
        collider.radius = 0.05f;
        collider.height = 0.2f;
        collider.direction = 2; // Z-Axis
    }

    private void AddRigidBody(GameObject obj)
    {
        Rigidbody rb = obj.AddComponent<Rigidbody>();
        rb.useGravity = false;
    }
}
