using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;


public class FileIOSystem : Singleton<FileIOSystem>
{
    public ObjectDatabaseSO database;
    string path;

    private void Start()
    {
        path =  Application.persistentDataPath; 
    }


    public void Save()
    {
        FileStream filestream = new FileStream(string.Format("{0}/{1}.json",path, "ObjectDatabase"), FileMode.Create);
        string jsondata = JsonUtility.ToJson(database);

        byte[] data = Encoding.UTF8.GetBytes(jsondata);
        filestream.Write(data, 0, data.Length);
        filestream.Close();

        Debug.Log("µ∆¥Ÿ¿Ã±‚");
    }

    public ObjectDatabaseSO Load()
    {

        FileStream filestream = new FileStream(string.Format("{0}/{1}.json", path, "ObjectDatabase"), FileMode.Open);
        
        
        byte[] data = new byte[filestream.Length];
        filestream.Read(data, 0, data.Length);
        filestream.Close();
        string jsondata = Encoding.UTF8.GetString(data);
        database = JsonUtility.FromJson<ObjectDatabaseSO>(jsondata);
        return database;
    }


}
