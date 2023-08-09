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
        if (StoreManager.Instance.viewItem_store.activeSelf)
        {
            DeleteAllChild();
        }
        if (StoreManager.Instance.viewItem_Inven.activeSelf)
        {
            DeleteAllChild();
        }
        ViewObj();
    }
    public void ViewObj()
    {
        if (this.gameObject.name == "ItemButton(Clone)")
        {
            //this.GetComponent<GetData>().GetInfo();
            if (childitem.CompareTag("furniture"))
            {
                StoreManager.Instance.viewItem_store = GameObject.Instantiate(childitem, viewObj.transform);
                StoreManager.Instance.viewItem_store.gameObject.GetComponent<Transform>().localScale = viewScale;
            }
            if (childitem.CompareTag("toy"))
            {
                StoreManager.Instance.viewItem_store = GameObject.Instantiate(childitem, viewObj.transform);
                StoreManager.Instance.viewItem_store.gameObject.GetComponent<Transform>().localScale = new Vector3(800, 800, 800);
            }
            if (childitem.CompareTag("food"))
            {
                StoreManager.Instance.viewItem_store = GameObject.Instantiate(childitem, viewObj.transform);
                StoreManager.Instance.viewItem_store.gameObject.GetComponent<Transform>().localScale = new Vector3(100,100,100);
                if (StoreManager.Instance.itemindex == 27)
                {
                    StoreManager.Instance.viewItem_store.gameObject.GetComponent<Transform>().localPosition = new Vector3(0, -30, 0);
                    StoreManager.Instance.viewItem_store.gameObject.GetComponent<Transform>().localScale = new Vector3(100000, 100000, 100000);
                }
                if (StoreManager.Instance.itemindex == 23)
                {
                    StoreManager.Instance.viewItem_store.gameObject.GetComponent<Transform>().localPosition = new Vector3(0, -30, 0);
                    StoreManager.Instance.viewItem_store.gameObject.GetComponent<Transform>().localScale = new Vector3(2500, 2500, 2500);
                }
            }
        }
        if (this.gameObject.name == "ItemButton_Inven(Clone)")
        {
            this.GetComponent<GetData>().GetInfo_Inven();
            if (childitem_Inven.CompareTag("furniture"))
            {
                StoreManager.Instance.viewItem_Inven = GameObject.Instantiate(childitem_Inven, viewObj_Inven.transform);
                StoreManager.Instance.viewItem_Inven.gameObject.GetComponent<Transform>().localScale = viewScale;
            }
            if (childitem_Inven.CompareTag("toy"))
            {
                StoreManager.Instance.viewItem_Inven = GameObject.Instantiate(childitem_Inven, viewObj_Inven.transform);
                StoreManager.Instance.viewItem_Inven.gameObject.GetComponent<Transform>().localScale = new Vector3(800, 800, 800);
            }
            if (childitem_Inven.CompareTag("food"))
            {
                StoreManager.Instance.viewItem_Inven = GameObject.Instantiate(childitem_Inven, viewObj_Inven.transform);
                StoreManager.Instance.viewItem_Inven.gameObject.GetComponent<Transform>().localScale = new Vector3(100, 100, 100);
                if (StoreManager.Instance.Itemindex_Inven == 27)
                {
                    StoreManager.Instance.viewItem_Inven.gameObject.GetComponent<Transform>().localPosition = new Vector3(0, -30, 0);
                    StoreManager.Instance.viewItem_Inven.gameObject.GetComponent<Transform>().localScale = new Vector3(100000, 100000, 100000);
                    Debug.Log("인덱스 == 27");
                }
                if (StoreManager.Instance.Itemindex_Inven == 23)
                {
                    StoreManager.Instance.viewItem_Inven.gameObject.GetComponent<Transform>().localPosition = new Vector3(0, -30, 0);
                    StoreManager.Instance.viewItem_Inven.gameObject.GetComponent<Transform>().localScale = new Vector3(2500, 2500, 2500);
                    Debug.Log("인덱스 == 23");
                }
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
        {
            //Destroy(StoreManager.Instance.viewItem_store.gameObject);
            StoreManager.Instance.viewItem_store.SetActive(false);
        }
        if (this.CompareTag("Inven"))
        {
            //Destroy(StoreManager.Instance.viewItem_Inven.gameObject);
            StoreManager.Instance.viewItem_Inven.SetActive(false);
        }
    }
    /*public void OnTriggerEnter(Collider other)
    {
        Debug.Log(this.gameObject.name);
        if(this.gameObject.name == "ItemButton(Clone)")
        {
            GetButton();
            this.GetComponent<GetData>().GetInfo();
            this.GetComponent<GetData>().ItemInfo();
            this.GetComponent<ParticleManager>().PlayParticle();
            this.GetComponent<SFXCon>().GetButton();
        }
        if(this.gameObject.name == "ItemButton_Inven(Clone)")
        {
            GetButton();
            this.GetComponent<GetData>().GetInfo_Inven();
            this.GetComponent<GetData>().ItemInfo_Inven();
            this.GetComponent<ParticleManager>().PlayParticle();
            this.GetComponent<SFXCon>().GetButton();
        }
    }*/
}
