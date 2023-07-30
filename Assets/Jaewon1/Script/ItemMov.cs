using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemMov : MonoBehaviour
{
    public bool isSelected;
    public GameObject childitem;
    private void Start()
    {
        isSelected = false;
    }
    private void Update()
    {
        if (isSelected)
        {
            childitem.GetComponent<Transform>().Rotate(new Vector3(0, 10, 0) * Time.deltaTime * 40);
        }
        if (!isSelected)
        {
            childitem.GetComponent<Transform>().rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
    }
    public void GetButton()
    {
        for (int i=0;i< StoreManager.Instance.objList.Length; i++)
        {
            Debug.Log(StoreManager.Instance.objList.Length);
            StoreManager.Instance.objList[i].GetComponent<ItemMov>().isSelected = false;
            Debug.Log(StoreManager.Instance.objList[i].name + "="+ StoreManager.Instance.objList[i].GetComponent<ItemMov>().isSelected);
        }
        isSelected = !isSelected;
    }
}
