using System;
using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;
using UnityEngine.Serialization;
using Logger = ZipsAR.Logger;

[RequireComponent(typeof(Collider))]
public class PetCollisionHandler : MonoBehaviour
{
    public PetParts petParts; // The part of the pet that this collider means

    public Material defaultMat;
    public Material onCollisionMat;
    
    private void Start()
    {
        transform.GetComponent<Collider>().isTrigger = true;
    }
    
}
