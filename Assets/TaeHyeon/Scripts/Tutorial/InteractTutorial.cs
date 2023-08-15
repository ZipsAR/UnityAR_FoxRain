using System;
using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;

public class InteractTutorial : MonoBehaviour
{
    [SerializeField] private GameObject table;
    [SerializeField] private GameObject toys;
    [SerializeField] private GameObject snacks;

    [SerializeField] private Transform tableSpawnPos;
    private Transform itemSpawnPos;
    private GameObject spawnedTable;
    private GameObject activeItems;
    private ItemType curTutorialItemType;

    private void Awake()
    {
        curTutorialItemType = ItemType.Toy;
    }

    private void OnEnable()
    {
        InteractEventManager.OnPetInitializedToAll -= OnPetInitialized;
        InteractEventManager.OnPetInitializedToAll += OnPetInitialized;
        InteractEventManager.OnGetTutorialInfo -= GetTutorialInfo;
        InteractEventManager.OnGetTutorialInfo += GetTutorialInfo;
    }

    private void OnDisable()
    {
        InteractEventManager.OnPetInitializedToAll -= OnPetInitialized;
        InteractEventManager.OnGetTutorialInfo -= GetTutorialInfo;
    }

    private void OnPetInitialized(object sender, PetArgs e)
    {
        InteractEventManager.NotifyClearDialog();

        Vector3 absoluteTablePos =
            new Vector3(tableSpawnPos.position.x, GameData.floorHeight, tableSpawnPos.position.z); 
        spawnedTable = Instantiate(table, absoluteTablePos, tableSpawnPos.rotation);
        itemSpawnPos = spawnedTable.GetComponent<TutorialDesk>().itemSpawnPosition;
        activeItems = Instantiate(toys, itemSpawnPos.position, itemSpawnPos.rotation);
        
        InteractEventManager.NotifyDialogShow("귀여운 장난감을 잡아서 펫과 놀아보세요!");
    }

    private void GetTutorialInfo(object sender, TutorialItemArgs e)
    {
        // Grab toy
        if (!e.isTutorialEnd && e.isGrabbed && e.itemType == ItemType.Toy)
        {
            if (activeItems != null)
            {
                Destroy(activeItems);
            }
            return;
        }
        
        // End toy tutorial
        if (e.isTutorialEnd && !e.isGrabbed && e.itemType == ItemType.Toy)
        {
            InteractEventManager.NotifyClearDialog();
            InteractEventManager.NotifyDialogShow("맛있는 음식을 펫에게 제공하세요!");
            
            activeItems = Instantiate(snacks, itemSpawnPos.position, itemSpawnPos.rotation);
            return;
        }
        
        // Start snack tutorial
        if (!e.isTutorialEnd && e.isGrabbed && e.itemType == ItemType.Snack)
        {
            if (activeItems != null)
            {
                Destroy(activeItems);
            }
            return;
        }
        
        // End snack tutorial
        if (e.isTutorialEnd && !e.isGrabbed && e.itemType == ItemType.Snack)
        {
            InteractEventManager.NotifyClearDialog();
            InteractEventManager.NotifyDialogShow("펫의 머리를 쓰다듬어 칭찬해주세요!");
            return;
        }
        
        
    }
    
    
    public void StartTutorial()
    {
        // if player select pet, then "OnPetInitialized" is called
        InteractEventManager.NotifyDialogShow("함께할 펫을 선택해주세요!");
    }
    
}
