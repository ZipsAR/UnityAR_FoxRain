using System;
using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Logger = ZipsAR.Logger;
using PlayMode = EnumTypes.PlayMode;

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

    public PetType curPetType;
    
    private void Start()
    {
        player = GameObject.Find("AR Camera").GetComponent<Player>();
        DontDestroyOnLoad(ARSessions);
        curPetType = PetType.None;
        // InteractEventManager.NotifyDialogShow("왼손바닥을 바라보면\n여우비의 메뉴창을 불러올 수 있어요.");
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
            PlacementSystem.Instance.ProtectGrib();
            MapInfo.Instance.CatchObjectInitialize();
            MapInfo.Instance.SetInvisiblemode();
           
            interactManager = 
                GameObject.Find("Interact Manager").GetComponent<InteractManager>();
            interactAudioManager =
                GameObject.Find("Interact Audio Manager").GetComponent<InteractAudioManager>();
            ChangeMode(PlayMode.InteractMode);
        }
        if(sceneName == "HousingMode"){
            MapInfo.Instance.SetMapHousingmode();
            MapInfo.Instance.SetOrigin();
            MapInfo.Instance.SetReScale(16);
            MapInfo.Instance.MapUnGrabmode();
        }
        if(sceneName == "StoreScene")
        {
            MapInfo.Instance.CatchObjectInitialize();
            MapInfo.Instance.SetInvisiblemode();
        }
        
    }
}
