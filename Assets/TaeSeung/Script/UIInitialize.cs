using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIInitialize : Singleton<UIInitialize>
{
    // Start is called before the first frame update
    [SerializeField]
    private ObjectDatabaseSO database;

    //하우징 버튼이 부착될 패널, 하우징 버튼의 프리팹
    [SerializeField]
    private GameObject menuPanel, HousingButtonPrefab;


    [SerializeField]
    private GameObject DebugTextUI;

    [SerializeField]
    private GameObject GridPlane;





    public List<GameObject> countlist;

    void Start()
    {
        foreach (ObjectData objdata in database.objectsData) {
            GameObject newobj = Instantiate(HousingButtonPrefab, menuPanel.transform);
            newobj.GetComponent<Button>().onClick.AddListener(() => PlacementSystem.Instance.StartPlacement(objdata.ID));
            newobj.GetComponentInChildren<Image>().color = Random.ColorHSV();
            newobj.GetComponentInChildren<TMP_Text>().text = objdata.ObjectCount.ToString();
            if (objdata.ObjectCount <= 0) newobj.GetComponent<Button>().interactable = false;
            countlist.Add(newobj);
        }   
    }

    public void ObjCountupdate(int id)
    {
        GameObject obj = countlist[id];
        obj.GetComponentInChildren<TMP_Text>().text = database.objectsData[id].ObjectCount.ToString();

    }


    public void DebuggingText(object newtext)
    {
        TMP_Text textmesh = DebugTextUI.GetComponent<TMP_Text>();
        textmesh.text = newtext.ToString();
    }


    public void ONHOUSING()
    {

        //평면 인식 상태
        if (InputManager.Instance.GetSelectedMapPositionbyVision().Length > 0) {

            //Normal mode
            print("find plane");

        }
        else
        {

            print("ignore");
            //무시함
        }

        DebuggingText("hosuing!");
    }


    public void ONTEXIT()
    {
        Application.Quit();
    }


}
