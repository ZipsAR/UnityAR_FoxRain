using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = ZipsAR.Logger;

public class PasswordDot : MonoBehaviour
{
    private enum PasswordNum
    {
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9
    }
    
    [SerializeField] private PasswordNum passwordNum;
    private PasswordController controller;
    private Material defaultMaterial;
    private Material selectedMaterial;
    private int fingerId = -1;
    
    public bool isSelected { get; private set; }
    
    public void Init(PasswordController controller, int num, Material defaultMaterial, Material selectedMaterial)
    {
        if(num is < 1 or > 9)
            throw new ArgumentOutOfRangeException("num", "num must be between 1 and 9");
        passwordNum = (PasswordNum) num;
        this.controller = controller;
        this.defaultMaterial = defaultMaterial;
        this.selectedMaterial = selectedMaterial;
    }
    
    public void SetActiveFingerId(int fingerId)
    {
        this.fingerId = fingerId;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(isSelected) return;
        
        if (other.name == "index003.Col" && (fingerId == -1 || fingerId == other.gameObject.GetInstanceID()))
        {
            Logger.Log("PasswordDot trigger enter: " + passwordNum);
            GetComponent<MeshRenderer>().material = selectedMaterial;
            controller.NotifyFingerIdSelected(other.gameObject.GetInstanceID());
            controller.AddSelectedPasswordIndex((int) passwordNum);
            isSelected = true;
        }
    }

    public void UnSelected()
    {
        isSelected = false;
        GetComponent<MeshRenderer>().material = defaultMaterial;
        fingerId = -1;
    }
}
