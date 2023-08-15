using System;
using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;
using Random = UnityEngine.Random;

public class GiftBoxSpawner : MonoBehaviour
{
    [SerializeField] private GameObject giftBoxObj;
    [SerializeField] private GameObject coinObj;
    [SerializeField] private ItemDatabase itemDatabase;
    private int coinEarnedValue;
    
    private void Awake()
    {
        coinEarnedValue = 1500;
        InteractEventManager.OnPetStatChanged += SpawnGiftBox;
    }

    private void SpawnGiftBox(object sender, PetStatChangedEventArgs e)
    {
        if(e.changedStatName != PetStatNames.Level) return;

        Vector3 curPetPos = GameManager.Instance.interactManager.GetCurPet().transform.position;
        Vector3 playerPos = GameManager.Instance.player.transform.position;
        InteractData data = GameManager.Instance.interactManager.GetInteractData();

        curPetPos = new Vector3(curPetPos.x, GameData.floorHeight, curPetPos.z);
        playerPos = new Vector3(playerPos.x, GameData.floorHeight, playerPos.z);
        
        // Box Position
        Vector3 playerToPetRight = (Quaternion.AngleAxis(90f,Vector3.up) * (curPetPos - playerPos)).normalized;
        Vector3 spawnPos = Utils.GetPointBeforeDistance(curPetPos, playerPos, GameData.playerFrontDistance);
        spawnPos += playerToPetRight * GameData.playerFrontDistance;
        
        // Box Rotation
        Quaternion spawnRotation = Quaternion.LookRotation(-(playerPos - spawnPos));
        
        // Instantiate
        GameObject spawnedObj = Instantiate(giftBoxObj, spawnPos, spawnRotation);
        
        GiftBox giftBox = spawnedObj.GetComponent<GiftBox>();
        giftBox.SetCoinEarnedValue(coinEarnedValue);
        
        // Set content of gift
        giftBox.SpawnGift(GetCoinObj());
    }

    private GameObject GetRandomItem()
    {
        if (itemDatabase.ItemData.Count <= 0) return null;
        ItemData data;

        do
        {
            int randomIdx = Random.Range(0, itemDatabase.ItemData.Count);
            data = itemDatabase.ItemData[randomIdx];    
        } while (data.itemCategory == ItemData.ItemCategory.Funiture);
        
        return data.Prefab;
    }

    private GameObject GetCoinObj() => coinObj;

}
