using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[CreateAssetMenu]
public class ItemDatabase : ScriptableObject
{
    // Start is called before the first frame update
    public List<ItemData> ItemData;

    private Dictionary<int, ItemData> _itemDictionary;
    public Dictionary<int, ItemData> Itemdictionary
    {
        get
        {
            if (_itemDictionary == null)
            {
                _itemDictionary = new();
                for (int i = 0; i < ItemData.Count; i++) _itemDictionary.Add(ItemData[i].ID, ItemData[i]);
            }
            return _itemDictionary;
        }
        set { _itemDictionary = value; }
    }
}


//���� ���� ���� �ִ� �Ͽ�¡ ������ ���� ����
[Serializable]
public class ItemData
{

    public enum ItemCategory
    {
        Funiture = 1000,
        Equip = 2000,
        Snack = 3000
    }

    public enum FunitureCategory
    {
        None,
        Place,
        Wall,
        Floor
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

    //Name
    public string Name;

    //Primary key
    public int ID;

    //(housing funiture) size
    public Vector2Int Housingsize  = Vector2Int.one;

    //(Store) Buying Price 
    public float BuyPrice = 0f;

    //(Store) Selling Price
    public float SellPrice  = 0f;

    //Actual Object Modeling
    public GameObject Prefab;

    //Text Explain
    public string DetailInfo = "No Data";

    //(housing funiture) Category
    public FunitureCategory funitureCategory = FunitureCategory.None;

    //Item Category
    public ItemCategory itemCategory;

    //(Interaction) ItemSound
    public ItemSound itemSound;
}
