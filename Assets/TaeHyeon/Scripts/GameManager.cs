using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Logger = ZipsAR.Logger;

public enum PlayMode
{
    None = 0,
    InteractMode = 1,
    StrollMode = 2,
    AgilityMode = 3,
}
public class GameManager : Singleton<GameManager>
{
    public GameObject ARSessions;
    
    public PlayMode currentPlayMode;
    public Player player { get; private set; }

    public InteractManager interactManager;
    public StrollManager strollManager;
    
    private void Start()
    {
        player = GameObject.Find("AR Camera").GetComponent<Player>();
        DontDestroyOnLoad(ARSessions);
    }

    public void QuitApp()
    {
        Application.Quit(0);
    }

    public bool ChangeMode(PlayMode playMode)
    {
        if (playMode == currentPlayMode)
        {
            return false;
        }

        currentPlayMode = playMode;
        return true;
    }

    public void LoadScene(String sceneName)
    {
        StartCoroutine(LoadSceneSequence(sceneName));
    }
    
    private IEnumerator LoadSceneSequence(string sceneName)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncOperation.isDone)
        {
            yield return null;
            Logger.Log("Progress : " + asyncOperation.progress);
        }

        interactManager = GameObject.Find("Interact Manager").GetComponent<InteractManager>();
        ChangeMode(PlayMode.InteractMode);
        // SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
