using System;
using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using Unity.VisualScripting;
using UnityEngine;

public class PetSelectController : MonoBehaviour
{
    [SerializeField] private GameObject petSelectCanvas;

    [SerializeField] private GameObject corgiObj;
    [SerializeField] private GameObject huskyObj;
    [SerializeField] private GameObject shibaObj;
    [SerializeField] private GameObject whiteObj;
    [SerializeField] private GameObject catObj;
    [SerializeField] private GameObject bellCatObj;

    private void Start()
    {
        switch (GameManager.Instance.curPetType)
        {
            case PetType.None:
                petSelectCanvas.SetActive(true);
                break;
            case PetType.Corgi:
                InteractEventManager.NotifyPetSelected(corgiObj);
                break;
            case PetType.Husky:
                InteractEventManager.NotifyPetSelected(huskyObj);
                break;
            case PetType.Shiba:
                InteractEventManager.NotifyPetSelected(shibaObj);
                break;
            case PetType.White:
                InteractEventManager.NotifyPetSelected(whiteObj);
                break;
            case PetType.Cat:
                InteractEventManager.NotifyPetSelected(catObj);
                break;
            case PetType.BellCat:
                InteractEventManager.NotifyPetSelected(bellCatObj);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (GameManager.Instance.curPetType != PetType.None)
        {
            petSelectCanvas.SetActive(false);
        }
    }

    public void SelectPet(int petTypeInt)
    {
        petSelectCanvas.SetActive(false);
        
        switch (petTypeInt)
        {
            case (int)PetType.None:
                throw new Exception("selected pet can't be None");
            case (int)PetType.Corgi:
                InteractEventManager.NotifyPetSelected(corgiObj);
                break;
            case (int)PetType.Husky:
                InteractEventManager.NotifyPetSelected(huskyObj);
                break;
            case (int)PetType.Shiba:
                InteractEventManager.NotifyPetSelected(shibaObj);
                break;
            case (int)PetType.White:
                InteractEventManager.NotifyPetSelected(whiteObj);
                break;
            case (int)PetType.Cat:
                InteractEventManager.NotifyPetSelected(catObj);
                break;
            case (int)PetType.BellCat:
                InteractEventManager.NotifyPetSelected(bellCatObj);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(petTypeInt), petTypeInt, null);
        }

        GameManager.Instance.curPetType = (PetType)Enum.ToObject(typeof(PetType), petTypeInt);
    }
}
