using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = ZipsAR.Logger;

public class TestFirebase : MonoBehaviour
{
    /*
     
    private IEnumerator Start()
    {
        Logger.Log("start");
        yield return new WaitForSeconds(2f);

        // Load
        FirebaseDBManager.Instance.LoadDataFromAsync<PetStatBase>(
            new List<string>{ "testUser", "pet", "Cat", "stat" }, 
            JustCallback);
        
        // Save
        // FirebaseDBManager.Instance.SaveDataToAsync(
        //     new List<string>{ "testUser", "pet", "testpet", "stat" },
        //     new PetStatBase());
    }

    private float timer;
    private int idx;
    
    private void Update()
    {
        if (timer < 1f)
        {
            timer += Time.deltaTime;
        }
        else
        {
            Logger.Log(idx++);
            timer = 0;
        }
    }

    private void JustCallback(bool isDataFound, PetStatBase stat)
    {
        Logger.Log(isDataFound ? "data is found" : "data is not found");
    }
    
    */
}
