using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public enum Mode
    {
        InteractMode,
        StrollMode,
        AgilityMode,
    }

    public Mode currentMode;

    private void Start()
    {
        currentMode = Mode.StrollMode;
    }
    
    
}
