using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    public GameObject[] Panels;
    public Text tagName;
    public GameObject currentPanel;
    public int panelNum;
    public void Start()
    {
        panelNum = 0;
        currentPanel = Panels[0];
        for (int i = 0; i < Panels.Length; i++)
        {
            Panels[i].SetActive(false);
        }
        currentPanel.SetActive(true);
        tagName.text = "Store-Furniture";
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
}
