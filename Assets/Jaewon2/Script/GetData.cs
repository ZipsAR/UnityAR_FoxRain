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
                    Debug.Log("인덱스 추출 = " + StoreManager.Instance.Itemindex_Inven);
                }
            }
            Destroy(c);
        }
    }
    public void ItemInfo()
    {
        if (StoreManager.Instance.viewItem_store != null)
        {
            itemDetail.text = itemdata.ItemData[StoreManager.Instance.itemindex].DetailInfo;
        }
    }
    public void ItemInfo_Inven()
    {
        Debug.Log("인벤토리 오브젝트 = " + StoreManager.Instance.viewItem_Inven.name);
        if (StoreManager.Instance.viewItem_Inven != null)
        {
            Debug.Log("StoreManager.Instance.viewItem_Inven 감지");
            itemDetail_Inven.text = itemdata.ItemData[StoreManager.Instance.Itemindex_Inven].DetailInfo;
        }
    }
}
