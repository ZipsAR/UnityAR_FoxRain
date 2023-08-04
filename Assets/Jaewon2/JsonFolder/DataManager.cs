using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class JsonItemData
{
    public int id;
    public int currentCount;
    public int money;
    public Vector3Int furniturePosition;
    public Vector3 furnitureRotation;
    public Vector2Int furnitureScale;
}
public class DataManager : MonoBehaviour
{
    public static DataManager instance;
    JsonItemData jsonData = new JsonItemData();
    string path;
    string fileName = "save";

    public void Awake()
    {
        #region ΩÃ±€≈Ê
        if (instance == null)
        {
            instance = this;
        }
        if(instance != null)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        #endregion
        path = Application.persistentDataPath + "/";
    }
    public void SaveData()
    {
        string jsondata = JsonUtility.ToJson(jsonData);
        File.WriteAllText(path + fileName, jsondata);
    }
    public void LoadData()
    {
        string data = File.ReadAllText(path + fileName);
        JsonUtility.FromJson<JsonItemData>(data);
    }
}
