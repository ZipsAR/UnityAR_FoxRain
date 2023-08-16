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
    private TutorialType curTutorialType;

    [SerializeField] private Sprite grabSprite;
    
    private void Awake()
    {
        curTutorialType = TutorialType.Toy;
    }

    private void Start()
    {
        if (GameManager.Instance.curPetType == PetType.None)
        {
            StartTutorial();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        InteractEventManager.OnPetInitializedToAll -= OnPetInitialized;
        InteractEventManager.OnPetInitializedToAll += OnPetInitialized;
        InteractEventManager.OnGetTutorialInfo -= GetTutorialInfo;
        InteractEventManager.OnGetTutorialInfo += GetTutorialInfo;
        InteractEventManager.OnPetStatChanged -= OnPetStatChanged;
        InteractEventManager.OnPetStatChanged += OnPetStatChanged;
    }

    private void OnDisable()
    {
        InteractEventManager.OnPetInitializedToAll -= OnPetInitialized;
        InteractEventManager.OnGetTutorialInfo -= GetTutorialInfo;
        InteractEventManager.OnPetStatChanged -= OnPetStatChanged;
    }

    private void OnPetInitialized(object sender, PetArgs e)
    {
        InteractEventManager.NotifyClearDialog();

        Vector3 absoluteTablePos =
            new Vector3(tableSpawnPos.position.x, GameData.floorHeight, tableSpawnPos.position.z); 
        spawnedTable = Instantiate(table, absoluteTablePos, tableSpawnPos.rotation);
        itemSpawnPos = spawnedTable.GetComponent<TutorialDesk>().itemSpawnPosition;
        activeItems = Instantiate(toys, itemSpawnPos.position, itemSpawnPos.rotation);
        
        InteractEventManager.NotifyDialogShow("귀여운 장난감을\n잡아서 펫과 놀아보세요!\nGrab모션으로 잡을 수 있어요!", grabSprite);
    }

    private void GetTutorialInfo(object sender, TutorialItemArgs e)
    {
        // Grab toy
        if (!e.isTutorialEnd && e.isGrabbed && e.TutorialType == TutorialType.Toy)
        {
            if (activeItems != null)
            {
                Destroy(activeItems);
                Destroy(spawnedTable);
            }
            return;
        }
        
        // End toy tutorial
        if (e.isTutorialEnd && !e.isGrabbed && e.TutorialType == TutorialType.Toy)
        {
            InteractEventManager.NotifyClearDialog();
            InteractEventManager.NotifyDialogShow("맛있는 음식을 펫에게 제공하세요!");

            Vector3 absoluteTablePos =
                new Vector3(tableSpawnPos.position.x, GameData.floorHeight, tableSpawnPos.position.z);
            spawnedTable = Instantiate(table, absoluteTablePos, tableSpawnPos.rotation);
            itemSpawnPos = spawnedTable.GetComponent<TutorialDesk>().itemSpawnPosition;
            activeItems = Instantiate(snacks, itemSpawnPos.position, itemSpawnPos.rotation);
            return;
        }
        
        // Grab snack
        if (!e.isTutorialEnd && e.isGrabbed && e.TutorialType == TutorialType.Snack)
        {
            if (activeItems != null)
            {
                Destroy(activeItems);
                Destroy(spawnedTable);
            }
            return;
        }
        
        // End snack tutorial
        if (e.isTutorialEnd && !e.isGrabbed && e.TutorialType == TutorialType.Snack)
        {
            InteractEventManager.NotifyClearDialog();
            InteractEventManager.NotifyDialogShow("이제 튜토리얼이 모두 끝났어요!!\n펫의 머리를 쓰다듬어 칭찬해주세요!\n나만의 펫 하우스도 꾸며볼까요?");
            return;
        }
    }
    
    private void OnPetStatChanged(object sender, PetStatChangedEventArgs e)
    {
        if (e.changedStatName == PetStatNames.Level && e.postStatAmount == 2)
        {
            InteractEventManager.NotifyDialogShow("레벨업에 성공했어요!\n상자를 열어 골드를 획득해요", dialogOrient: DialogOrient.Right);
        }
    }
    
    private void StartTutorial()
    {
        // if player select pet, then "OnPetInitialized" is called
        InteractEventManager.NotifyDialogShow("함께할 펫을 선택해주세요!");
    }
    
}
