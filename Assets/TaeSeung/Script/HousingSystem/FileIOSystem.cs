using Newtonsoft.Json;
using System.IO;
using System.Text;
using UnityEngine;


public class FileIOSystem : Singleton<FileIOSystem>
{
    public InventoryDatabase InvenDatabase;
    public string PathName;

    [HideInInspector]
    public HousingObjectdatabase HousingDatabase;
    [HideInInspector]
    public StatDatabase StatDatabase;
    [HideInInspector]
    public const string InvenFileName = "Invendatabase";
    [HideInInspector]
    public const string HousingFileName = "Housingdatabase";
    [HideInInspector]
    public const string StatFileName = "Statdatabase";

    private void Start()
    {
        PathName = Application.persistentDataPath;
        if (IsFileExist(InvenFileName))  AllLoad();
        else{
            Save(HousingDatabase, HousingFileName);
            Save(InvenDatabase, InvenFileName);
        }
    }

    /// <summary>
    /// Check file exist
    /// </summary>
    /// <param name="filename">choice find filename</param>
    /// <returns>exist: true, else: false </returns>
    public bool IsFileExist(string filename)
    {
        if (File.Exists(PathName + "/" + filename + ".json")) return true;
        else return false;
    }

    //Save data file
    public void Save<T>(T database, string Filename)
    {
        FileStream filestream = new FileStream(string.Format("{0}/{1}.json",PathName, Filename), FileMode.Create);
        string jsondata;
        jsondata = JsonConvert.SerializeObject(database);
        byte[] data = Encoding.UTF8.GetBytes(jsondata);
        filestream.Write(data, 0, data.Length);
        filestream.Close();
    }


    //Load data file
    public void Load<T>(T database, string Filename)
    {
        FileStream filestream = new FileStream(string.Format("{0}/{1}.json", PathName, Filename), FileMode.Open);
        print(database.GetType());
        byte[] data = new byte[filestream.Length];
        filestream.Read(data, 0, data.Length);
        filestream.Close();

        string jsondata = Encoding.UTF8.GetString(data);
        database = JsonConvert.DeserializeObject<T>(jsondata);

    }

    public void AllSave()
    {
        FileSave<InventoryDatabase>("Invendatabase", InvenDatabase);
        FileSave<HousingObjectdatabase>("Housingdatabase", HousingDatabase);
        FileSave<StatDatabase>("Statdatabase", StatDatabase);
    }

    //Load data file
    public void AllLoad()
    {
        InvenDatabase = JsonUtility.FromJson<InventoryDatabase>(FileOpen(InvenFileName));
        HousingDatabase = JsonConvert.DeserializeObject<HousingObjectdatabase>(FileOpen(HousingFileName));
        StatDatabase = JsonUtility.FromJson<StatDatabase>(FileOpen(StatFileName));
    }


    private string FileOpen(string filename)
    {
        FileStream filestream = new FileStream(string.Format("{0}/{1}.json", PathName, filename), FileMode.Open);
        byte[] data = new byte[filestream.Length];
        filestream.Read(data, 0, data.Length);
        filestream.Close();
        string jsondata = Encoding.UTF8.GetString(data);
        return jsondata;
    }


    private void FileSave<T>(string filename, T database)
    {
        FileStream filestream = new FileStream(string.Format("{0}/{1}.json", PathName, filename), FileMode.Create);
        string jsondata = JsonUtility.ToJson(database);
        byte[] data = Encoding.UTF8.GetBytes(jsondata);
        filestream.Write(data, 0, data.Length);
        filestream.Close();
    }
}
