using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetData : MonoBehaviour
{
    public ItemDatabase itemdata;
    Text itemDetail;
    private void Start()
    {
        itemDetail = GameObject.Find("Des1").GetComponent<Text>();
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
        itemDetail.text = itemdata.ItemData[StoreManager.Instance.itemindex].DetailInfo;
    }
}
