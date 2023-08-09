using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class HousingUISystem : MonoBehaviour
{
    public static HousingUISystem Instance;


    [SerializeField]
    private GameObject menuPanel, HousingButtonPrefab;
    [SerializeField]
    private ItemDatabase itemdatabase;
    public List<GameObject> countlist;

    [SerializeField]
    private GameObject prefab;


    private void Start()
    {
        if (HousingUISystem.Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            InitializeUI();
        }
    }

    private void Update()
    {

    }


    public void InitializeUI()
    {
        foreach (MyData objdata in FileIOSystem.Instance.invendatabase.mydata) {
                int idindex = itemdatabase.ItemData.FindIndex(data => data.ID == objdata.id);

            if (idindex >= 0 && itemdatabase.ItemData[idindex].itemCategory == ItemData.ItemCategory.Funiture)
            {
                GameObject newobj = Instantiate(HousingButtonPrefab, menuPanel.transform);
                newobj.GetComponent<Button>().onClick.AddListener(() => PlacementSystem.Instance.StartPlacement(objdata.id));

                GameObject previewobj = Instantiate(itemdatabase.ItemData[idindex].Prefab, newobj.transform.Find("GameObject"));
                previewobj.GetComponent<XRGrabInteractable>().enabled = false;
                previewobj.transform.localScale *= 32;

                newobj.GetComponentInChildren<TMP_Text>().text = objdata.count.ToString();
                if (objdata.count <= 0) newobj.GetComponent<Button>().interactable = false;
                countlist.Add(newobj);
            }
 
        }   
    }

    public void EnableButton(bool flag)
    {
        for(int i=0; i<countlist.Count; i++)
            countlist[i].GetComponent<Button>().interactable = flag;

    }



    public void ObjCountupdate(int id)
    {
        GameObject obj = countlist[id];
        obj.GetComponentInChildren<TMP_Text>().text = FileIOSystem.Instance.invendatabase.mydata[id].count.ToString();
    

    }

    public void DebuggingText(object newtext)
    {
        //TMP_Text textmesh = DebugTextUI.GetComponent<TMP_Text>();
        //textmesh.text = newtext.ToString();
    }

    public void ONTEXIT()
    {
        Application.Quit();
    }


}
