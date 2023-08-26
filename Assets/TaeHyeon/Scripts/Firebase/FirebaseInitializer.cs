using System;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using UnityEngine;

public class FirebaseInitializer : MonoBehaviour
{
    private void Awake()
    {
        // Check if firebase is ready
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available) {
                
                Init();
                FirebaseDBManager.Instance.Init();
                FirebaseAuthManager.Instance.Init();
                
            } else {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    private void Init()
    {
        AppOptions options = new AppOptions
            { DatabaseUrl = new Uri("https://foxrain-398a4-default-rtdb.firebaseio.com/") };
        FirebaseApp.Create(options);
    }
}
