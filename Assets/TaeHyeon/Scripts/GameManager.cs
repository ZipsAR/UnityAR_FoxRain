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
    private Player player;
    public Player Player
    {
        get => player;
        private set => player = value;
    }
    
    private void Start()
    {
        currentPlayMode = PlayMode.StrollMode;
        Player = GameObject.Find("AR Camera").GetComponent<Player>();
    }
}
