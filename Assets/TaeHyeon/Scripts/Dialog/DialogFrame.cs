using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DialogFrame : MonoBehaviour
{
    private DialogController controller;
    private int dialogId; 
    [SerializeField] private Text content;
    [SerializeField] private Button nextBtn;
    [SerializeField] private Image infoImg;
    
    // Effect
    [SerializeField] private GameObject dialogSpawnEffect; 
    
    // Sound
    [SerializeField] private AudioClip dialogSpawnClip;
    private AudioSource audioSource;
    
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = dialogSpawnClip;
        audioSource.Play();
        controller = transform.parent.GetComponent<DialogController>();

        GameObject spawnedEffect = Instantiate(dialogSpawnEffect, transform);
        spawnedEffect.transform.localScale = Vector3.one * (5 / transform.localScale.x); 
        nextBtn.onClick.AddListener(() => controller.OnNextBtnClicked(dialogId));
        nextBtn.onClick.AddListener(InteractEventManager.NotifyDialogExitClicked);
    }

    private void Update()
    {
        transform.LookAt(GameManager.Instance.player.transform);
        transform.Rotate(0,180f,0);
    }

    public void InitDialog(int id, string inputContent)
    {
        dialogId = id;
        content.text = inputContent;
    }

    public void SetImg(Sprite eInfoSprite)
    {
        if (eInfoSprite == default)
        {
            infoImg.gameObject.SetActive(false);
        }
        else
        {
            infoImg.sprite = eInfoSprite;
        }
    }
}
