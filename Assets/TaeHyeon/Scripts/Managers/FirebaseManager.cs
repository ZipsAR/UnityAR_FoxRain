using System;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using UnityEngine;

public class FirebaseManager : Singleton<FirebaseManager>
{
    private FirebaseApp _app;
    private DatabaseReference _reference;
    private bool isFirebaseReady;
    
    protected override void Awake()
    {
        base.Awake();
        
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available) {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                _app = FirebaseApp.DefaultInstance;
                InitializeFirebase();
                
                // Set a flag here to indicate whether Firebase is ready to use by your app.
                
            } else {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    private void InitializeFirebase()
    {
        AppOptions options = new AppOptions
            { DatabaseUrl = new Uri("https://foxrain-398a4-default-rtdb.firebaseio.com/") };
        _app = FirebaseApp.Create(options);
        // _reference = FirebaseDatabase.DefaultInstance.GetReference("pet");
        _reference = FirebaseDatabase.DefaultInstance.RootReference;

        // Check for data in child in selected location
        _reference.GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("Error occurred while fetching data");
            }
            else if (task.IsCompleted)
            {
                if (task.Result.Exists)
                {
                    // Data exists at the specific reference
                    Debug.Log("File exist");
                    isFirebaseReady = true;
                }
                else
                {
                    Debug.LogError("No data exists at the specific reference");
                }
            }
        });
    }

    public void GetPetName(Action<string> callback)
    {
        Debug.Log("GetPetName called");

        if (!isFirebaseReady)
        {
            Debug.LogError("firebase is not ready");
            return;
        }

        _reference.Child("pet").Child("name").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                callback((string)task.Result.Value);
            }
        });
    }

    public void SetPetName(string petName)
    {
        Debug.Log("SetPetName called");

        _reference.Child("pet").Child("name").SetValueAsync(petName);
    }
    
    #region Incompleteness
    
    public bool GetInformation<T>(Action<List<T>> callback) where T : new()
    {
        Debug.Log("GetInformation called");

        if (!isFirebaseReady)
        {
            Debug.LogError("firebase is not ready");
            return false;
        }
        
        var list = new List<T>();

        _reference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                // The column we added together with a corresponds to this one
                DataSnapshot snapshot = task.Result;

                foreach (DataSnapshot data in snapshot.Children)
                {
                    Debug.Log("snapshot's child value: " + data.Value);
                    IDictionary info = (IDictionary)data.Value;
                    foreach (DictionaryEntry elEntry in info)
                    {
                        Debug.Log("elEntry type : " + elEntry.Value.GetType().Name);
                        if (elEntry.Value is T value)
                        {
                            Debug.Log("elEntry.Value: " + elEntry.Value);
                            list.Add(value);
                        }
                    }
                }
                Debug.Log("here");
                foreach (var value in list)
                {
                    Debug.Log("in get data: " + value);
                }
                Debug.Log("before call back");
                callback(list);
            }
        });

        return true;
    }

    public bool SetInformation(PetStatBase stat)
    {
        Debug.Log("SetInformation called");

        if (!isFirebaseReady)
        {
            Debug.LogError("firebase is not ready");
            return false;
        }
        
        string json = JsonUtility.ToJson(stat);
        string key = _reference.Push().Key;

        _reference.Child(key).SetRawJsonValueAsync(json);

        return true;
    }
    #endregion

}
