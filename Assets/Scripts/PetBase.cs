using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PetBase : MonoBehaviour
{
    [SerializeField] protected PetStatBase stat;
    protected Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        switch (GameManager.Instance.currentMode)
        {
            case GameManager.Mode.StrollMode:
                StrollModeUpdate();
                break;
        }
    }

    private void StrollModeUpdate()
    {
        
    }
}
