using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;


public class HousingUISystem : MonoBehaviour
{
    public static HousingUISystem Instance;
    public TutorialManager Tutorial;
    public Dictionary<int, GameObject> d_Count_Object = new();

    [SerializeField]
    private GameObject _menuPanel, _housingButtonPrefab;
    [SerializeField]
    private ItemDatabase _itemDatabase;
    


    private void Start()
    { 
        if (HousingUISystem.Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            InitializeUI();
        }
        if (MapInfo.Instance.l_IsHousingTutorialFinish[0])
        {
            InteractEventManager.NotifyDialogShow("배치하고 싶은 가구버튼을 누르세요!");
           // StartCoroutine(Tutorial.CameraRotateToDialog());
        }
     }

    private void Update()
    {
        if (MapInfo.Instance.l_IsHousingTutorialFinish[0])
        {
            MapInfo.Instance.l_IsHousingTutorialFinish[0] = false;
            StartCoroutine( Tutorial.CameraRotateToDialog());
        }
    }


    public void InitializeUI()
    {
        foreach (MyData objdata in FileIOSystem.Instance.InvenDatabase.mydata) {
       
            int idindex = _itemDatabase.ItemData.FindIndex(data => data.ID == objdata.id);

            if (idindex >= 0 && _itemDatabase.ItemData[idindex].itemCategory == ItemData.ItemCategory.Funiture)
            {
                GameObject newobj = Instantiate(_housingButtonPrefab, _menuPanel.transform);
                newobj.GetComponent<Button>().onClick.AddListener(() => PlacementSystem.Instance.StartPlacement(objdata.id));

                GameObject previewobj = Instantiate(_itemDatabase.ItemData[idindex].Prefab, newobj.transform.Find("GameObject"));
                previewobj.GetComponent<XRGrabInteractable>().enabled = false;
                previewobj.transform.localScale *= 32;

                newobj.GetComponentInChildren<TMP_Text>().text = objdata.count.ToString();
                if (objdata.count <= 0) newobj.GetComponent<Button>().interactable = false;
                d_Count_Object.Add(objdata.id,newobj);
            }
 
        }   
    }

    public void EnableButton(bool flag)
    {
        foreach(var item in d_Count_Object)
        {
            int key = item.Key;
            int index = FileIOSystem.Instance.InvenDatabase.mydata.FindIndex(data => data.id == key);
            if (FileIOSystem.Instance.InvenDatabase.mydata[index].count != 0) item.Value.GetComponent<Button>().interactable = flag;
        }
    }



    public void ObjCountupdate(int id, int myIndex)
    {
        GameObject obj = d_Count_Object[id];
        obj.GetComponentInChildren<TMP_Text>().text = FileIOSystem.Instance.InvenDatabase.mydata[myIndex].count.ToString();
        if(FileIOSystem.Instance.InvenDatabase.mydata[myIndex].count == 1) obj.GetComponent<Button>().interactable = true;
    }

    public void DebuggingText(object newtext)
    {
        //TMP_Text textmesh = DebugTextUI.GetComponent<TMP_Text>();
        //textmesh.text = newtext.ToString();
    }

    public void ONTEXIT() => Application.Quit();
    


}
