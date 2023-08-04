using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;


public class FileIOSystem : Singleton<FileIOSystem>
{
    public InventoryDatabase invendatabase { get; private set; }
    public HousingObjectdatabase housingdatabase { get; private set; } 
    public StatDatabase statdatabase { get; private set; } 
    public string path;

    public const string InvenCall = "Invendatabase";
    public const string HousingCall = "Housingdatabase";
    public const string StatCall = "Statdatabase";

    private void Awake()
    {
        path = Application.persistentDataPath;
        try
        {
            StartLoad();
        }
        catch(IOException e)
        {
            StartSave();
            invendatabase = new();
            housingdatabase = new();
            statdatabase = new();
        }
    }


    //Save data file
    public void Save<T>(T database, string databasemode)
    {
        FileStream filestream = new FileStream(string.Format("{0}/{1}.json",path, databasemode), FileMode.Create);
        string jsondata = JsonUtility.ToJson(database);
        byte[] data = Encoding.UTF8.GetBytes(jsondata);
        filestream.Write(data, 0, data.Length);
        filestream.Close();
    }


    //Load data file
    public void Load<T>(ref T database, string databasemode)
    {
        FileStream filestream = new FileStream(string.Format("{0}/{1}.json", path, databasemode), FileMode.Open);
        byte[] data = new byte[filestream.Length];
        filestream.Read(data, 0, data.Length);
        filestream.Close();
        string jsondata = Encoding.UTF8.GetString(data);
        database = JsonUtility.FromJson<T>(jsondata);

    }

    public void StartSave()
    {
        FileSave<InventoryDatabase>("Invendatabase", invendatabase);
        FileSave<HousingObjectdatabase>("Housingdatabase", housingdatabase);
        FileSave<StatDatabase>("Statdatabase", statdatabase);
    }

    //Load data file
    public void StartLoad()
    {
        invendatabase = JsonUtility.FromJson<InventoryDatabase>(FileOpen(InvenCall));
        housingdatabase = JsonUtility.FromJson<HousingObjectdatabase>(FileOpen(HousingCall));
        statdatabase = JsonUtility.FromJson<StatDatabase>(FileOpen(StatCall));
    }


    private string FileOpen(string filename)
    {
        FileStream filestream = new FileStream(string.Format("{0}/{1}.json", path, filename), FileMode.Open);
        byte[] data = new byte[filestream.Length];
        filestream.Read(data, 0, data.Length);
        filestream.Close();
        string jsondata = Encoding.UTF8.GetString(data);
        return jsondata;
    }


    private void FileSave<T>(string filename, T database)
    {
        FileStream filestream = new FileStream(string.Format("{0}/{1}.json", path, filename), FileMode.Create);
        string jsondata = JsonUtility.ToJson(database);
        byte[] data = Encoding.UTF8.GetBytes(jsondata);
        filestream.Write(data, 0, data.Length);
        filestream.Close();
    }
}
