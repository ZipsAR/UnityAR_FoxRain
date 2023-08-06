using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetEffectController : MonoBehaviour
{
    [SerializeField] private List<GameObject> levelEffects;
    [SerializeField] private GameObject levelUpEffect;
    
    [SerializeField] PetBase pet;
    private Transform effectTransform;
    
    private void Awake()
    {
        InteractEventManager.OnPetStatInitialized += OnPetStatInitialized;
        InteractEventManager.OnPetStatChanged += OnPetStatChanged;
        
        if (levelEffects.Count != pet.GetStat().levelMax)
            throw new Exception("level effects number must be equal with max level");

        effectTransform = pet.GetEffectPosition();
    }

    private void OnPetStatInitialized(object sender, PetStatInitializedArgs e)
    {
        ClearLevelEffects();
        AddLevelEffect(levelEffects[e.petStatBase.level - 1]);
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
