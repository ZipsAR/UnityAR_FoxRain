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
   // public List<ObjectData> objectdata;


    [field: SerializeField]
    //������Ʈ �̸�
    public string Name { get;  set; }

    [field: SerializeField]
    //������Ʈ ���̵�
    public int ID { get;  set; }

    [field: SerializeField]
    //������Ʈ ũ��
    public Vector2Int Size { get; set; } = Vector2Int.one;

    [field: SerializeField]
    //������Ʈ ����
    public int ObjectCount { get; set; } = 0;

    [field : SerializeField]
    //������Ʈ ����
    public GameObject Prefab { get; set; }

}


