using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemButtonGen : MonoBehaviour
{
    public GameObject[] itemDBObjs;
    public GameObject[] ButtonGened;
    public GameObject[] ButtonGened_Inven;
    public GameObject buttonPanel;
    public GameObject buttonPanel_Inven;
    public GameObject[] parentPanel = new GameObject[3];
    public GameObject[] parentPanel_Inven = new GameObject[3];
    public Sprite buttonUI;
    private void Start()
    {
        ButtonGened = new GameObject[itemDBObjs.Length];
        ButtonGened_Inven = new GameObject[itemDBObjs.Length];
        if (this.CompareTag("Store"))
        {
            ButtonGen();
        }
        if (this.CompareTag("Inven"))
        {
            ButtonGen_Inven();
        }
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
                ButtonGened[i].transform.localScale = new Vector3(80, 80, 80);
                GenedPanel.GetComponent<ItemMov>().childitem = ButtonGened[i];
            }
            if (itemDBObjs[i].CompareTag("toy"))
            {
                GameObject GenedPanel = GameObject.Instantiate(buttonPanel, parentPanel[1].transform);
                ButtonGened[i] = GameObject.Instantiate(itemDBObjs[i], GenedPanel.transform);
                ButtonGened[i].transform.localPosition = new Vector3(0, -30, -0);
                ButtonGened[i].transform.localScale = new Vector3(300, 300, 300);
                GenedPanel.GetComponent<ItemMov>().childitem = ButtonGened[i];
            }
            if (itemDBObjs[i].CompareTag("food"))
            {
                GameObject GenedPanel = GameObject.Instantiate(buttonPanel, parentPanel[2].transform);
                ButtonGened[i] = GameObject.Instantiate(itemDBObjs[i], GenedPanel.transform);
                ButtonGened[i].transform.localPosition = new Vector3(0, -30, -0);
                ButtonGened[i].transform.localScale = new Vector3(60, 60, 60);
                if (i == 27)
                {
                    ButtonGened[i].GetComponent<Transform>().localPosition = new Vector3(0, -30, 0);
                    ButtonGened[i].GetComponent<Transform>().localScale = new Vector3(1000, 1000, 1000);
                    Debug.Log("¿Œµ¶Ω∫ == 27");
                }
                if (i == 25)
                {
                    ButtonGened[i].GetComponent<Transform>().localPosition = new Vector3(0, -30, 0);
                    ButtonGened[i].GetComponent<Transform>().localScale = new Vector3(60000, 60000, 60000);
                    Debug.Log("¿Œµ¶Ω∫ == 25");
                }

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
                ButtonGened_Inven[i] = GameObject.Instantiate(itemDBObjs[i], GenedPanel.transform);
                ButtonGened_Inven[i].transform.localPosition = new Vector3(0, -30, -0);
                ButtonGened_Inven[i].transform.localScale = new Vector3(80, 80, 80);
                GenedPanel.GetComponent<ItemMov>().childitem_Inven = ButtonGened_Inven[i];
            }
            if (itemDBObjs[i].CompareTag("toy"))
            {
                GameObject GenedPanel = GameObject.Instantiate(buttonPanel_Inven, parentPanel_Inven[1].transform);
                ButtonGened_Inven[i] = GameObject.Instantiate(itemDBObjs[i], GenedPanel.transform);
                ButtonGened_Inven[i].transform.localPosition = new Vector3(0, -30, -0);
                ButtonGened_Inven[i].transform.localScale = new Vector3(300, 300, 300);
                GenedPanel.GetComponent<ItemMov>().childitem_Inven = ButtonGened_Inven[i];
            }
            if (itemDBObjs[i].CompareTag("food"))
            {
                GameObject GenedPanel = GameObject.Instantiate(buttonPanel_Inven, parentPanel_Inven[2].transform);
                ButtonGened_Inven[i] = GameObject.Instantiate(itemDBObjs[i], GenedPanel.transform);
                ButtonGened_Inven[i].transform.localPosition = new Vector3(0, -30, -0);
                ButtonGened_Inven[i].transform.localScale = new Vector3(60, 60, 60);
                if (i == 27)
                {
                    ButtonGened_Inven[i].GetComponent<Transform>().localScale = new Vector3(1000, 1000, 1000);
                    Debug.Log("¿Œµ¶Ω∫ == 27");
                }
                if (i == 25)
                {
                    ButtonGened_Inven[i].GetComponent<Transform>().localScale = new Vector3(60000, 60000, 60000);
                    Debug.Log("¿Œµ¶Ω∫ == 25");
                }
                GenedPanel.GetComponent<ItemMov>().childitem_Inven = ButtonGened_Inven[i];
            }
        }
    }

}
