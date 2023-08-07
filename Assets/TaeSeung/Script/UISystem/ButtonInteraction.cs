using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInteraction : MonoBehaviour
{

    private Button parent;

    private void Start()
    {
        parent = this.transform.parent.GetComponent<Button>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("index003"))
        {
            parent.image.color = parent.colors.pressedColor;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name.Contains("index003"))
        {
            parent.image.color = parent.colors.normalColor;
            parent.onClick.Invoke();
        }
        
    }
}
