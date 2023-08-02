using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public GameObject[] Panels;
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

    }
}
