using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Item
{
    item1 = 100,
    item2 = 200
}
public class UImanager : MonoBehaviour
{
    Item item;
    public Text T_BuyText; //아이템 구매 버튼 텍스트
    public Text T_money;
    public int money;
    public bool isSelected;
    [SerializeField]
    private ItemDB itemdb;
    private void Start()
    {
        isSelected = false;
        T_money.text = "현재 골드 = " + itemdb.money;
    }
    private void Update()
    {
        ViewRotate(); 
    }
    public void ViewRotate()
    {
    }
    public void BuyItem()
    {
        T_BuyText.text = "구매하기 가격 = " + (int)Item.item1;
        isSelected = true;
    }
    public void BuyItemClick()
    {
        Pay((int)Item.item1);
    }
    public void Pay(int pay)
    {
        itemdb.money -= pay;
        T_money.text = "현재 골드 = " + itemdb.money;
    }
    public void SellItem(int sell)
    {
        itemdb.money += sell;
        T_money.text = "현재 골드 = " + itemdb.money;
    }
}
