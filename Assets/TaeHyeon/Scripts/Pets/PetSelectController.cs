using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PetSelectController : MonoBehaviour
{
    [SerializeField] private GameObject catObj;
    [SerializeField] private GameObject dogObj;

    public void SelectPet(int petType)
    {
        switch (petType)
        {
            case (int)PetType.Cat:
                InteractEventManager.NotifyPetSelected(catObj);
                break;
            case (int)PetType.Dog:
                InteractEventManager.NotifyPetSelected(dogObj);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(petType), petType, null);
        }
    }
}
