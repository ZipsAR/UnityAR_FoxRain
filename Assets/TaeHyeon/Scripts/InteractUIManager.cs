using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractUIManager : MonoBehaviour
{
    [SerializeField] private Button exitBtn;

    private void Awake()
    {
        exitBtn.onClick.AddListener(GameManager.Instance.QuitApp);
    }
}
