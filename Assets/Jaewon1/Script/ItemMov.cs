using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemMov : MonoBehaviour
{
    private bool isSelected;
    public GameObject childitem;
    private void Start()
    {
        isSelected = false;
    }
    private void Update()
    {
        Debug.Log(isSelected + this.gameObject.name);
        if (isSelected)
        {
            Debug.Log("È¸Àü");
            childitem.GetComponent<Transform>().Rotate(new Vector3(0, 10, 0) * Time.deltaTime * 80);
        }
    }
    public void GetButton()
    {
        isSelected = true;
    }
}
