using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetData : MonoBehaviour
{
    public ItemDatabase itemdata;
    Image sellButtonSprite;
    Image buyButtonSprite;
    Text buyButtonText;
    Text sellButtonText;
    Text itemDetail;
    Text itemDetail_Inven;
    Text itemDetail2;
    Text itemDetail2_Inven;
    Text itemDetail3;
    Text itemDetail4;


    private void Awake()
    {
        itemDetail = GameObject.Find("Des1").GetComponent<Text>();
        itemDetail2 = GameObject.Find("Des2").GetComponent<Text>();
        itemDetail_Inven = GameObject.Find("Des1_Inven").GetComponent<Text>();
        itemDetail2_Inven = GameObject.Find("Des2_Inven").GetComponent<Text>();
        itemDetail3 = GameObject.Find("Des3").GetComponent<Text>();
        itemDetail4 = GameObject.Find("Des4").GetComponent<Text>();

        sellButtonSprite = GameObject.Find("sellButtonSprite").GetComponent<Image>();
        sellButtonText = GameObject.Find("sellButtonText").GetComponent<Text>();
        buyButtonSprite = GameObject.Find("buyButtonSprite").GetComponent<Image>();
        buyButtonText = GameObject.Find("buyButtonText").GetComponent<Text>();
    }
    public void GetInfo()
    {
        for (int i = 0; i < itemdata.ItemData.Count; i++)
        {
            GameObject c = GameObject.Instantiate(itemdata.ItemData[i].Prefab);
            c.transform.localScale = Vector3.zero;
            if (this.GetComponent<ItemMov>().childitem.name == c.name)
            {
                if (this.CompareTag("Store"))
                {
                    StoreManager.Instance.itemindex = i;
                }
            }
            Destroy(c);
        }
    }
    public void GetInfo_Inven()
    {
        for (int i = 0; i < itemdata.ItemData.Count; i++)
        {
            GameObject c = GameObject.Instantiate(itemdata.ItemData[i].Prefab);
            c.transform.localScale = Vector3.zero;
            if (this.GetComponent<ItemMov>().childitem_Inven.name == c.name)
            {
                if (this.CompareTag("Inven"))
                {
                    StoreManager.Instance.Itemindex_Inven = i;
                }
            }
            Destroy(c);
        }
    }
    public void ItemInfo()
    {
        int idx = FileIOSystem.Instance.invendatabase.mydata.FindIndex(data => data.id == itemdata.ItemData[StoreManager.Instance.itemindex].ID);
        if (idx == -1)
        {
            MyData data = new();
            data.id = itemdata.ItemData[StoreManager.Instance.itemindex].ID;
            data.count = 0;
            FileIOSystem.Instance.invendatabase.mydata.Add(data);
        }
        //GetInfo();
        for (int i = 0; i < FileIOSystem.Instance.invendatabase.mydata.Count; i++)
        {
            if (FileIOSystem.Instance.invendatabase.mydata[i].id == itemdata.ItemData[StoreManager.Instance.itemindex].ID)
            {
               if (FileIOSystem.Instance.invendatabase.mydata[i].count <= 0)
                {
                    sellButtonSprite.color = new Color32(85,85,85,255);
                    sellButtonText.color = new Color32(255, 255, 255, 255);
                    sellButtonText.fontSize = 30;
                    sellButtonText.text = "갯수가 부족합니다";
                }
                if (FileIOSystem.Instance.invendatabase.mydata[i].count > 0)
                {
                    sellButtonSprite.color = new Color32(255, 255, 255, 255);
                    sellButtonText.color = new Color32(50, 50, 50, 255);
                    sellButtonText.fontSize = 40;
                    sellButtonText.text = "되팔기";
                }
                itemDetail.text = itemdata.ItemData[StoreManager.Instance.itemindex].DetailInfo;
                Debug.Log("\n 판매2 가격 = " + itemdata.ItemData[StoreManager.Instance.itemindex].SellPrice.ToString());
                itemDetail2.text = "현재 보유 수 " + FileIOSystem.Instance.invendatabase.mydata[i].count.ToString() + "\n 판매 가격 = " + itemdata.ItemData[StoreManager.Instance.itemindex].SellPrice.ToString() + "\n 구매 가격 = " + itemdata.ItemData[StoreManager.Instance.itemindex].BuyPrice.ToString();
                itemDetail3.text = "판매 가격 = " + itemdata.ItemData[StoreManager.Instance.itemindex].SellPrice.ToString();
                itemDetail4.text = "구매 가격 = " + itemdata.ItemData[StoreManager.Instance.itemindex].BuyPrice.ToString();

            }
        }
        for (int i = 0; i < FileIOSystem.Instance.invendatabase.mydata.Count; i++)
        {
            if (FileIOSystem.Instance.invendatabase.mydata[i].id == itemdata.ItemData[StoreManager.Instance.itemindex].ID)
            {
                if (FileIOSystem.Instance.invendatabase.money < itemdata.ItemData[StoreManager.Instance.itemindex].BuyPrice)
                {
                    buyButtonSprite.color = new Color32(85, 85, 85, 255);
                    buyButtonText.color = new Color32(255, 255, 255, 255);
                    buyButtonText.fontSize = 30;
                    buyButtonText.text = "골드가 부족합니다";
                }
                if (FileIOSystem.Instance.invendatabase.money >= itemdata.ItemData[StoreManager.Instance.itemindex].BuyPrice)
                {
                    buyButtonSprite.color = new Color32(255, 255, 255, 255);
                    buyButtonText.color = new Color32(50, 50, 50, 255);
                    buyButtonText.fontSize = 40;
                    buyButtonText.text = "구매하기";
                }
                itemDetail.text = itemdata.ItemData[StoreManager.Instance.itemindex].DetailInfo;
                itemDetail2.text = "현재 보유 수 " + FileIOSystem.Instance.invendatabase.mydata[i].count.ToString();
            }
        }

    }
    public void ItemInfo_Inven()
    {
        int idx = FileIOSystem.Instance.invendatabase.mydata.FindIndex(data => data.id == itemdata.ItemData[StoreManager.Instance.Itemindex_Inven].ID);
        if (idx == -1)
        {
            MyData data = new();
            data.id = itemdata.ItemData[StoreManager.Instance.Itemindex_Inven].ID;
            data.count = 0;
            FileIOSystem.Instance.invendatabase.mydata.Add(data);
        }
        for (int i = 0; i < FileIOSystem.Instance.invendatabase.mydata.Count; i++)
        {
            if (FileIOSystem.Instance.invendatabase.mydata[i].id == itemdata.ItemData[StoreManager.Instance.Itemindex_Inven].ID)
            {
                itemDetail_Inven.text = itemdata.ItemData[StoreManager.Instance.Itemindex_Inven].DetailInfo;
                itemDetail2_Inven.text = "현재 보유 수 " + FileIOSystem.Instance.invendatabase.mydata[i].count.ToString();
            }
        }
    }
}
