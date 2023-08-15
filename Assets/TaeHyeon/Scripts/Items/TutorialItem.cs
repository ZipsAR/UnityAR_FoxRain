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
            InteractEventManager.NotifyTutorialItemInfo(false, true, TutorialType.Toy);
        }
        else if (transform.TryGetComponent(out Snack snack))
        {
            InteractEventManager.NotifyTutorialItemInfo(false, true, TutorialType.Snack);
        }
    }

    public void EndItemTutorial(TutorialType tutorialType)
    {
        switch (tutorialType)
        {
            case TutorialType.Toy:
                InteractEventManager.NotifyTutorialItemInfo(true, false, TutorialType.Toy);
                break;
            case TutorialType.Snack:
                InteractEventManager.NotifyTutorialItemInfo(true, false, TutorialType.Snack);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(tutorialType), tutorialType, null);
        }
    }
}
