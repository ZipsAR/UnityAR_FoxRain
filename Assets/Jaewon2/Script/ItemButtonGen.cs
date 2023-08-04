using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemButtonGen : MonoBehaviour
{
    public GameObject[] itemDBObjs;
    public GameObject[] ButtonGened;
    public GameObject buttonPanel;
    public GameObject buttonPanel_Inven;
    public GameObject[] parentPanel = new GameObject[3];
    public GameObject[] parentPanel_Inven = new GameObject[3];
    private void Start()
    {
        ButtonGened = new GameObject[itemDBObjs.Length];
        ButtonGen();
        //ButtonGen_Inven();
    }
    public void ButtonGen()
    {
        for (int i = 0; i < itemDBObjs.Length; i++)
        {
            if (itemDBObjs[i].CompareTag("furniture")) 
            {
                GameObject GenedPanel = GameObject.Instantiate(buttonPanel, parentPanel[0].transform);
                ButtonGened[i] = GameObject.Instantiate(itemDBObjs[i], GenedPanel.transform);
                ButtonGened[i].transform.localPosition = new Vector3(0, -30, -0);
                ButtonGened[i].transform.localScale = new Vector3(60, 60, 60);
                GenedPanel.GetComponent<ItemMov>().childitem = ButtonGened[i];
            }
            if (itemDBObjs[i].CompareTag("toy"))
            {
                GameObject GenedPanel = GameObject.Instantiate(buttonPanel, parentPanel[1].transform);
                ButtonGened[i] = GameObject.Instantiate(itemDBObjs[i], GenedPanel.transform);
                ButtonGened[i].transform.localPosition = new Vector3(0, -30, -0);
                ButtonGened[i].transform.localScale = new Vector3(600, 600, 600);
                GenedPanel.GetComponent<ItemMov>().childitem = ButtonGened[i];
            }
            if (itemDBObjs[i].CompareTag("food"))
            {
                GameObject GenedPanel = GameObject.Instantiate(buttonPanel, parentPanel[2].transform);
                ButtonGened[i] = GameObject.Instantiate(itemDBObjs[i], GenedPanel.transform);
                ButtonGened[i].transform.localPosition = new Vector3(0, -30, -0);
                ButtonGened[i].transform.localScale = new Vector3(600, 600, 600);
                GenedPanel.GetComponent<ItemMov>().childitem = ButtonGened[i];
            }
        }
    }
    public void ButtonGen_Inven()
    {
        for (int i = 0; i < itemDBObjs.Length; i++)
        {
            if (itemDBObjs[i].CompareTag("furniture"))
            {
                GameObject GenedPanel = GameObject.Instantiate(buttonPanel_Inven, parentPanel_Inven[0].transform);
                ButtonGened[i] = GameObject.Instantiate(itemDBObjs[i], GenedPanel.transform);
                ButtonGened[i].transform.localPosition = new Vector3(0, -30, -0);
                ButtonGened[i].transform.localScale = new Vector3(60, 60, 60);
                GenedPanel.GetComponent<ItemMov>().childitem_Inven = ButtonGened[i];
            }
            if (itemDBObjs[i].CompareTag("toy"))
            {
                GameObject GenedPanel = GameObject.Instantiate(buttonPanel_Inven, parentPanel_Inven[1].transform);
                ButtonGened[i] = GameObject.Instantiate(itemDBObjs[i], GenedPanel.transform);
                ButtonGened[i].transform.localPosition = new Vector3(0, -30, -0);
                ButtonGened[i].transform.localScale = new Vector3(600, 600, 600);
                GenedPanel.GetComponent<ItemMov>().childitem_Inven = ButtonGened[i];
            }
            if (itemDBObjs[i].CompareTag("food"))
            {
                GameObject GenedPanel = GameObject.Instantiate(buttonPanel_Inven, parentPanel_Inven[2].transform);
                ButtonGened[i] = GameObject.Instantiate(itemDBObjs[i], GenedPanel.transform);
                ButtonGened[i].transform.localPosition = new Vector3(0, -30, -0);
                ButtonGened[i].transform.localScale = new Vector3(600, 600, 600);
                GenedPanel.GetComponent<ItemMov>().childitem_Inven = ButtonGened[i];
            }
        }
    }

}
