using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Local Storage Class
[Serializable]
public class InventoryDatabase
{
    int money = 0;
    public List<MyData> mydata = new();
}


[Serializable]
public class HousingObjectdatabase
{
    public List<ObjectLocation> objectsLocation = new();
   
}


[Serializable]
public class StatDatabase
{
      //태현이 스텟 정보인데, 어차피 태현이가 만들었을거같으니 나중에 써먹어봄
}



//Data Class
[Serializable]
public class MyData
{
    public int id;
    public int count;
}



//내가 배치시킨 하우징 가구에 대한 정보
[Serializable]
public class ObjectLocation
{
    public int InstanceId;

    public Vector3Int location;
    public Quaternion rotation;

    public Vector2Int size;

    public int id;

    public bool placementstatus = false;
}
