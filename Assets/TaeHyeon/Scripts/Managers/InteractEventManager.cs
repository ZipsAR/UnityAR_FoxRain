using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InteractEventManager
{
    public static event EventHandler<PetStatChangedEventArgs> OnPetStatChanged;

    public static event EventHandler<PetStatInitializedArgs> OnPetStatInitialized; 
    public static void RaisePetStatChanged(PetStatBase petStatBase, PetStatNames changedStatName, int preStatAmount, int postStatAmount)
    {
        OnPetStatChanged?.Invoke(null, new PetStatChangedEventArgs(petStatBase, changedStatName, preStatAmount, postStatAmount));
    }

    public static void NotifyStatInitialized(PetStatBase petStatBase)
    {
        OnPetStatInitialized?.Invoke(null, new PetStatInitializedArgs(petStatBase));
    }
}

public class PetStatChangedEventArgs : EventArgs
{
    public PetStatBase petStatBase;
    public PetStatNames changedStatName;
    public int preStatAmount;
    public int postStatAmount;

    public PetStatChangedEventArgs(PetStatBase petStatBase, PetStatNames changedStatName, int preStatAmount, int postStatAmount)
    {
        this.petStatBase = petStatBase;
        this.changedStatName = changedStatName;
        this.preStatAmount = preStatAmount;
        this.postStatAmount = postStatAmount;
    }
}

public class PetStatInitializedArgs : EventArgs
{
    public PetStatBase petStatBase;

    public PetStatInitializedArgs(PetStatBase petStatBase)
    {
        this.petStatBase = petStatBase;
    }
}