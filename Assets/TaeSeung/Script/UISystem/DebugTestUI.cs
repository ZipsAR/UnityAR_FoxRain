using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugTestUI : Singleton<DebugTestUI>
{
    // Start is called before the first frame update

    [SerializeField]
    private List<TMP_Text> Debugtext;


    public void DebugTextSize(int size, int index)
    {
        Debugtext[index].fontSize = size;
    }

    public void DebugText(object obj, int index)
    {
        Debugtext[index].text = obj.ToString();
    }


    public void HitsDebugText(RaycastHit[] hits, int index)
    {
        Debugtext[index].text = "";
        for (int i = 0; i < hits.Length; i++)
        {
            Debugtext[index].text += hits[i].transform.name + " : " + hits[i].point;
        }
    }

}
