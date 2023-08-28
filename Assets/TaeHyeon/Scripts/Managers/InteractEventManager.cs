using System;
using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;

/// <summary>
/// Initialization process
/// 1. When you select a pet, PetSelectController notify InteractManager that a pet has been selected
/// 2. When the pet is completely initialized, PetBase notify InteractManager that it is completely initialized
/// 3. InteractManager shares pet information with every Script when they confirm that the pet has been initialized
///
/// Stat change tracking process
/// 1. Notify that stats change in PetBase
/// 2. Detect changes in PetEffectController, InteractUIManager and GiftBoxSpawner to reflect changed stat values
/// </summary>
public static class InteractEventManager
{
    // Stat change event
    public static event EventHandler<PetStatChangedEventArgs> OnPetStatChanged;
    
    // Pet select event
    public static event EventHandler<PetArgs> OnPetSelected;
    
    // Pet initialization event
    public static event EventHandler<PetArgs> OnPetInitializedToManager;
    public static event EventHandler<PetArgs> OnPetInitializedToAll;

    // Dialog event
    public static event EventHandler<DialogArgs> OnDialogCall;
    public static event EventHandler OnClearDialog;
    public static event EventHandler OnClickedDialogExitBtn;

    // Tutorial event
    public static event EventHandler<TutorialItemArgs> OnGetTutorialInfo;
    
    
    // Stat changed event notifier
    public static void NotifyPetStatChanged(PetStatNames changedStatName, int preStatAmount, int postStatAmount)
        => OnPetStatChanged?.Invoke(null, new PetStatChangedEventArgs(changedStatName, preStatAmount, postStatAmount));

    // Pet selected event notifier
    public static void NotifyPetSelected(GameObject petObj)
        => OnPetSelected?.Invoke(null, new PetArgs(petObj));

    // Pet initialized event notifier
    public static void NotifyPetInitializedToManager(GameObject petObj)
        => OnPetInitializedToManager?.Invoke(null, new PetArgs(petObj));
    public static void NotifyPetInitializedToAll(GameObject petObj)
        => OnPetInitializedToAll?.Invoke(null, new PetArgs(petObj));
    
    // Dialog event notifier
    public static void NotifyDialogShow(string content, Sprite infoSprite = default, DialogOrient dialogOrient = DialogOrient.Center)
        => OnDialogCall?.Invoke(null, new DialogArgs(content, infoSprite, dialogOrient));
    public static void NotifyClearDialog()
        => OnClearDialog?.Invoke(null, null);
    public static void NotifyDialogExitClicked()
        => OnClickedDialogExitBtn?.Invoke(null, null);

    // Tutorial event notifier
    public static void NotifyTutorialItemInfo(bool isTutorialEnd, bool isGrabbed, TutorialType tutorialType)
        => OnGetTutorialInfo?.Invoke(null, new TutorialItemArgs(isTutorialEnd, isGrabbed, tutorialType));
}

public class PetStatChangedEventArgs : EventArgs
{
    public PetStatNames changedStatName;
    public int preStatAmount;
    public int postStatAmount;

    public PetStatChangedEventArgs(PetStatNames changedStatName, int preStatAmount, int postStatAmount)
    {
        this.changedStatName = changedStatName;
        this.preStatAmount = preStatAmount;
        this.postStatAmount = postStatAmount;
    }
}

public class PetArgs : EventArgs
{
    public GameObject petObj;

    public PetArgs(GameObject petObj)
    {
        this.petObj = petObj;
    }
}

public class DialogArgs : EventArgs
{
    public string content;
    public Sprite infoSprite;
    public DialogOrient dialogOrient;
    
    public DialogArgs(string content, Sprite infoSprite, DialogOrient dialogOrient)
    {
        this.content = content;
        this.infoSprite = infoSprite;
        this.dialogOrient = dialogOrient;
    }
}

public class TutorialItemArgs : EventArgs
{
    public bool isTutorialEnd;
    public bool isGrabbed;
    public TutorialType TutorialType;
    
    public TutorialItemArgs(bool isTutorialEnd, bool isGrabbed, TutorialType tutorialType)
    {
        this.isTutorialEnd = isTutorialEnd;
        this.isGrabbed = isGrabbed;
        this.TutorialType = tutorialType;
    }
}