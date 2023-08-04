using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UICon : MonoBehaviour
{
    public Text T_BuyText; //������ ���� ��ư �ؽ�Ʈ
    public Text T_money;
    public float money;
    public bool isSelected;
    public ItemDatabase itemdata;
    private void Start()
    {
        isSelected = false;
        money = 10000;
        T_money.text = money.ToString();
    }
    public void BuyItem()
    {
        T_BuyText.text = "�����ϱ� ���� = " + itemdata.ItemData[StoreManager.Instance.itemindex].BuyPrice;
        isSelected = true;
    }
    public void BuyItemClick()
    {
        Pay(itemdata.ItemData[StoreManager.Instance.itemindex].BuyPrice);
    }
    public void Pay(float pay)
    {
        money -= pay;
        T_money.text = "���� ��� = " + money;
        Debug.Log("�����ϱ�, ���� = " + itemdata.ItemData[StoreManager.Instance.itemindex].BuyPrice);
    }
    public void Sell()
    {
        money += itemdata.ItemData[StoreManager.Instance.itemindex].SellPrice;
        T_money.text = "���� ��� = " + money;
        Debug.Log("�Ǹ��ϱ�, ���� = " + itemdata.ItemData[StoreManager.Instance.itemindex].SellPrice);
    }
}
