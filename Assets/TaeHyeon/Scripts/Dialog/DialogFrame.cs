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
    
    private void Awake()
    {
        controller = transform.parent.GetComponent<DialogController>();
        
        nextBtn.onClick.AddListener(() => controller.OnNextBtnClicked(dialogId));
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
