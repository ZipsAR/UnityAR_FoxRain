using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Endmanager : MonoBehaviour
{
    float time = 0;
    private void Update()
    {
        time += Time.deltaTime;
        if (time > 30.0f) Application.Quit();
    }
}
