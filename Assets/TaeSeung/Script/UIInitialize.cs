using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIInitialize : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private ObjectDatabaseSO database;
    [SerializeField]
    private GameObject menuPanel;
    public List<GameObject> countlist;

    void Start()
    {
        print(menuPanel.transform.childCount);

        for (int i = 0; i < menuPanel.transform.childCount-1; i++)
        {
            GameObject tmptext = menuPanel.transform.GetChild(i).gameObject;
            
            tmptext.GetComponentInChildren<TMP_Text>().text = "" + database.objectsData[i].ObjectCount;
            if(database.objectsData[i].ObjectCount <= 0)
            {
                tmptext.GetComponent<Button>().interactable = false;
            }
            countlist.Add(tmptext);
        }
    }

    public void Resetsetting()
    {
        for (int i=0; i< database.objectsData.Count; i++)
        {
            database.objectsData[i].ObjectCount = 10;
            countlist[i].GetComponentInChildren<TMP_Text>().text = ""+10;
            countlist[i].GetComponent<Button>().interactable = true;
        }
    }

}
