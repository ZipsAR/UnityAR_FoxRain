using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestJson : MonoBehaviour
{
    void Start()
    {

        FileIOSystem.Instance.AllLoad();
        Debug.Log(FileIOSystem.Instance.invendatabase.mydata.Count);
        /*RebootData();
        FileIOSystem.Instance.AllSave();
        FileIOSystem.Instance.AllLoad();*/
        string c = JsonUtility.ToJson(FileIOSystem.Instance.invendatabase);
        Debug.Log(c);
    }
    //reboot database
    public void RebootData()
    {
        FileIOSystem.Instance.invendatabase.mydata.Clear();   
        for (int i = 0; i < 17; i++)
        {
            MyData a = new();
            if (FileIOSystem.Instance.invendatabase.mydata.Count < 17)
            {
                FileIOSystem.Instance.invendatabase.mydata.Add(a);
            }
            FileIOSystem.Instance.invendatabase.mydata[i].count = 1;
            if (i < 14)
            {
                FileIOSystem.Instance.invendatabase.mydata[i].id = 1001 + i;
            }
            if (13 < i && 16 > i)
            {
                FileIOSystem.Instance.invendatabase.mydata[i].id = 2001 + i - 14;
            }
            if (15 < i)
            {
                FileIOSystem.Instance.invendatabase.mydata[i].id = 3001 + i - 16;
            }
        }
        FileIOSystem.Instance.AllSave();
    }
}
