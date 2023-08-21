using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFirebase : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);

        // Incomplete code
        // FirebaseManager.Instance.SetInformation(new PetStatBase(99,88,77,66,5,30));
        // FirebaseManager.Instance.GetInformation<Int64>(ShowReceivedData);

        // Get/Set string from firebase
        // FirebaseManager.Instance.SetPetName("myPetName");
        // FirebaseManager.Instance.GetPetName(ShowSavedPetName);

        // Check data exist in path
        // FirebaseManager.Instance.IsDataExistInPath(CheckDataExistInPath, new List<string> { "pet", "stat" });

        // Get/Set pet stat
        PetStatBase testStat = new PetStatBase
        {
            cleanliness = 99,
            tiredness = 88,
            fullness = 77,
            exp = 66,
            level = 9,
            speed = 9
        };
        // FirebaseManager.Instance.SetPetStat(testStat);
        FirebaseManager.Instance.GetPetStat(GetPetStat);

    }

    private void GetPetStat(PetStatBase petStat)
    {
        Debug.Log("GetPetStat called");
        Debug.Log("petStat: " + petStat.exp);
    }
    
    private void CheckDataExistInPath(bool isExist, List<string> paths)
    {
        Debug.Log("is data exist in path: " + isExist);
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
