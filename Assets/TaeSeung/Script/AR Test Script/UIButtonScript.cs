using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIButtonScript: Singleton<UIButtonScript>
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject DebugTextUI, DebugTextinvenUI;
    
    [SerializeField]
    private ObjectDatabaseSO database;
    private int selectedObjectindex = -2;

    [SerializeField]
    Camera arcamera;

    [SerializeField]
    private GameObject GridPlane;


    public void DebuggingText(object newtext)
    {
        TMP_Text textmesh = DebugTextUI.GetComponent<TMP_Text>();
        textmesh.text = newtext.ToString();
    }

    public void DebuggingTextInven(object newtext)
    {
        TMP_Text textmesh = DebugTextinvenUI.GetComponent<TMP_Text>();
        textmesh.text = newtext.ToString();
    }


    public void ONHOUSING()
    {
        if (!GridPlane.activeSelf)
            GridPlane.SetActive(true);
        else
            GridPlane.SetActive(false);

        DebuggingText("hosuing!");
    }


    public void ONTEXIT()
    {
        Application.Quit();
    }


    public void MakeNewObj(int ID)
    {
        DebuggingTextInven(ID);
        selectedObjectindex = database.objectsData.FindIndex(data => data.ID == ID);
        if(selectedObjectindex < 0)
        {
            DebuggingTextInven("WrongIndex");
        }
        if (database.objectsData[selectedObjectindex].ObjectCount <= 0)
        {
            DebuggingTextInven("NoObject");
        }

        GameObject newobject = Instantiate(database.objectsData[selectedObjectindex].Prefab);
        newobject.transform.localScale = newobject.transform.localScale * 0.125f;
        Vector3 position = arcamera.transform.position;
        position.z += 0.15f;
        newobject.transform.position = position;


    }

}
