using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = ZipsAR.Logger;

public class CubeSpawner : MonoBehaviour
{
    public GameObject spawnObject;
    public GameObject cameraObj;

    private void Update()
    {
        UsePhys();
    }

    private void UsePhys()
    {   
        Ray ray = new Ray(cameraObj.transform.position, cameraObj.transform.forward);
        RaycastHit hitData;

        if (Physics.Raycast(ray, out hitData, maxDistance: 1000f, layerMask: LayerMask.GetMask("Water")))
        {
            Logger.Log("use physics: " + hitData.point);
            Instantiate(spawnObject, hitData.point, Quaternion.identity);
        }
        else
        {
            Logger.Log("not detected physics");
        }
    }

}
