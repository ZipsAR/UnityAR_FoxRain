using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Logger = ZipsAR.Logger;

public class LoggerTester : Singleton<LoggerTester>
{
    [SerializeField] private TMP_Text countdownText;
    private int logIdx;
    private int autoQuitTime = 60;
    private string countdownMsg = "quit app in ";
    private string logMsg;
    
    private void Start()
    {
        StartCoroutine(ShowCountdownMsg());
        logIdx = autoQuitTime;
        logMsg = "start reset";
    }

    IEnumerator ShowCountdownMsg()
    {
        while (logIdx >= 0)
        {
            countdownText.text = countdownMsg + logIdx;
            logIdx -= 1;
            yield return new WaitForSeconds(1f);
        }
        
        Application.Quit(0);
    }
}
