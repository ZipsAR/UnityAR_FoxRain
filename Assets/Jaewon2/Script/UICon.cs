using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UICon : MonoBehaviour
{
    public Text T_BuyText;
    public Text T_money;
    public bool isSelected;
    public ItemDatabase itemdata;
    private void Start()
    {
        isSelected = false;
        T_money.text = FileIOSystem.Instance.invendatabase.money.ToString();
        FileIOSystem.Instance.Load(FileIOSystem.Instance.invendatabase, FileIOSystem.InvenFilename);

    }
    public void BuyItem()
    {
        T_BuyText.text = "�����ϱ� ����";
        isSelected = true;
    }
    public void Pay()
    {
        if(StoreManager.Instance.itemindex == -1) return;
        
        //GetComponent<GetData>().ItemInfo();
        T_money.text = FileIOSystem.Instance.invendatabase.money.ToString();

        //TaeSeung CODING
        int idx = FileIOSystem.Instance.invendatabase.mydata.FindIndex(data => data.id == itemdata.ItemData[StoreManager.Instance.itemindex].ID);
        if (idx == -1)
        {
            MyData data = new();
            data.id = itemdata.ItemData[StoreManager.Instance.itemindex].ID;
            data.count = 0;
            FileIOSystem.Instance.invendatabase.mydata.Add(data);
            for (int i = 0; i < FileIOSystem.Instance.invendatabase.mydata.Count; i++)
            {
                if (FileIOSystem.Instance.invendatabase.mydata[i].id == StoreManager.Instance.itemindex)
                {
                    if (FileIOSystem.Instance.invendatabase.money >= itemdata.ItemData[StoreManager.Instance.itemindex].BuyPrice)
                    {
                        FileIOSystem.Instance.invendatabase.mydata[i].count++;
                    }
                }
            }
        }
            if (FileIOSystem.Instance.invendatabase.money >= itemdata.ItemData[StoreManager.Instance.itemindex].BuyPrice)
            {
                for (int i = 0; i < FileIOSystem.Instance.invendatabase.mydata.Count; i++)
                {
                    if(FileIOSystem.Instance.invendatabase.mydata[i].id == itemdata.ItemData[StoreManager.Instance.itemindex].ID)
                    {
                        FileIOSystem.Instance.invendatabase.mydata[i].count++;
                    }
                }
                FileIOSystem.Instance.invendatabase.money -= (int)itemdata.ItemData[StoreManager.Instance.itemindex].BuyPrice;
            }
        //

        FileIOSystem.Instance.Save(FileIOSystem.Instance.invendatabase,FileIOSystem.InvenFilename);
        FileIOSystem.Instance.Load(FileIOSystem.Instance.invendatabase, FileIOSystem.InvenFilename);
        GetComponent<GetData>().ItemInfo_Inven();
        GetComponent<GetData>().ItemInfo();
        //GetComponent<GetData>().GetInfo();
        string c = JsonUtility.ToJson(FileIOSystem.Instance.invendatabase);
        Debug.Log("구매 완료 = " + c);
        T_money.text = FileIOSystem.Instance.invendatabase.money.ToString();
    }
    public void Sell()
    {
        if(StoreManager.Instance.itemindex == -1) return;

        T_money.text = FileIOSystem.Instance.invendatabase.money.ToString();
        int idx = FileIOSystem.Instance.invendatabase.mydata.FindIndex(data => data.id == itemdata.ItemData[StoreManager.Instance.itemindex].ID);
        for (int i = 0; i < FileIOSystem.Instance.invendatabase.mydata.Count; i++)
        {
            if(FileIOSystem.Instance.invendatabase.mydata[i].id == itemdata.ItemData[StoreManager.Instance.itemindex].ID)
            {
                Debug.Log("갯수 추출 = " + FileIOSystem.Instance.invendatabase.mydata[i].count);
                if (FileIOSystem.Instance.invendatabase.mydata[i].count <= 0)
                {
                    Debug.Log("판매버튼 비활성화");
                }
                else
                {
                    FileIOSystem.Instance.invendatabase.mydata[i].count--;
                    Debug.Log("판매 완료 = " + FileIOSystem.Instance.invendatabase.mydata[i].count);
                    FileIOSystem.Instance.invendatabase.money += (int)itemdata.ItemData[StoreManager.Instance.itemindex].SellPrice;
                }

            }
        }
        FileIOSystem.Instance.Save(FileIOSystem.Instance.invendatabase, FileIOSystem.InvenFilename);
        FileIOSystem.Instance.Load(FileIOSystem.Instance.invendatabase, FileIOSystem.InvenFilename);
        GetComponent<GetData>().ItemInfo_Inven();
        GetComponent<GetData>().ItemInfo();
        //GetComponent<GetData>().GetInfo();
        string c = JsonUtility.ToJson(FileIOSystem.Instance.invendatabase);
        Debug.Log("판매 완료 = " + c);
        T_money.text = FileIOSystem.Instance.invendatabase.money.ToString();

    }
}
