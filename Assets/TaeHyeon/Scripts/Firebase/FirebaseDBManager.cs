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
    public void IsDataExistInPath(List<string> paths, Action<bool, List<string>> callback)
    {
        try
        {
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
        catch (Exception ex)
        {
            Logger.LogError($"Error while check data in path: {ex.Message}");
        }

    }

    #region Load
    
    private async Task<T> LoadDataFromPrivateAsync<T>(List<string> paths)
    {
        try
        {
            DatabaseReference reference = _rootRef;
            foreach (string s in paths)
            {
                reference = reference.Child(s);
            }

            DataSnapshot snapshot = await reference.GetValueAsync();
            string json = snapshot.GetRawJsonValue();
            T data = JsonUtility.FromJson<T>(json);
            
            return data;
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error while loading data: {ex.Message}");
            return default;
        }
    }
    
    public async void LoadDataFromAsync<T>(List<string> paths, Action<bool, T> callback)
    {
        Logger.Log("LoadDataFromAsync start");

        if (!_isFbInit)
        {
            callback(false, default);
            return;
        }

        T t = await LoadDataFromPrivateAsync<T>(paths);

        // Check if t is not null and not equal to default value
        bool isDataFound = !EqualityComparer<T>.Default.Equals(t, default);
        
        callback(isDataFound, t);
    }
    
    #endregion

    #region Save

    public async void SaveDataToAsync(List<string> paths, object data)
    {
        Logger.Log("SaveDataToAsync start");

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
    
    #endregion
}
