using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandUIController : MonoBehaviour
{
    public GameObject handCanvasObj;

    private void Awake()
    {
        handCanvasObj.GetComponent<RectTransform>().localScale = new Vector3(0.001f, 0.001f, 0.001f);
    }
}
