using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
    public GameObject[] objList;
    private static StoreManager instance;
    public UICon uImanager;
    public ItemMov itemMov;
    public GameObject viewItem_store;
    public GameObject viewItem_Inven;
    public int itemindex;

    public static StoreManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<StoreManager>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(StoreManager).Name);
                    instance = singletonObject.AddComponent<StoreManager>();
                }
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }
    private void OnEnable()
    {
        Debug.Log("√ ±‚»≠");
        itemindex = -1;
    }
}
