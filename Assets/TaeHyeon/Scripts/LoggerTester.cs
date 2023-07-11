using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class LoggerTester : MonoBehaviour
{
    [SerializeField] private TMP_Text strollLogText;
    private int logidx = 0;
    private string logMsg = "Logmessage ";

    private void Start()
    {
        StartCoroutine(ShowLogMsg());
    }

    IEnumerator ShowLogMsg()
    {
        while (logidx < 5)
        {
            strollLogText.text = logMsg + logidx;
            logidx += 1;
            yield return new WaitForSeconds(1f);
        }
        
        Application.Quit(0);
    }
}
