using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = ZipsAR.Logger;

// create : stroll mode on enter
// destroy : stroll mode on exit
public class StrollManager : Singleton<StrollManager>
{
    [SerializeField] private StrollData strollData;
    private Player player;
    private void Start()
    {
        strollData.Init();
        player = GameObject.Find("AR Camera").GetComponent<Player>();
    }

    private void Update()
    {
        strollData.strollTime += Time.deltaTime;
    }
}
