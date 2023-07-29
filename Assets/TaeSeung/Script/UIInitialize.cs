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

    //�Ͽ�¡ ��ư�� ������ �г�, �Ͽ�¡ ��ư�� ������
    [SerializeField]
    private GameObject menuPanel, HousingButtonPrefab;


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


}
