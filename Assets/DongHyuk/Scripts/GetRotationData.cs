using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetRotationData : MonoBehaviour
{
    public GameObject LHand;
    public TMP_Text ScriptText;

    // Start is called before the first frame update
    void Start()
    {
        // ScriptText.text = "rotation";
    }

    // Update is called once per frame
    void Update()
    {
        var x = LHand.transform.eulerAngles.x;
        var y = LHand.transform.eulerAngles.y;
        var z = LHand.transform.eulerAngles.z;
        /* var x = UnityEditor.TransformUtils.GetInspectorRotation(LHand.transform).x;
        var y = UnityEditor.TransformUtils.GetInspectorRotation(LHand.transform).y;
        var z = UnityEditor.TransformUtils.GetInspectorRotation(LHand.transform).z;
        */
        ScriptText.text = "x: "+x+"\ny: "+y+"\nz: "+z;
    }
}
