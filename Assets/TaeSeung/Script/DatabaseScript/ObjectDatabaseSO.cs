using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]  //��Ŭ�� ���Ѽ� �������� �� ���� ��Ͽ� ���Խ�Ų��
public class ObjectDatabaseSO : ScriptableObject
{
    // Start is called before the first frame update
    public List<ObjectData> objectsData;
    public List<ObjectLocation> objectsLocation;
}


//���� ���� ���� �ִ� �Ͽ�¡ ������ ���� ����
[Serializable]
public class ObjectData
{
    [field: SerializeField]
    //������Ʈ �̸�
    public string Name { get;  private set; }

    [field: SerializeField]
    //������Ʈ ���̵�
    public int ID { get; private set; }

    [field: SerializeField]
    //������Ʈ ũ��
    public Vector2Int Size { get; private set; } = Vector2Int.one;

    [field: SerializeField]
    //������Ʈ ����
    public int ObjectCount { get; set; } = 0;

    [field : SerializeField]
    //������Ʈ ����
    public GameObject Prefab { get; private set; }

}

//���� ��ġ��Ų �Ͽ�¡ ������ ���� ����
[Serializable]
public class ObjectLocation
{
    public int InstanceId;

    public Vector3Int location;
    //position = xyz
    public Quaternion rotation;
    //rotation xyz

    public Vector2Int size;

    public int OBJID;

    public bool placementstatus = false;
}


