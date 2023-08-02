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
    public GameObject ARSessions; // Set AR Session Related Objects to Don't Destroy Object

    public PlayMode currentPlayMode;
    public Player player { get; private set; } // Attached to AR Camera && Used to locate a user

    public InteractManager interactManager;
    public InteractAudioManager interactAudioManager;
    public StrollManager strollManager;

    public HandController leftHand;
    public HandController rightHand;
    
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

    /// <summary>
    /// Caution : Scene name must be spelled correctly because there is no exception handling part
    /// </summary>
    /// <param name="sceneName">Scene name to load</param>
    public void LoadScene(String sceneName)
    {
        StartCoroutine(LoadSceneSequence(sceneName));
    }
    
    private IEnumerator LoadSceneSequence(string sceneName)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        // Waiting for next scenes to load
        while (!asyncOperation.isDone)
        {
            yield return null;
            Logger.Log("Progress : " + asyncOperation.progress);
        }

        if (sceneName == "InteractMode")
        {
            interactManager = 
                GameObject.Find("Interact Manager").GetComponent<InteractManager>();
            interactAudioManager =
                GameObject.Find("Interact Audio Manager").GetComponent<InteractAudioManager>();
            ChangeMode(PlayMode.InteractMode);
        }
    }
}
