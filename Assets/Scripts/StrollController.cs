using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = ZipsAR.Logger;

public class StrollController : MonoBehaviour
{
    private Transform userTransform;
    private Camera userCamera;
    private void Awake()
    {
        if (Camera.main != null)
        {
            userCamera = Camera.main;
            Logger.Log("main camera exist");
        }
        else
        {
            Logger.Log("main camera is null");
        }
    }

    private void Update()
    {
        userTransform = userCamera.transform;
        Logger.Log(userTransform.position);
    }
}
