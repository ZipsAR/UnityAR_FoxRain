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
}
