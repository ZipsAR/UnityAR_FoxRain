using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDatabase : MonoBehaviour
{
    public int money = 0;
    public Text T_money;
    public Text pay_info;
    public int payInfo = 100;
    // Start is called before the first frame update
    void Start()
    {
        money = 10000;
        payInfo = 100;
        T_money.text = "���� ��� = " + money;
    }
    public void Pay()
    {
        money -= payInfo;
        T_money.text = "���� ��� = " + money;
    }
    public void GetPayInfo()
    {
        pay_info.text = "�����ϱ� ��� = " + payInfo;
    }
}
