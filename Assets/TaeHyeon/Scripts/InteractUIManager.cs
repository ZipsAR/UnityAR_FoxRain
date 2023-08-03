using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logger = ZipsAR.Logger;

public class InteractUIManager : MonoBehaviour
{
    [SerializeField] private Button exitBtn;
    [SerializeField] private Canvas overlayCanvas;
    [SerializeField] private PetStatBase trackingStat;

    [SerializeField] private Slider fullnessSlider;
    [SerializeField] private Slider cleanlinessSlider;
    [SerializeField] private Slider tirednessSlider;
    
    private void Awake()
    {
        exitBtn.onClick.AddListener(GameManager.Instance.QuitApp);
    }

    private void Start()
    {
        overlayCanvas.worldCamera = GameManager.Instance.player.gameObject.GetComponent<Camera>();
        overlayCanvas.planeDistance = 0.11f;
    }

    private void Update()
    {
        fullnessSlider.value = trackingStat.fullness / 100f;
        cleanlinessSlider.value = trackingStat.cleanliness / 100f;
        tirednessSlider.value = trackingStat.tiredness / 100f;
    }
}
