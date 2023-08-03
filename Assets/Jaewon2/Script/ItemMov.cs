using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemMov : MonoBehaviour
{
    [SerializeField] private int ItemNum;
    public bool isSelected;
    public GameObject childitem;
    public GameObject viewObj;
    public Queue<GameObject> childs;
    private Vector3 viewScale;
    private void Start()
    {
        isSelected = false;
        viewObj = GameObject.Find("ViewObj");
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
        if(StoreManager.Instance.viewItem != null) DeleteAllChild();
        ViewObj();
    }
    public void ViewObj()
    {
        StoreManager.Instance.viewItem = Instantiate(childitem, viewObj.transform);
        Debug.Log(StoreManager.Instance.viewItem.name);
        StoreManager.Instance.viewItem.GetComponent<Transform>().localScale = viewScale;
    }
    //�θ��� ������Ʈ�� ���̾ �κ��丮 �� ��츦 ����
    public void ViewRotate()
    {
        if (StoreManager.Instance.viewItem != null)
        {
            StoreManager.Instance.viewItem.transform.Rotate(new Vector3(0, 10, 0) * Time.deltaTime);
        }
    }
    public void DeleteAllChild()
    {
        Destroy(StoreManager.Instance.viewItem);
    }
}
