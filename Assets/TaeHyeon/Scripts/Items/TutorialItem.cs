using System;
using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TutorialItem : MonoBehaviour
{
    private XRGrabInteractable xrGrabInteractable;
     
    private void Awake()
    {
        xrGrabInteractable = GetComponent<XRGrabInteractable>();
        xrGrabInteractable.selectEntered.AddListener(SelectedEntered);
    }

    private void SelectedEntered(SelectEnterEventArgs arg0)
    {
        if (transform.TryGetComponent(out Toy toy))
        {
            InteractEventManager.NotifyTutorialItemInfo(false, true, ItemType.Toy);
        }
        else if (transform.TryGetComponent(out Snack snack))
        {
            InteractEventManager.NotifyTutorialItemInfo(false, true, ItemType.Snack);
        }
    }

    public void EndItemTutorial(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Toy:
                InteractEventManager.NotifyTutorialItemInfo(true, false, ItemType.Toy);
                break;
            case ItemType.Snack:
                InteractEventManager.NotifyTutorialItemInfo(true, false, ItemType.Snack);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null);
        }
    }
}
