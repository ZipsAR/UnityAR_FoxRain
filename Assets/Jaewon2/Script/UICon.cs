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
        T_money.text = FileIOSystem.Instance.invendatabase.money.ToString();
        FileIOSystem.Instance.AllLoad();
    }
    public void BuyItem()
    {
        T_BuyText.text = "�����ϱ� ����";
        isSelected = true;
    }
    public void Pay()
    {
        FileIOSystem.Instance.invendatabase.money -= (int)itemdata.ItemData[StoreManager.Instance.itemindex].BuyPrice;
        T_money.text = FileIOSystem.Instance.invendatabase.money.ToString();
        Debug.Log("BuyPrice = " + itemdata.ItemData[StoreManager.Instance.itemindex].BuyPrice);

        //TaeSeung CODING
        int idx = FileIOSystem.Instance.invendatabase.mydata.FindIndex(data => data.id == itemdata.ItemData[StoreManager.Instance.itemindex].ID);
        if (idx == -1)
        {
            MyData data = new();
            data.id = itemdata.ItemData[StoreManager.Instance.itemindex].ID;
            data.count = 0;
            FileIOSystem.Instance.invendatabase.mydata.Add(data);
            FileIOSystem.Instance.invendatabase.mydata[FileIOSystem.Instance.invendatabase.mydata.Count-1].count++;
        }
        else
        {
            FileIOSystem.Instance.invendatabase.mydata[idx].count++;
        }
        //

        FileIOSystem.Instance.AllSave();
        FileIOSystem.Instance.AllLoad();
        GetComponent<GetData>().ItemInfo_Inven();
        GetComponent<GetData>().ItemInfo();
        string c = JsonUtility.ToJson(FileIOSystem.Instance.invendatabase);
        Debug.Log("구매 = "+ StoreManager.Instance.itemindex + c);
    }
    public void Sell()
    {
        FileIOSystem.Instance.invendatabase.money += (int)itemdata.ItemData[StoreManager.Instance.itemindex].SellPrice;
        T_money.text = FileIOSystem.Instance.invendatabase.money.ToString();
        Debug.Log("SellPrice = " + itemdata.ItemData[StoreManager.Instance.itemindex].SellPrice);
        FileIOSystem.Instance.invendatabase.mydata[StoreManager.Instance.itemindex].count--;
        FileIOSystem.Instance.AllSave();
        FileIOSystem.Instance.AllLoad();
        GetComponent<GetData>().ItemInfo_Inven();
        GetComponent<GetData>().ItemInfo();
        string c = JsonUtility.ToJson(FileIOSystem.Instance.invendatabase);
        Debug.Log("판매 = "+ StoreManager.Instance.itemindex+c);
    }
}
