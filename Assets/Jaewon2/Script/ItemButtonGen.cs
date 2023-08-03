using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemButtonGen : MonoBehaviour
{
    public GameObject[] itemDBObjs;
    public GameObject[] ButtonGened;
    public GameObject buttonPanel;
    public GameObject[] parentPanel = new  GameObject[3];
    private void Start()
    {
        ButtonGened = new GameObject[itemDBObjs.Length];
        ButtonGen();
    }
    public void ButtonGen()
    {
        for (int i = 0; i < itemDBObjs.Length; i++)
        {
            if (itemDBObjs[i].CompareTag("furniture")) 
            {
                GameObject GenedPanel = GameObject.Instantiate(buttonPanel, parentPanel[0].transform);
                ButtonGened[i] = GameObject.Instantiate(itemDBObjs[i], GenedPanel.transform);
                ButtonGened[i].transform.localPosition = new Vector3(0, -40, -0);
                ButtonGened[i].transform.localScale = new Vector3(80, 80, 80);
                GenedPanel.GetComponent<ItemMov>().childitem = ButtonGened[i];
            }
            if (itemDBObjs[i].CompareTag("toy"))
            {
                GameObject GenedPanel = GameObject.Instantiate(buttonPanel, parentPanel[1].transform);
                ButtonGened[i] = GameObject.Instantiate(itemDBObjs[i], GenedPanel.transform);
                ButtonGened[i].transform.localPosition = new Vector3(0, -40, -0);
                ButtonGened[i].transform.localScale = new Vector3(300, 300, 300);
                GenedPanel.GetComponent<ItemMov>().childitem = ButtonGened[i];
            }
            if (itemDBObjs[i].CompareTag("food"))
            {
                GameObject GenedPanel = GameObject.Instantiate(buttonPanel, parentPanel[2].transform);
                ButtonGened[i] = GameObject.Instantiate(itemDBObjs[i], GenedPanel.transform);
                ButtonGened[i].transform.localPosition = new Vector3(0, -40, -0);
                ButtonGened[i].transform.localScale = new Vector3(300, 300, 300);
                GenedPanel.GetComponent<ItemMov>().childitem = ButtonGened[i];
            }
        }
    }
}
