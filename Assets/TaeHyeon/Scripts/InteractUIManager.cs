using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Button exitBtn;
    [SerializeField] private Canvas overlayCanvas;
    [SerializeField] private PetStatBase trackingStat;

    // [SerializeField] private Slider fullnessSlider;
    // [SerializeField] private Slider cleanlinessSlider;
    // [SerializeField] private Slider tirednessSlider;

    [SerializeField] private List<StatUIRef> statUIList;
    [SerializeField] private List<Sprite> statRatingSpriteList;
    
    public Color badColor;
    public Color normalColor;
    public Color goodColor;
    
    private void Awake()
    {
        exitBtn.onClick.AddListener(GameManager.Instance.QuitApp);
        if (statRatingSpriteList.Count != Enum.GetNames(typeof(StatUIRating)).Length)
            throw new Exception("Number of statRatingImg and statRating must be the same");
    }

    private void Start()
    {
        overlayCanvas.worldCamera = GameManager.Instance.player.gameObject.GetComponent<Camera>();
        overlayCanvas.planeDistance = 0.11f;
    }

    private void Update()
    {
        // fullnessSlider.value = trackingStat.fullness / 100f;
        // cleanlinessSlider.value = trackingStat.cleanliness / 100f;
        // tirednessSlider.value = trackingStat.tiredness / 100f;

        foreach (StatUIRef statUI in statUIList)
        {
            switch (statUI.statName)
            {
                case PetStatNames.Fullness:
                    statUI.slider.value = trackingStat.fullness / 100f;
                    CheckStatRating(statUI, false);
                    break;
                case PetStatNames.Tiredness:
                    statUI.slider.value = trackingStat.tiredness / 100f;
                    CheckStatRating(statUI, true);
                    break;
                case PetStatNames.Cleanliness:
                    statUI.slider.value = trackingStat.cleanliness / 100f;
                    CheckStatRating(statUI, false);
                    break;
                // case PetStatNames.Exp:
                //     break;
                // case PetStatNames.Level:
                //     break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            
            
        }
    }

    private void CheckStatRating(StatUIRef statUIRef, bool isReversed)
    {
        if (isReversed)
        {
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

    private void CheckStatReverseRating(StatUIRef statUIRef)
    {
        
    }
}

[System.Serializable]
public class StatUIRef
{
    public PetStatNames statName;
    public Slider slider;
    public Image stateImg;
}
