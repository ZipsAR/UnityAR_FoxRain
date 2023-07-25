using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HandSide
{
    Left,
    Right,
}
public class HandAttacher : MonoBehaviour
{
    private GameObject leftHandObj;
    private GameObject rightHandObj;

    private void Update()
    {
        leftHandObj = GameObject.Find("QC Hand Left");
        rightHandObj = GameObject.Find("QC Hand Right");

        if (leftHandObj != null)
        {
            leftHandObj.AddComponent<HandController>().handSide = HandSide.Left;
            rightHandObj.AddComponent<HandController>().handSide = HandSide.Right;
            AddCollider(leftHandObj);
            AddCollider(rightHandObj);
            AddRigidBody(leftHandObj);
            AddRigidBody(rightHandObj);
            Destroy(gameObject);
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
