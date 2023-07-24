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
    private float collisionTimeLimit;
    private PetParts petCollisionPart;

    private void Start()
    {
        collisionTimer = 0f;
        isColliding = false;
        collisionTimeLimit = 2f;
    }

    private void Update()
    {
        switch (GameManager.Instance.currentPlayMode)
        {
            case PlayMode.InteractMode:
                if (!isColliding) return;
        
                collisionTimer += Time.deltaTime;

                if (collisionTimer > collisionTimeLimit)
                {
                    isColliding = false;
                    collisionTimer = 0f;
                    
                    Logger.Log("touch in 2 seconds");
                    switch (petCollisionPart)
                    {
                        case PetParts.Head:
                            InteractManager.Instance.interactHeadEvent();
                            break;
                        case PetParts.Jaw:
                            InteractManager.Instance.interactJawEvent();
                            break;
                        case PetParts.Body:
                            InteractManager.Instance.interactBodyEvent();
                            break;
                    }
                
                }
                break;
        }
        
    }
    

    private void OnTriggerEnter(Collider other)
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
