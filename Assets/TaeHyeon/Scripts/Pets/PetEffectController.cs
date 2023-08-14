using System;
using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;
using Logger = ZipsAR.Logger;

public class PetEffectController : MonoBehaviour
{
    [SerializeField] private List<GameObject> levelEffects;
    [SerializeField] private GameObject levelUpEffect;
    
    private Transform effectTransform;
    
    private void Awake()
    {
        InteractEventManager.OnPetInitializedToAll -= OnPetInitialized;
        InteractEventManager.OnPetInitializedToAll += OnPetInitialized;
        InteractEventManager.OnPetStatChanged -= OnPetStatChanged;
        InteractEventManager.OnPetStatChanged += OnPetStatChanged;
    }

    private void OnDisable()
    {
        InteractEventManager.OnPetInitializedToAll -= OnPetInitialized;
        InteractEventManager.OnPetStatChanged -= OnPetStatChanged;
    }

    private void OnPetInitialized(object sender, PetArgs e)
    {
        if (levelEffects.Count != e.petObj.GetComponent<PetBase>().GetStat().levelMax)
            throw new Exception("level effects number must be equal with max level");

        effectTransform = e.petObj.GetComponent<PetBase>().GetEffectPosition();
        
        ClearLevelEffects();
        AddLevelEffect(levelEffects[e.petObj.GetComponent<PetBase>().GetStat().level - 1]);
    }

    private void OnPetStatChanged(object sender, PetStatChangedEventArgs e)
    {
        if (e.changedStatName == PetStatNames.Level)
        {
            ClearLevelEffects();
            AddLevelEffect(levelEffects[e.postStatAmount - 1]);
            TriggerLevelUpEffect();
        }
    }
    
    private void ClearLevelEffects()
    {
        GameObject[] children = new GameObject[effectTransform.childCount];
        for (int i = 0; i < effectTransform.childCount; i++)
        {
            children[i] = effectTransform.GetChild(i).gameObject;
        }
            
        foreach (var obj in children)
        {
            Destroy(obj);
        }
    }

    private void AddLevelEffect(GameObject addedEffect)
    {
        for (int i = 0; i < effectTransform.childCount; i++)
        {
            Transform childTransform = effectTransform.GetChild(i);
                
            if (childTransform.name == addedEffect.name)
            {
                return;
            }
        }

        Instantiate(addedEffect, effectTransform);
    }

    private void TriggerLevelUpEffect()
    {
        Instantiate(levelUpEffect, effectTransform);
    }
}
