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

    private void Awake()
    {
        collisionTimer = 0f;
        isColliding = false;
        collisionTimeLimit = 2f;
        Logger.Log("HandController attached");
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
                            Logger.Log("Head Interaction start in HandController");
                            break;
                        case PetParts.Jaw:
                            InteractManager.Instance.interactJawEvent();
                            Logger.Log("Jaw Interaction start in HandController");
                            break;
                        case PetParts.Body:
                            InteractManager.Instance.interactBodyEvent();
                            Logger.Log("Body Interaction start in HandController");
                            break;
                        case PetParts.HandDetection:
                            InteractManager.Instance.interactHandDetectionEvent();
                            Logger.Log( "HandDetection Interaction start in HandController");
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
            other.gameObject.GetComponent<MeshRenderer>().material = handler.onCollisionMat;
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
        PetCollisionHandler handler;
        if ((handler = other.gameObject.GetComponent<PetCollisionHandler>()) != null)
        {
            isColliding = false;
            collisionTimer = 0f;
            petCollisionPart = PetParts.None;
            other.gameObject.GetComponent<MeshRenderer>().material = handler.detaultMat;
            Logger.Log(handler.petParts + " exit colliding");
        }
    }
}
