using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemMov : MonoBehaviour
{
    [SerializeField] private int ItemNum;
    public bool isSelected;
    public GameObject childitem;
    public GameObject childitem_Inven;
    public GameObject viewObj;
    public GameObject viewObj_Inven;
    public Queue<GameObject> childs;
    private Vector3 viewScale;
    private void Start()
    {
        isSelected = false;
        viewObj = GameObject.Find("ViewObj");
        viewObj_Inven = GameObject.Find("ViewObj_Inven");
        viewScale = new Vector3(300, 300, 300);
        childs = null;
    }
    private void Update()
    {
        ViewRotate();
    }
    public void GetButton()
    {
        isSelected = !isSelected;
        if (StoreManager.Instance.viewItem_store != null)
        {
            DeleteAllChild();
        }
        if (StoreManager.Instance.viewItem_Inven != null)
        {
            DeleteAllChild();
        }
        ViewObj();
    }
    public void ViewObj()
    {
        Debug.Log(this.gameObject.name);
        if (this.gameObject.name == "ItemButton(Clone)")
        {
            if (childitem.CompareTag("furniture"))
            {
                StoreManager.Instance.viewItem_store = GameObject.Instantiate(childitem, viewObj.transform);
                StoreManager.Instance.viewItem_store.gameObject.GetComponent<Transform>().localScale = viewScale;
            }
            else
            {
                StoreManager.Instance.viewItem_store = GameObject.Instantiate(childitem, viewObj.transform);
                StoreManager.Instance.viewItem_store.gameObject.GetComponent<Transform>().localScale = new Vector3(800,800,800);
            }
        }
        if (this.gameObject.name == "ItemButton_Inven(Clone)")
        {
            if (childitem.CompareTag("furniture"))
            {
                StoreManager.Instance.viewItem_Inven = GameObject.Instantiate(childitem, viewObj_Inven.transform);
                StoreManager.Instance.viewItem_Inven.gameObject.GetComponent<Transform>().localScale = viewScale;
            }
            else
            {
                StoreManager.Instance.viewItem_Inven = GameObject.Instantiate(childitem, viewObj_Inven.transform);
                StoreManager.Instance.viewItem_Inven.gameObject.GetComponent<Transform>().localScale = new Vector3(800, 800, 800);
            }
        }
    }
    //부모의 오브젝트의 레이어가 인벤토리 일 경우를 나눔
    public void ViewRotate()
    {
        if (StoreManager.Instance.viewItem_store != null)
        {
            StoreManager.Instance.viewItem_store.transform.Rotate(new Vector3(0, 10, 0) * Time.deltaTime);
        }
        if (StoreManager.Instance.viewItem_Inven != null)
        {
            StoreManager.Instance.viewItem_Inven.transform.Rotate(new Vector3(0, 10, 0) * Time.deltaTime);
        }
    }
    public void DeleteAllChild()
    {
        if (this.CompareTag("Store"))
            Destroy(StoreManager.Instance.viewItem_store.gameObject);
        if (this.CompareTag("Inven"))
            Destroy(StoreManager.Instance.viewItem_Inven.gameObject);
    }
}
