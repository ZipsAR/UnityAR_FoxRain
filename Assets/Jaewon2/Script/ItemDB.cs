using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDB", menuName = "Scriptable Object/ItemDB", order = int.MaxValue)]

public class ItemDB : ScriptableObject
{
    enum ItemData//아이템 열거형, 추후에 조정 예정
    {
        Item0 = 0,
        Item1,
        Item2,
        Item3 = 10,
        Item4,
        Item5,
        Item6 = 20,
        Item7,
        Item8,
        Item9
    }
    public string[] ItemName = new string[10];
    public GameObject[] Item = new GameObject[10];
    public int[] buyGold = new int[10];
    public int[] soldGold = new int[10];
    public int[] i_number = new int[10];//인벤토리 잔여 갯수
    public int money;
}
