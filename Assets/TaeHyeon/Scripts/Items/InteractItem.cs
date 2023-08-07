using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractItem : MonoBehaviour
{
    [SerializeField] private GameObject instantiateEffect;

    private void Start()
    {
        Instantiate(instantiateEffect, transform);
    }
}
