using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftBoxSpawner : MonoBehaviour
{
    [SerializeField] private GameObject giftBoxObj;
    [SerializeField] private GameObject sampleGift;
    private void Awake()
    {
        InteractEventManager.OnPetStatChanged += SpawnGiftBox;
    }

    private void SpawnGiftBox(object sender, PetStatChangedEventArgs e)
    {
        if(e.changedStatName != PetStatNames.Level) return;
        
        GameObject spawnedObj = Instantiate(giftBoxObj, Vector3.zero, Quaternion.identity);
        GiftBox giftBox = spawnedObj.GetComponent<GiftBox>();
        
        giftBox.SetGift(sampleGift);
    }
}
