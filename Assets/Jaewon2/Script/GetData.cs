using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetData : MonoBehaviour
{
    public ItemDatabase itemdata;
    Text itemDetail;
    Text itemDetail_Inven;
    private void Start()
    {
        itemDetail = GameObject.Find("Des1").GetComponent<Text>();
        itemDetail_Inven = GameObject.Find("Des1_Inven").GetComponent<Text>();
    }
    public void GetInfo()
    {
            for (int i = 0; i < itemdata.ItemData.Count; i++)
            {
                GameObject c =GameObject.Instantiate(itemdata.ItemData[i].Prefab);
                c.transform.localScale = Vector3.zero;
                if (this.GetComponent<ItemMov>().childitem.name == c.name)
                {
                StoreManager.Instance.itemindex = i;
                Debug.Log(StoreManager.Instance.itemindex);
                }
            }
    }
    public void ItemInfo()
    {
        if (StoreManager.Instance.viewItem_store != null)
        {
            itemDetail.text = itemdata.ItemData[StoreManager.Instance.itemindex].DetailInfo;
        }
        if (StoreManager.Instance.viewItem_Inven != null)
        {
            itemDetail_Inven.text = itemdata.ItemData[StoreManager.Instance.itemindex].DetailInfo;
        }

    }
}
