using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[CreateAssetMenu]
public class ItemDatabase : ScriptableObject
{
    // Start is called before the first frame update
    public List<Item> ItemData;


}


//내가 현재 갖고 있는 하우징 가구에 대한 정보
[Serializable]
public class Item
{

    public enum ItemCategory
    {
        Funiture = 1000,
        Equip = 2000,
        Snack = 3000
    }

    [System.Serializable]
    public struct ItemSound
    {
        public AudioClip eatSound; 
        public AudioClip throwSound;

        public ItemSound(AudioClip eatSound, AudioClip throwSound)
        {
            this.eatSound = eatSound;
            this.throwSound = throwSound;
        }
    }

    //오브젝트 실제 프리팹
    //오브젝트 이름
    public string Name;

    //오브젝트 아이디
    public int ID;

    //오브젝트 하우징 크기
    public Vector2Int Housingsize  = Vector2Int.one;

    //구매 가격
    public float BuyPrice = 0f;

    //판매 가격
    public float SellPrice  = 0f;

    //오브젝트 실제 프리팹
    public GameObject Prefab;

    //오브젝트 디테일 설명
    public string DetailInfo = "No Data";

    //아이템 카테고리
    public ItemCategory itemCategory;

    //아이템 사운드 관련 구조체
    public ItemSound itemSound;
    

}
