using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFirebase : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(3f);
        // FirebaseManager.Instance.SetInformation(new PetStatBase(99,88,77,66,5,30));
        // FirebaseManager.Instance.GetInformation<Int64>(ShowReceivedData);
        
        
        FirebaseManager.Instance.SetPetName("myPetName");
        // FirebaseManager.Instance.GetPetName(ShowSavedPetName);
    }

    private void ShowSavedPetName(string petName)
    {
        Debug.Log("saved pet name : " + petName);
    }
    
    
    private void ShowReceivedData(List<Int64> list)
    {
        Debug.Log("ShowReceivedData called");
        Debug.Log("received data count : " + list.Count);
        foreach (var value in list)
        {
            Debug.Log("read data: " + value);
        }
    }
}
