using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandUIController : MonoBehaviour
{
    public GameObject handCanvasObj1,handCanvasObj2,handCanvasObj3;
    private GameObject arCam;
    private Vector3 handForward, handRight, handVertical;
    private RaycastHit handUIRay;

    private void Awake()
    {
        handCanvasObj1.GetComponent<RectTransform>().localScale = new Vector3(0.001f, 0.001f, 0.001f);
        handCanvasObj2.GetComponent<RectTransform>().localScale = new Vector3(0.001f, 0.001f, 0.001f);
        handCanvasObj3.GetComponent<RectTransform>().localScale = new Vector3(0.001f, 0.001f, 0.001f);
        handCanvasObj1.SetActive(false);
        handCanvasObj2.SetActive(false);
        handCanvasObj3.SetActive(false);
        arCam = GameManager.Instance.player.gameObject;
    }

    private void Update()
    {
        /*handForward = GameManager.Instance.leftHand.transform.forward;
        handRight = GameManager.Instance.leftHand.transform.right;
        handVertical = Vector3.Cross(handForward,handRight);
        //Debug.DrawRay(GameManager.Instance.leftHand.transform.position, GameManager.Instance.leftHand.transform.localRotation * Vector3.right * 1000.0f, Color.blue, 1000f);
        //Debug.DrawRay(arCam.transform.forward, arCam.transform.localRotation * Vector3.forward * 1000.0f, Color.green, 1000f);
        float angle = Vector3.SignedAngle(arCam.transform.forward, handVertical, transform.forward);
        Debug.Log(angle+", "+arCam.transform.forward+", "+handVertical+", "+handRight+", "+handForward);
        if (angle > 90.0 && angle < 180.0) handCanvasObj1.SetActive(true);
        else handCanvasObj1.SetActive(false);*/
        handUIVisible();
        Debug.DrawRay(transform.position + transform.forward.normalized * 0.15f, transform.forward * (-2.0f), Color.green);
        //Debug.Log(handUIRay.collider.name);
    }
    private bool isCollidingCam()
    {
        Physics.Raycast(transform.position + transform.forward.normalized * 0.15f, transform.forward * (-1.0f), out handUIRay, 2.0f);
        if(handUIRay.collider.name == "AR Camera") return true;
        else return false;
    }
    private void handUIVisible()
    {
        if(isCollidingCam())
        {
            handCanvasObj1.SetActive(true);
        	/*//AR Camera
            Debug.Log(handUIRay.collider.name);*/
        }
        else
        {
            handCanvasObj1.SetActive(false);
        }
    }
}
