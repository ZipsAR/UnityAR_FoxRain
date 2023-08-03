using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UICon : MonoBehaviour
{
    public Text T_BuyText; //아이템 구매 버튼 텍스트
    public Text T_money;
    public float money;
    public bool isSelected;
    public ItemDatabase itemdata;
    private void Start()
    {
        isSelected = false;
        money = 10000;
        T_money.text = "현재 골드 = " + money;
    }
    public void BuyItem()
    {
        T_BuyText.text = "구매하기 가격 = " + itemdata.ItemData[StoreManager.Instance.itemindex].BuyPrice;
        isSelected = true;
    }
    public void BuyItemClick()
    {
        Pay(itemdata.ItemData[StoreManager.Instance.itemindex].BuyPrice);
    }
    public void Pay(float pay)
    {
        money -= pay;
        T_money.text = "현재 골드 = " + money;
        Debug.Log("구매하기, 가격 = " + itemdata.ItemData[StoreManager.Instance.itemindex].BuyPrice);
    }
    public void Sell()
    {
        money += itemdata.ItemData[StoreManager.Instance.itemindex].SellPrice;
        T_money.text = "현재 골드 = " + money;
        Debug.Log("판매하기, 가격 = " + itemdata.ItemData[StoreManager.Instance.itemindex].SellPrice);
    }
}
