using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestJson : MonoBehaviour
{
    public FileIOSystem fileIOSystem;
    void Start()
    {
        fileIOSystem.AllLoad();
        Debug.Log(fileIOSystem.invendatabase.mydata.Count);
        RebootData();
        fileIOSystem.AllSave();
        fileIOSystem.AllLoad();
        string c = JsonUtility.ToJson(fileIOSystem.invendatabase);
        Debug.Log(c);
    }
    //reboot database
    public void RebootData()
    {
        fileIOSystem.invendatabase.mydata.Clear();   
        for (int i = 0; i < 17; i++)
        {
            MyData a = new();
            if (fileIOSystem.invendatabase.mydata.Count < 17)
            {
                fileIOSystem.invendatabase.mydata.Add(a);
            }
            fileIOSystem.invendatabase.mydata[i].count = 1;
            if (i < 14)
            {
                fileIOSystem.invendatabase.mydata[i].id = 1001 + i;
            }
            if (13 < i && 16 > i)
            {
                fileIOSystem.invendatabase.mydata[i].id = 2001 + i - 14;
            }
            if (15 < i)
            {
                fileIOSystem.invendatabase.mydata[i].id = 3001 + i - 16;
            }
        }
        fileIOSystem.AllSave();
    }
}
