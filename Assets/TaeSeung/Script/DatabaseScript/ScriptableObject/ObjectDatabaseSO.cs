using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu]  //우클릭 시켜서 생성가능 한 에셋 목록에 포함시킨다
public class ObjectDatabaseSO : ScriptableObject
{
    // Start is called before the first frame update
    public List<ObjectData> objectsData;
    public List<ObjectLocation> objectsLocation;
}


//내가 현재 갖고 있는 하우징 가구에 대한 정보
[Serializable]
public class ObjectData
{
   // public List<ObjectData> objectdata;


    [field: SerializeField]
    //오브젝트 이름
    public string Name { get;  set; }

    [field: SerializeField]
    //오브젝트 아이디
    public int ID { get;  set; }

    [field: SerializeField]
    //오브젝트 크기
    public Vector2Int Size { get; set; } = Vector2Int.one;

    [field: SerializeField]
    //오브젝트 갯수
    public int ObjectCount { get; set; } = 0;

    [field : SerializeField]
    //오브젝트 에셋
    public GameObject Prefab { get; set; }

}


