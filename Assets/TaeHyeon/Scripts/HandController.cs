using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = ZipsAR.Logger;

public class HandController : MonoBehaviour
{
    public HandSide handSide;
    private float collisionTimer;
    private bool isColliding;
    private PetParts petCollisionPart;

    private void Start()
    {
        collisionTimer = 0f;
        isColliding = false;
    }

    private void Update()
    {
        if (isColliding)
        {
            collisionTimer += Time.deltaTime;
            
        }
    }


    private void OnCollisionEnter(Collision other)
    {
        PetCollisionHandler handler;
        if ((handler = other.gameObject.GetComponent<PetCollisionHandler>()) != null)
        {
            Logger.Log(handler.petParts + " start colliding");
            petCollisionPart = handler.petParts;
            isColliding = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        PetCollisionHandler handler;
        if ((handler = other.gameObject.GetComponent<PetCollisionHandler>()) != null)
        {
            Logger.Log(handler.petParts + " is colliding");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isColliding = false;
        collisionTimer = 0f;
        petCollisionPart = PetParts.None;
    }
}
