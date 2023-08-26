using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Local Storage Class
[Serializable]
public class InventoryDatabase
{
    public int money = 0;
    public List<MyData> mydata = new();
}


[Serializable]
public class HousingObjectdatabase
{
    //public List<ObjectLocation> objectsLocationList = new();
    public Dictionary<int, ObjectLocation> objectsLocation = new();
    /*{
        get
        {
            if (objectsLocation == null) objectsLocation = new();

            if (objectsLocation.Count == 0)
            {
                for (int i = 0; i < objectsLocationList.Count; i++) objectsLocation[objectsLocation[i].InstanceId] = objectsLocationList[i];
            }
            return objectsLocation;
        }
        private set{ }
    }*/

}


[Serializable]
public class StatDatabase
{
      public List<PetStatBase> savedStats = new();
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
    [Serializable]
    public struct SerializableVector3Int
    {
        public int x;
        public int y;
        public int z;

        public SerializableVector3Int(Vector3Int v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public static implicit operator SerializableVector3Int(Vector3Int sv)
        {
            return new SerializableVector3Int(sv);
        }

        public static implicit operator Vector3Int(SerializableVector3Int sv)
        {
            return new Vector3Int(sv.x, sv.y, sv.z);
        }

    }

    public struct SerializableVector2Int
    {
        public int x;
        public int y;

        public SerializableVector2Int(Vector2Int v)
        {
            x = v.x;
            y = v.y;
        }

        public static implicit operator SerializableVector2Int(Vector2Int sv)
        {
            return new SerializableVector2Int(sv);
        }

        public static implicit operator Vector2Int(SerializableVector2Int sv)
        {
            return new Vector2Int(sv.x, sv.y);
        }

    }

    [Serializable]
    public struct SerializableQuaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;
        
        
        public SerializableQuaternion(Quaternion q)
        {
            x = q.x;
            y = q.y;
            z = q.z;
            w = q.w;
        }

        public static implicit operator SerializableQuaternion(Quaternion sv)
        {
            return new SerializableQuaternion(sv);
        }

        public static implicit operator Quaternion(SerializableQuaternion sv)
        {
            return new Quaternion(sv.x, sv.y, sv.z, sv.w);
        }
    }


    public int InstanceId;
    public SerializableVector3Int location;
    public SerializableQuaternion rotation;
    public SerializableVector2Int size;
    public int id;
    public bool placementstatus = false;
}
