using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DialogFrame : MonoBehaviour
{
    private DialogController controller;
    [SerializeField] private Text content;
    [SerializeField] private Button nextBtn;

    private void Awake()
    {
        controller = transform.parent.GetComponent<DialogController>();
        
        nextBtn.onClick.AddListener(controller.OnNextBtnClicked);
    }

    public void SetText(string inputContent)
    {
        content.text = inputContent;
    }
}
