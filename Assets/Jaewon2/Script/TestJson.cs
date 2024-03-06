using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestJson : MonoBehaviour
{
    void Start()
    {
        FileIOSystem.Instance.Load(FileIOSystem.Instance.InvenDatabase, FileIOSystem.InvenFileName);
        Debug.Log(FileIOSystem.Instance.InvenDatabase.mydata.Count);
        RebootData();FileIOSystem.Instance.AllSave();FileIOSystem.Instance.AllLoad();
        Debug.Log("Json money°ª = " + FileIOSystem.Instance.InvenDatabase.money);
        string c = JsonUtility.ToJson(FileIOSystem.Instance.InvenDatabase);
        Debug.Log(c);
    }
    //reboot database
    public void RebootData()
    {
        FileIOSystem.Instance.InvenDatabase.mydata.Clear();
        FileIOSystem.Instance.InvenDatabase.money = 30000;
        for (int i = 0; i < 28; i++)
        {
            MyData a = new();
            if (FileIOSystem.Instance.InvenDatabase.mydata.Count < 28)
            {
                FileIOSystem.Instance.InvenDatabase.mydata.Add(a);
            }
            FileIOSystem.Instance.InvenDatabase.mydata[i].count = 1;
            if (i < 14)
            {
                FileIOSystem.Instance.InvenDatabase.mydata[i].id = 1001 + i;
            }
            if (14 <= i && 19 > i)
            {
                FileIOSystem.Instance.InvenDatabase.mydata[i].id = 2001 + i - 14;
            }
            if (19 < i)
            {
                FileIOSystem.Instance.InvenDatabase.mydata[i].id = 3001 + i - 16;
            }
        }
        FileIOSystem.Instance.AllSave();
    }
}
