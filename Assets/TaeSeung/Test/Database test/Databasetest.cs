using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Databasetest : MonoBehaviour
{
    // Start is called before the first frame update

    public ItemDatabase itemdatabase;

    void Start()
    {
        foreach (KeyValuePair<int,ItemData> items in itemdatabase.Itemdictionary)
        {
            print(items);
        }
    }

    // Update is called once per frame
    void Update()
    {
        print("asd!: "+itemdatabase.Itemdictionary[1000].funitureCategory);

    }





}
