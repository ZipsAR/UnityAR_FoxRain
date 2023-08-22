using System;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;

public class FirebaseDBManager : Singleton<FirebaseDBManager>
{
    private DatabaseReference _rootRef;
    private bool isFBInit;
    
    public void Init()
    {
        _rootRef = FirebaseDatabase.DefaultInstance.RootReference;
        isFBInit = true;
    }
    
    private void CheckFirebaseInitialized()
    {
        if (!isFBInit)
        {
            Debug.LogError("Firebase is not initialized yet.");
        }
    }

    // Method for determining whether or not the input path has data
    public void IsDataExistInPath(Action<bool, List<string>> callback, List<string> paths)
    {
        CheckFirebaseInitialized();

        DatabaseReference reference = _rootRef;
        
        foreach (string s in paths)
        {
            reference = reference.Child(s);
        }

        reference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                callback(task.Result.Exists, paths);
            }
        });
    }

    public void SetPetStatTo(PetStatBase stat, List<string> paths)
    {
        CheckFirebaseInitialized();
        
        DatabaseReference reference = _rootRef;
        
        foreach (string s in paths)
        {
            reference = reference.Child(s);
        }
        
        string json = JsonUtility.ToJson(stat);
        
        reference.SetRawJsonValueAsync(json);
    }

    public void GetPetStatFrom(Action<PetStatBase> callback, List<string> paths)
    {
        CheckFirebaseInitialized();

        DatabaseReference reference = _rootRef;

        foreach (string s in paths)
        {
            reference = reference.Child(s);
        }
        
        reference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                string json = snapshot.GetRawJsonValue();
                PetStatBase stat = JsonUtility.FromJson<PetStatBase>(json);
                callback(stat);
            }
        });
    }


    public void GetPetName(Action<string> callback)
    {
        Debug.Log("GetPetName called");

        CheckFirebaseInitialized();

        _rootRef.Child("pet").Child("name").GetValueAsync().ContinueWith(task =>
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

        CheckFirebaseInitialized();

        _rootRef.Child("pet").Child("name").SetValueAsync(petName);
    }
    
    #region Incompleteness
    
    public bool GetInformation<T>(Action<List<T>> callback) where T : new()
    {
        Debug.Log("GetInformation called");

        CheckFirebaseInitialized();
        
        var list = new List<T>();

        _rootRef.GetValueAsync().ContinueWith(task =>
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

        CheckFirebaseInitialized();
        
        string json = JsonUtility.ToJson(stat);
        string key = _rootRef.Push().Key;

        _rootRef.Child(key).SetRawJsonValueAsync(json);

        return true;
    }
    #endregion

}
