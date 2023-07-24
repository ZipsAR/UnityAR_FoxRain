using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum PlayMode
{
    InteractMode = 0,
    StrollMode = 1,
    AgilityMode = 2,
}
public class GameManager : Singleton<GameManager>
{
    public PlayMode currentPlayMode;
    public Player player { get; private set; }

    private void Start()
    {
        player = GameObject.Find("AR Camera").GetComponent<Player>();
    }
}
