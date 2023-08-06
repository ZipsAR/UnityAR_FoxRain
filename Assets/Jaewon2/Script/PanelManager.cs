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
        tagName.text = "Store-Furniture";

        for (int i = 0; i < Panels_Inven.Length; i++)
        {
            Panels_Inven[i].SetActive(false);
        }
        currentPanel_Inven.SetActive(true);
        tagName_Inven.text = "Inventory-Furniture";

    }
    public void PanelCon_Right()
    {
        panelNum++;
        for (int i = 0; i < Panels.Length; i++)
        {
            Panels[i].SetActive(false);
        }
        currentPanel = Panels[panelNum];
        currentPanel.SetActive(true);
        if (panelNum == Panels.Length-1)
        {
            panelNum = -1;
        }
        switch (panelNum)
        {
            case 0:
                tagName.text = "Store-Furniture";
                break;
            case 1:
                tagName.text = "Store-Toy";
                break;
            case 2:
                tagName.text = "Store-Food";
                break;

        }
    }
    public void PanelCon_Right_Inven()
    {
        panelNum_Inven++;
        for (int i = 0; i < Panels_Inven.Length; i++)
        {
            Panels_Inven[i].SetActive(false);
        }
        currentPanel_Inven = Panels_Inven[panelNum_Inven];
        currentPanel_Inven.SetActive(true);
        if (panelNum_Inven == Panels_Inven.Length - 1)
        {
            panelNum_Inven = -1;
        }
        switch (panelNum_Inven)
        {
            case 0:
                tagName_Inven.text = "Inventory-Furniture";
                break;
            case 1:
                tagName_Inven.text = "Inventory-Toy";
                break;
            case 2:
                tagName_Inven.text = "Inventory-Food";
                break;

        }
    }

}
