using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = ZipsAR.Logger;

public class Player : MonoBehaviour
{
    public GameObject cameraObj;
    private void FixedUpdate()
    {
        Logger.Log("Origin : " + transform.position);
        Logger.Log("Camera : " + cameraObj.transform.position);
    }
}
