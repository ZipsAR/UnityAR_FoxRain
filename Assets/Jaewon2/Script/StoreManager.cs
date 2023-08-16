using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
    public GameObject[] objList;
    public static StoreManager Instance;
    public UICon uImanager;
    public ItemMov itemMov;
    public GetData getData;
    public GameObject viewItem_store;
    public GameObject viewItem_Inven;
    public int itemindex;
    public int Itemindex_Inven;
    public SFXCon sfx;
    public PanelManager panelManager;
    private void OnEnable()
    {
        Instance = this;
        Debug.Log("√ ±‚»≠");
        itemindex = -1;
    }
}
