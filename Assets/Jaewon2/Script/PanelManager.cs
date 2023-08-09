using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    public GameObject[] Panels;
    public GameObject[] Panels_Inven;
    public Text tagName;
    public Text tagName_Inven;
    public GameObject currentPanel;
    public GameObject currentPanel_Inven;
    public int panelNum;
    public int panelNum_Inven;
    public void Start()
    {
        panelNum = 0;
        panelNum_Inven = 0;
        currentPanel = Panels[0];
        currentPanel_Inven = Panels_Inven[0];
        for (int i = 0; i < Panels.Length; i++)
        {
            Panels[i].SetActive(false);
        }
        currentPanel.SetActive(true);
        tagName.text = "상점";

        for (int i = 0; i < Panels_Inven.Length; i++)
        {
            Panels_Inven[i].SetActive(false);
        }
        currentPanel_Inven.SetActive(true);
        tagName_Inven.text = "인벤토리";

    }
    public void PanelCon_Right()
    {
        if (panelNum == Panels.Length-1 || panelNum < 0)
        {
            panelNum = -1;
        }
        panelNum++;
        for (int i = 0; i < Panels.Length; i++)
        {
            Panels[i].SetActive(false);
        }
        currentPanel = Panels[panelNum];
        currentPanel.SetActive(true);
        switch (panelNum)
        {
            case 0:
                tagName.text = "상점";
                break;
            case 1:
                tagName.text = "상점";
                break;
            case -1:
                Debug.Log("푸드_Store 진입");
                tagName.text = "상점";
                break;
        }
        Debug.Log("판넬 = " + panelNum);
    }
    public void PanelCon_Left()
    {
        if (panelNum == 1-Panels.Length || panelNum > 0)
        {
            panelNum = 1;
        }

        panelNum--;
        for (int i = 0; i < Panels.Length; i++)
        {
            Panels[i].SetActive(false);
        }
        currentPanel = Panels[-panelNum];
        currentPanel.SetActive(true);
        switch (panelNum)
        {
            case 0:
                tagName.text = "상점";
                break;
            case 1:
                tagName.text = "상점";
                break;
            case -1:
                Debug.Log("푸드_Store 진입");
                tagName.text = "상점";
                break;
        }
        Debug.Log("판넬 = " + panelNum);
    }

    public void PanelCon_Right_Inven()
    {
        if (panelNum_Inven == Panels_Inven.Length-1 || panelNum_Inven < 0)
        {
            panelNum_Inven = -1;
        }
        panelNum_Inven++;
        for (int i = 0; i < Panels_Inven.Length; i++)
        {
            Panels_Inven[i].SetActive(false);
        }
        currentPanel_Inven = Panels_Inven[panelNum_Inven];
        currentPanel_Inven.SetActive(true);
        switch (panelNum_Inven)
        {
            case 0:
                tagName_Inven.text = "인벤토리";
                break;
            case 1:
                tagName_Inven.text = "인벤토리";
                break;
            case -1:
                tagName_Inven.text = "인벤토리";
                break;
        }
    }
    public void PanelCon_Left_Inven()
    {
        if (panelNum_Inven == 1-Panels_Inven.Length || panelNum_Inven > 0)
        {
            panelNum_Inven = 1;
        }

        panelNum_Inven--;
        for (int i = 0; i < Panels_Inven.Length; i++)
        {
            Panels_Inven[i].SetActive(false);
        }
        currentPanel_Inven = Panels_Inven[-panelNum_Inven];
        currentPanel_Inven.SetActive(true);
        switch (panelNum_Inven)
        {
            case 0:
                tagName_Inven.text = "인벤토리";
                break;
            case 1:
                tagName_Inven.text = "인벤토리";
                break;
            case -1:
                tagName_Inven.text = "인벤토리";
                break;
        }
    }

}
