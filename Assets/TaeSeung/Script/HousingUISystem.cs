using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HousingUISystem : Singleton<HousingUISystem>
{
    // Start is called before the first frame update
    [SerializeField]
    private ObjectDatabaseSO database;

    //�Ͽ�¡ ��ư�� ������ �г�, �Ͽ�¡ ��ư�� ������
    [SerializeField]
    private GameObject menuPanel, HousingButtonPrefab;

    [SerializeField]
    private GameObject DebugTextUI;

    [SerializeField]
    private GameObject GridPlane;
    public List<GameObject> countlist;

    private bool housingmode = true;


    public void InitializeUI()
    {
        
        foreach (MyData objdata in FileIOSystem.Instance.invendatabase.mydata) {

            GameObject newobj = Instantiate(HousingButtonPrefab, menuPanel.transform);
            newobj.GetComponent<Button>().onClick.AddListener(() => PlacementSystem.Instance.StartPlacement(objdata.id));
            newobj.GetComponentInChildren<Image>().color = Random.ColorHSV();
            newobj.GetComponentInChildren<TMP_Text>().text = objdata.count.ToString();
            if (objdata.count <= 0) newobj.GetComponent<Button>().interactable = false;
            countlist.Add(newobj);
    
        }   
    }

    public void ObjCountupdate(int id)
    {
        GameObject obj = countlist[id];
        obj.GetComponentInChildren<TMP_Text>().text = FileIOSystem.Instance.invendatabase.mydata[id].count.ToString();

    }


    public void DebuggingText(object newtext)
    {
        TMP_Text textmesh = DebugTextUI.GetComponent<TMP_Text>();
        textmesh.text = newtext.ToString();
    }


    public void ONHOUSING()
    {
        if (housingmode) {
            PlacementSystem.Instance.ProtectGrib();
            housingmode = false;
            //씬 넘어가는 ㅇㅇ

        }
        else
        {
            PlacementSystem.Instance.ReleaseGrib();
            housingmode = true;
        }

        DebuggingText("hosuing!");
    }


    public void ONTEXIT()
    {
        Application.Quit();
    }


}
