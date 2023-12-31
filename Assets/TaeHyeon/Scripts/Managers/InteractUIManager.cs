using System;
using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Logger = ZipsAR.Logger;

public enum StatUIRating
{
    Bad = 30,
    Normal = 70,
    Good = 100,
}

public class InteractUIManager : MonoBehaviour
{
    // UI Canvas
    [SerializeField] private GameObject uiCanvas;
    
    // Stats
    [SerializeField] private Canvas overlayCanvas;
    [SerializeField] private List<StatUIRef> statUIList;
    [SerializeField] private List<Sprite> statRatingSpriteList;
    
    // Color
    public Color badColor;
    public Color normalColor;
    public Color goodColor;
    
    // Level
    public GameObject star;
    public Transform starContainer;
    
    // Exp
    public Slider expSlider;
    
    // Gold
    public Text moneyText;
    
    private void Awake()
    {
        InteractEventManager.OnPetInitializedToAll -= OnPetInitialized;
        InteractEventManager.OnPetInitializedToAll += OnPetInitialized;
        InteractEventManager.OnPetStatChanged -= OnPetStatChanged;
        InteractEventManager.OnPetStatChanged += OnPetStatChanged;
        
        if (statRatingSpriteList.Count != Enum.GetNames(typeof(StatUIRating)).Length)
            throw new Exception("Number of statRatingImg and statRating must be the same");

        SetUIDefault();
    }

    private void Start()
    {
        // Canvas Setting
        overlayCanvas.worldCamera = GameManager.Instance.player.gameObject.GetComponent<Camera>();
        overlayCanvas.planeDistance = 0.11f;
    }

    private void SetUIDefault()
    {
        uiCanvas.SetActive(false);
        
        ClearStars();
        expSlider.value = 0;
        foreach (StatUIRef statUIRef in statUIList)
        {
            statUIRef.slider.value = 0;
            statUIRef.stateImg.sprite = statRatingSpriteList[2];
            statUIRef.stateImg.color = new Color(goodColor.r, goodColor.g, goodColor.b, 1f);
        }
    }

    private void OnDisable()
    {
        InteractEventManager.OnPetInitializedToAll -= OnPetInitialized;
        InteractEventManager.OnPetStatChanged -= OnPetStatChanged;
    }

    private void OnPetInitialized(object sender, PetArgs e)
    {
        uiCanvas.SetActive(true);
        
        PetStatBase initializedPetStat = e.petObj.GetComponent<PetBase>().GetStat();

        // Level
        ClearStars();
        InsertStars(initializedPetStat.level);
        
        // Exp
        expSlider.value = initializedPetStat.exp / 100f;
        
        // Gold
        moneyText.text = FileIOSystem.Instance.invendatabase.money.ToString();
        
        // Stat
        foreach (StatUIRef statUIRef in statUIList)
        {
            switch (statUIRef.statName)
            {
                case PetStatNames.Fullness:
                    statUIRef.slider.value = initializedPetStat.fullness / 100f;
                    CheckStatRating(statUIRef, false);
                    break;
                case PetStatNames.Tiredness:
                    statUIRef.slider.value = initializedPetStat.tiredness / 100f;
                    CheckStatRating(statUIRef, true);
                    break;
                case PetStatNames.Cleanliness:
                    statUIRef.slider.value = initializedPetStat.cleanliness / 100f;
                    CheckStatRating(statUIRef, false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    private void OnPetStatChanged(object sender, PetStatChangedEventArgs e)
    {
        StatUIRef changeUIRef;

        switch (e.changedStatName)
        {
            // Level
            case PetStatNames.Level:
                ClearStars();
                InsertStars(e.postStatAmount);
                return;
            
            // Exp
            case PetStatNames.Exp:
                expSlider.value = e.postStatAmount / 100f;
                return;
            
            case PetStatNames.Money:
                moneyText.text = e.postStatAmount.ToString();
                return;
            
            default:
                // Stat
                switch (e.changedStatName)
                {
                    case PetStatNames.Fullness:
                        changeUIRef = statUIList[(int)PetStatNames.Fullness]; 
                        if (changeUIRef.statName != PetStatNames.Fullness)
                            throw new Exception("Stat trying to change does not match the current ui");
                
                        changeUIRef.slider.value = e.postStatAmount / 100f; 
                        CheckStatRating(changeUIRef, false);
                        break;
            
                    case PetStatNames.Tiredness:
                        changeUIRef = statUIList[(int)PetStatNames.Tiredness]; 
                        if (changeUIRef.statName != PetStatNames.Tiredness)
                            throw new Exception("Stat trying to change does not match the current ui");
                
                        changeUIRef.slider.value = e.postStatAmount / 100f; 
                        CheckStatRating(changeUIRef, true);
                        break;
            
                    case PetStatNames.Cleanliness:
                        changeUIRef = statUIList[(int)PetStatNames.Cleanliness]; 
                        if (changeUIRef.statName != PetStatNames.Cleanliness)
                            throw new Exception("Stat trying to change does not match the current ui");
                
                        changeUIRef.slider.value = e.postStatAmount / 100f; 
                        CheckStatRating(changeUIRef, false);
                        break;
                    // case PetStatNames.Exp:
                    //     break;
                    // case PetStatNames.Level:
                    //     break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                break;
        }
    }
    
    private void CheckStatRating(StatUIRef statUIRef, bool isReversed)
    {
        if (isReversed)
        {
            // Tiredness
            switch (statUIRef.slider.value * 100f)
            {
                case < (int)StatUIRating.Bad:
                    statUIRef.stateImg.sprite = statRatingSpriteList[2];
                    statUIRef.stateImg.color = new Color(goodColor.r, goodColor.g, goodColor.b, 1f);
                    break;
                case < (int)StatUIRating.Normal:
                    statUIRef.stateImg.sprite = statRatingSpriteList[1];
                    statUIRef.stateImg.color = new Color(normalColor.r, normalColor.g, normalColor.b, 1f);
                    break;
                default:
                    statUIRef.stateImg.sprite = statRatingSpriteList[0];
                    statUIRef.stateImg.color = new Color(badColor.r, badColor.g, badColor.b, 1f);
                    break;
            }
        }
        else
        {
            // Cleanliness, Fullness
            switch (statUIRef.slider.value * 100f)
            {
                case < (int)StatUIRating.Bad:
                    statUIRef.stateImg.sprite = statRatingSpriteList[0];
                    statUIRef.stateImg.color = new Color(badColor.r, badColor.g, badColor.b, 1f);
                    break;
                case < (int)StatUIRating.Normal:
                    statUIRef.stateImg.sprite = statRatingSpriteList[1];
                    statUIRef.stateImg.color = new Color(normalColor.r, normalColor.g, normalColor.b, 1f);
                    break;
                default:
                    statUIRef.stateImg.sprite = statRatingSpriteList[2];
                    statUIRef.stateImg.color = new Color(goodColor.r, goodColor.g, goodColor.b, 1f);
                    break;
            }
        }
    }

    private void ClearStars()
    {
        int childCount = starContainer.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            GameObject child = starContainer.GetChild(i).gameObject;
            Destroy(child);
        }
    }

    private void InsertStars(int num)
    {
        for (int i = 0; i < num; i++)
        {
            Instantiate(star, starContainer);
        }
    }
}

[System.Serializable]
public class StatUIRef
{
    public PetStatNames statName;
    public Slider slider;
    public Image stateImg;
}
