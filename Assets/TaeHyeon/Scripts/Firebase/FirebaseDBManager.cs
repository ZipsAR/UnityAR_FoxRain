using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;
using Logger = ZipsAR.Logger;

public class FirebaseDBManager : Singleton<FirebaseDBManager>
{
    private DatabaseReference _rootRef;
    private bool _isFbInit;
    
    public void Init()
    {
        _rootRef = FirebaseDatabase.DefaultInstance.RootReference;
        _isFbInit = true;
    }

    // Method for determining whether or not the input path has data
    public async void IsDataExistInAsync(List<string> paths, Action<bool, List<string>> callback)
    {
        Logger.Log("IsDataExistInAsync start");

        if (!_isFbInit)
        {
            Logger.LogError("Firebase is not initialized yet");
            callback(false, default);
            return;
        }
        
        try
        {
            DatabaseReference reference = _rootRef;

            foreach (string s in paths)
            {
                reference = reference.Child(s);
            }
            
            DataSnapshot snapshot = await reference.GetValueAsync();

            callback(snapshot.Exists, paths);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error while check data in path: {ex.Message}");
            callback(false, default);
        }

    }

    public async void LoadDataFromAsync<T>(List<string> paths, Action<bool, T> callback)
    {
        Logger.Log("LoadDataFromAsync start");

        if (!_isFbInit)
        {
            Logger.LogError("Firebase is not initialized yet");
            callback(false, default);
            return;
        }

        try
        {
            DatabaseReference reference = _rootRef;
            foreach (string s in paths)
            {
                reference = reference.Child(s);
            }

            DataSnapshot snapshot = await reference.GetValueAsync();

            if (snapshot.Exists)
            {
                string json = snapshot.GetRawJsonValue();
                T data = JsonUtility.FromJson<T>(json);
                callback(true, data);
            }
            else
            {
                // data is not found
                callback(false, default);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error while loading data: {ex.Message}");
            callback(false, default);
        }
    }
    
    public async void SaveDataToAsync(List<string> paths, object data)
    {
        Logger.Log("SaveDataToAsync start");

        if (!_isFbInit)
        {
            Logger.LogError("Firebase is not initialized yet");
            return;
        }
        
        try
        {
            DatabaseReference reference = _rootRef;

            foreach (string s in paths)
            {
                reference = reference.Child(s);
            }

            string json = JsonUtility.ToJson(data);

            await reference.SetRawJsonValueAsync(json);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error while saving data: {ex.Message}");
        }
    }
}
