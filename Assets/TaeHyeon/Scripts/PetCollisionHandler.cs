using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = ZipsAR.Logger;

[RequireComponent(typeof(Collider))]
public class PetCollisionHandler : MonoBehaviour
{
    public PetParts petParts;
    
    private void Start()
    {
        transform.GetComponent<Collider>().isTrigger = true;
    }
}
