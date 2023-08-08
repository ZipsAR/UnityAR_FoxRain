using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetSelectController : MonoBehaviour
{
    [SerializeField] private GameObject petSelectCanvas;
    
    [SerializeField] private GameObject catObj;
    [SerializeField] private GameObject dogObj;
    
    private void Start()
    {
        switch (GameManager.Instance.curPetType)
        {
            case PetType.None:
                petSelectCanvas.SetActive(true);
                break;
            case PetType.Cat:
                petSelectCanvas.SetActive(false);
                InteractEventManager.NotifyPetSelected(catObj);
                break;
            case PetType.Dog:
                petSelectCanvas.SetActive(false);
                InteractEventManager.NotifyPetSelected(dogObj);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SelectPet(int petType)
    {
        petSelectCanvas.SetActive(false);
        
        switch (petType)
        {
            case (int)PetType.Cat:
                InteractEventManager.NotifyPetSelected(catObj);
                GameManager.Instance.curPetType = PetType.Cat;
                break;
            case (int)PetType.Dog:
                InteractEventManager.NotifyPetSelected(dogObj);
                GameManager.Instance.curPetType = PetType.Dog;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(petType), petType, null);
        }
    }
}
