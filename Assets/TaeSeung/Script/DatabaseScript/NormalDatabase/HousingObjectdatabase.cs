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
      //������ ���� �����ε�, ������ �����̰� ��������Ű����� ���߿� ��Ծ
      public PetStatBase savedStat = new();
}



//Data Class
[Serializable]
public class MyData
{
    public int id;
    public int count;
}



//���� ��ġ��Ų �Ͽ�¡ ������ ���� ����
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
