using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;


public class FileIOSystem : Singleton<FileIOSystem>
{
    public InventoryDatabase invendatabase;
    public HousingObjectdatabase housingdatabase;
    public StatDatabase statdatabase;
    public string path;

    public const string InvenFilename = "Invendatabase";
    public const string HousingFilename = "Housingdatabase";
    public const string StatFilename = "Statdatabase";

    private void Start()
    {
        path = Application.persistentDataPath;
        print(IsFileExist(InvenFilename));

        try
        {
            AllLoad();
        }
        catch(IOException e)
        {
            Save(housingdatabase, HousingFilename);
            Save(invendatabase, InvenFilename);

        }
    }


    /// <summary>
    /// Check file exist
    /// </summary>
    /// <param name="filename">choice find filename</param>
    /// <returns>exist: true, else: false </returns>
    public bool IsFileExist(string filename)
    {
        if (File.Exists(path + "/" + filename + ".json"))
            return true;

        else
            return false;
    }




    //Save data file
    public void Save<T>(T database, string Filename)
    {
        FileStream filestream = new FileStream(string.Format("{0}/{1}.json",path, Filename), FileMode.Create);
        string jsondata = JsonUtility.ToJson(database);
        byte[] data = Encoding.UTF8.GetBytes(jsondata);
        filestream.Write(data, 0, data.Length);
        filestream.Close();
    }


    //Load data file
    public void Load<T>(T database, string Filename)
    {
        FileStream filestream = new FileStream(string.Format("{0}/{1}.json", path, Filename), FileMode.Open);
        byte[] data = new byte[filestream.Length];
        filestream.Read(data, 0, data.Length);
        filestream.Close();
        string jsondata = Encoding.UTF8.GetString(data);
        database = JsonUtility.FromJson<T>(jsondata);

    }

    public void AllSave()
    {
        FileSave<InventoryDatabase>("Invendatabase", invendatabase);
        FileSave<HousingObjectdatabase>("Housingdatabase", housingdatabase);
        FileSave<StatDatabase>("Statdatabase", statdatabase);
    }

    //Load data file
    public void AllLoad()
    {
        invendatabase = JsonUtility.FromJson<InventoryDatabase>(FileOpen(InvenFilename));
        housingdatabase = JsonUtility.FromJson<HousingObjectdatabase>(FileOpen(HousingFilename));
        statdatabase = JsonUtility.FromJson<StatDatabase>(FileOpen(StatFilename));
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
