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

        Vector3 curPetPos = GameManager.Instance.interactManager.GetCurPet().transform.position;
        Vector3 playerPos = GameManager.Instance.player.transform.position;
        InteractData data = GameManager.Instance.interactManager.GetInteractData();

        curPetPos = new Vector3(curPetPos.x, data.floorHeight, curPetPos.z);
        playerPos = new Vector3(playerPos.x, data.floorHeight, playerPos.z);
        
        Vector3 spawnPos = GetPointBeforeDistance(curPetPos, playerPos, data.playerFrontDistance);

        Quaternion spawnRotation = Quaternion.LookRotation(playerPos - spawnPos);
        GameObject spawnedObj = Instantiate(giftBoxObj, spawnPos, spawnRotation);
        GiftBox giftBox = spawnedObj.GetComponent<GiftBox>();
        
        giftBox.SetGift(sampleGift);
    }
    
    
    private Vector3 GetPointBeforeDistance(Vector3 startPoint, Vector3 endPoint, float beforeDistance)
    {
        float distance = Vector3.Distance(startPoint, endPoint);
        float x = endPoint.x - (beforeDistance / distance) * (endPoint.x - startPoint.x);
        float y = startPoint.y;
        float z = endPoint.z - (beforeDistance / distance) * (endPoint.z - startPoint.z);

        return new Vector3(x, y, z);
    }
}
