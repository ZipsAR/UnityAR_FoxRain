using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UICon : MonoBehaviour
{
    public Text T_BuyText;
    public Text T_money;
    public float money; //after FileIOSystem.Inven.monry > public
    public bool isSelected;
    public ItemDatabase itemdata;
    private void Start()
    {
        isSelected = false;
        money = 10000;
        T_money.text = money.ToString();
        FileIOSystem.Instance.AllLoad();
    }
    public void BuyItem()
    {
        T_BuyText.text = "�����ϱ� ����";
        isSelected = true;
    }
    public void Pay()
    {
        money -= itemdata.ItemData[StoreManager.Instance.itemindex].BuyPrice;
        T_money.text = money.ToString();
        Debug.Log("BuyPrice = " + itemdata.ItemData[StoreManager.Instance.itemindex].BuyPrice);
        FileIOSystem.Instance.invendatabase.mydata[StoreManager.Instance.itemindex].count++;
        FileIOSystem.Instance.AllSave();
        FileIOSystem.Instance.AllLoad();
        string c = JsonUtility.ToJson(FileIOSystem.Instance.invendatabase);
        Debug.Log(c);
    }
    public void Sell()
    {
        money += itemdata.ItemData[StoreManager.Instance.itemindex].SellPrice;
        T_money.text = money.ToString();
        Debug.Log("SellPrice = " + itemdata.ItemData[StoreManager.Instance.itemindex].SellPrice);
        FileIOSystem.Instance.invendatabase.mydata[StoreManager.Instance.itemindex].count--;
        FileIOSystem.Instance.AllSave();
        FileIOSystem.Instance.AllLoad();
        string c = JsonUtility.ToJson(FileIOSystem.Instance.invendatabase);
        Debug.Log(c);
    }
}
