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

    //������Ʈ ���� ������
    //������Ʈ �̸�
    public string Name;

    //������Ʈ ���̵�
    public int ID;

    //������Ʈ �Ͽ�¡ ũ��
    public Vector2Int Housingsize  = Vector2Int.one;

    //���� ����
    public float BuyPrice = 0f;

    //�Ǹ� ����
    public float SellPrice  = 0f;

    //������Ʈ ���� ������
    public GameObject Prefab;

    //������Ʈ ������ ����
    public string DetailInfo = "No Data";

    //������ ī�װ�
    public ItemCategory itemCategory;

    //������ ���� ���� ����ü
    public ItemSound itemSound;
    

}
