using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIButtonScript: Singleton<UIButtonScript>
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject DebugTextUI;

    [SerializeField]
    private GameObject GridPlane;


    public void DebuggingText(string newtext)
    {
        TMP_Text textmesh = DebugTextUI.GetComponent<TMP_Text>();
        textmesh.text = newtext;
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
}
