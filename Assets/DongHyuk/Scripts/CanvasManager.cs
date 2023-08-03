using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public GameObject tempObj;
    public HandController handThis;
    private GameObject handCanvas;
    private Vector3 canvasPos;
    // Start is called before the first frame update
    void Start()
    {
        handThis = GameManager4Test.Instance.leftHand;
        canvasPos = handThis.transform.position + new Vector3(0f,0f,0f);
        handCanvas = Instantiate(tempObj,canvasPos,Quaternion.identity,GameManager.Instance.leftHand.transform);
    }   

    // Update is called once per frame
    void Update()
    {
        //handCanvas.transform.position = GameManager4Test.Instance.leftHand.transform.position;
        //handCanvas.transform.localPosition = new Vector3(-0.15f,0f,0.2f);
        //ts.transform.rotation = Quaternion.Euler(GameManager4Test.Instance.leftHand.transform.right);
    }
}
