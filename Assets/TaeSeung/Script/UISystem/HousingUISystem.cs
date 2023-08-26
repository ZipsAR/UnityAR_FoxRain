using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.ARFoundation;

public class HousingUISystem : MonoBehaviour
{
    public static HousingUISystem Instance;
    public TutorialManager tutorial;

    [SerializeField]
    private GameObject menuPanel, HousingButtonPrefab;
    [SerializeField]
    private ItemDatabase itemdatabase;
    public Dictionary<int, GameObject> countlist = new();

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
        if (MapInfo.Instance.housingtutorial[0])
        {
            InteractEventManager.NotifyDialogShow("배치하고 싶은 가구버튼을 누르세요!");
           // StartCoroutine(tutorial.CameraRotateToDialog());
        }
     }

    private void Update()
    {
        if (MapInfo.Instance.housingtutorial[0])
        {
            MapInfo.Instance.housingtutorial[0] = false;
            StartCoroutine( tutorial.CameraRotateToDialog());
        }
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
                countlist.Add(objdata.id,newobj);
            }
 
        }   
    }

    public void EnableButton(bool flag)
    {
        foreach(var item in countlist)
        {
            int key = item.Key;
            int index = FileIOSystem.Instance.invendatabase.mydata.FindIndex(data => data.id == key);

            if (FileIOSystem.Instance.invendatabase.mydata[index].count != 0)
            {
                item.Value.GetComponent<Button>().interactable = flag;
            }

        }
    }



    public void ObjCountupdate(int id, int myindex)
    {
        GameObject obj = countlist[id];

        obj.GetComponentInChildren<TMP_Text>().text = FileIOSystem.Instance.invendatabase.mydata[myindex].count.ToString();
        if(FileIOSystem.Instance.invendatabase.mydata[myindex].count == 1)
        {
            obj.GetComponent<Button>().interactable = true;
        }

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
