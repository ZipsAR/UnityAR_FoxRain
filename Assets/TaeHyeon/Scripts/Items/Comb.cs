using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comb : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PetCollisionHandler handler;
        if ((handler = other.gameObject.GetComponent<PetCollisionHandler>()) != null)
        {
            ZipsAR.Logger.Log(handler.petParts + " start Brushing");
            if (handler.petParts == PetParts.Body)
            {
                GameManager.Instance.interactManager.CombCollision(true);
            }
            other.gameObject.GetComponent<MeshRenderer>().material = handler.onCollisionMat;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PetCollisionHandler handler;
        if ((handler = other.gameObject.GetComponent<PetCollisionHandler>()) != null)
        {
            ZipsAR.Logger.Log(handler.petParts + " end Brushing");
            if (handler.petParts == PetParts.Body)
            {
                GameManager.Instance.interactManager.CombCollision(false);
            }
            other.gameObject.GetComponent<MeshRenderer>().material = handler.defaultMat;
        }
    }
}
