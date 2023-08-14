using System;
using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;
using Logger = ZipsAR.Logger;

public class HandController : MonoBehaviour
{
    public HandSide handSide;

    private void Awake()
    {
        Logger.Log("HandController attached");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.curPetType != PetType.None)
        {
            PetCollisionHandler handler;
            if ((handler = other.gameObject.GetComponent<PetCollisionHandler>()) != null)
            {
                GameManager.Instance.interactManager.PetPartCollisionEnter(handler.petParts);

                Logger.Log(handler.petParts + " start colliding");
                other.gameObject.GetComponent<MeshRenderer>().material = handler.onCollisionMat;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (GameManager.Instance.curPetType != PetType.None)
        {
            PetCollisionHandler handler;
            if ((handler = other.gameObject.GetComponent<PetCollisionHandler>()) != null)
            {
                GameManager.Instance.interactManager.PetPartCollisionExit(handler.petParts);

                Logger.Log(handler.petParts + " end colliding");
                other.gameObject.GetComponent<MeshRenderer>().material = handler.defaultMat;
            }
        }
    }
}
