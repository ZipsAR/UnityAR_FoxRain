using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGen : MonoBehaviour
{
    [SerializeField]
    ItemDB itemdb;
    [SerializeField]
    GameObject Panel_furniture;
    [SerializeField]
    GameObject Panel_toy;
    [SerializeField]
    GameObject Panel_food;
    [SerializeField]
    private GameObject[] itembutttons;

    private void Start()
    {
        for (int a = 0; a < 2; a++)
        {
            if (a == 1)
            {
                for (int i = 0; i < itemdb.ItemName.Length; i++)
                {
                    if (itemdb.Item[i].CompareTag("furniture"))
                    {
                        GameObject getItem = itemdb.Item[i];
                        GameObject Gen = GameObject.Instantiate(itembutttons[i], Panel_furniture.transform);
                    }
                    if (itemdb.Item[i].CompareTag("toy"))
                    {
                        GameObject getItem = itemdb.Item[i];
                        GameObject Gen = GameObject.Instantiate(itembutttons[i], Panel_toy.transform);
                    }
                    if (itemdb.Item[i].CompareTag("food"))
                    {
                        GameObject getItem = itemdb.Item[i];
                        GameObject Gen = GameObject.Instantiate(itembutttons[i], Panel_food.transform);
                    }
                }
            }
        }
    }
}
