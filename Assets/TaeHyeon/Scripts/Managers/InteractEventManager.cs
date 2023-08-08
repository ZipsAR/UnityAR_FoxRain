using System;
using System.Collections;
using System.Collections.Generic;
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
    public static event EventHandler<PetStatChangedEventArgs> OnPetStatChanged;

    public static event EventHandler<PetArgs> OnPetSelected;
    
    public static event EventHandler<PetArgs> OnPetInitializedToManager;

    public static event EventHandler<PetArgs> OnPetInitializedToAll;

    public static void NotifyPetStatChanged(PetStatNames changedStatName, int preStatAmount, int postStatAmount)
    {
        OnPetStatChanged?.Invoke(null, new PetStatChangedEventArgs(changedStatName, preStatAmount, postStatAmount));
    }

    public static void NotifyPetSelected(GameObject petObj)
    {
        OnPetSelected?.Invoke(null, new PetArgs(petObj));
    }

    public static void NotifyPetInitializedToManager(GameObject petObj)
    {
        OnPetInitializedToManager?.Invoke(null, new PetArgs(petObj));
    }
    
    public static void NotifyPetInitializedToAll(GameObject petObj)
    {
        OnPetInitializedToAll?.Invoke(null, new PetArgs(petObj));
    }
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