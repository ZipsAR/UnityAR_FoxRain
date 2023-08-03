using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Logger = ZipsAR.Logger;

public enum PlayMode4Test
{
    None = 0,
    InteractMode = 1,
    StrollMode = 2,
    AgilityMode = 3,
    CanvasTemplate = 4,
}
public class GameManager4Test : Singleton<GameManager>
{
    public GameObject ARSessions4Test;
    
    public PlayMode4Test currentPlayMode4Test;
    public Player player4Test { get; private set; }

    public InteractManager interactManager4Test;
    public StrollManager strollManager4Test;

    public HandController leftHand4Test;
    public HandController rightHand4Test;
    
    private void Start()
    {
        player4Test = GameObject.Find("AR Camera").GetComponent<Player>();
        DontDestroyOnLoad(ARSessions4Test);
    }

    public void QuitApp4Test()
    {
        Application.Quit(0);
    }

    public bool ChangeMode4Test(PlayMode4Test PlayMode4Test)
    {
        if (PlayMode4Test == currentPlayMode4Test)
        {
            return false;
        }

        currentPlayMode4Test = PlayMode4Test;
        return true;
    }

    public void LoadScene4Test(String sceneName)
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

        interactManager4Test = GameObject.Find("Interact Manager").GetComponent<InteractManager>();
        ChangeMode4Test(PlayMode4Test.InteractMode);
        // SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
