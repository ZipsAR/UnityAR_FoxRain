using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;

// Ŀ�� ��ġ�� ���� world ��ǥ�迡���� �� ��ġ�� ������ִ� Ŭ���� + ���콺 ��ǲ �̺�Ʈ�� ó�����ִ� Ŭ����

public class InputManager:MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Camera sceneCamera;
    private Vector3 lastPosition;
    private Vector3 FirstPosition;

    [SerializeField]
    private LayerMask placementLayermask;
    private bool hitting;
    public GameObject Plane;

    [SerializeField]
    private GameManager test;

    private Ray ray;

    private GameObject aa;
    private Vector3 forward;

    public event Action OnClicked, OnExit;

    private void Start()
    {
        FirstPosition = sceneCamera.transform.position;
        //print(sceneCamera.transform.parent.name);
        aa = sceneCamera.transform.parent.Find("Trackables").gameObject;

    }

    private void Update()
    {
        //print("ī�޶� ��ġ+ " + sceneCamera.transform.forward);
        /*
        if (Input.GetMouseButtonDown(0))
            OnClicked?.Invoke();
        if (Input.GetKeyDown(KeyCode.Escape))
            OnExit?.Invoke();
        */
        //GetSelectedMapPosition();
        angle();

    }

    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

    
    /// <summary>
    /// object ��ġ�� �������� �׸��� �������� �������ִ� �Լ�
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>

    public Vector3 GetSelectedMapPositionbyObject(Transform transform)
    {
        ray.origin = transform.position;
        ray.direction = Plane.transform.up * -1;

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, placementLayermask))
        {
            // UIButtonScript.Instance.DebuggingText("hitting : " +hit.collider.gameObject.name);
            lastPosition = hit.point;
            hitting = true;
        }
        else
            hitting = false;

        return lastPosition;
    }


    public void angle()
    {
        /*
        Vector3 angle = aa.GetComponentsInChildren<Transform>()[2].rotation.eulerAngles;


        if(angle.y%360 >= 0 && angle.y % 360<90)
            UIButtonScript.Instance.DebuggingText("090: "+aa.GetComponentsInChildren<Transform>()[2].rotation.eulerAngles);
        else if (angle.y % 360 >= 90 && angle.y % 360 < 180)
            UIButtonScript.Instance.DebuggingText("90180: " + aa.GetComponentsInChildren<Transform>()[2].rotation.eulerAngles);
        else if (angle.y % 360 >= 180 && angle.y % 360 < 240)
            UIButtonScript.Instance.DebuggingText("180240: " + aa.GetComponentsInChildren<Transform>()[2].rotation.eulerAngles);
        else if (angle.y % 360 >= 240 && angle.y % 360 < 360)
            UIButtonScript.Instance.DebuggingText("240360: " + aa.GetComponentsInChildren<Transform>()[2].rotation.eulerAngles);
        */



    }


    public bool ishit() => hitting;




    [Obsolete]
    public Vector3 GetSelectedMapPositionbyVision()
    {
        //Vector3 mousePos = Input.mousePosition;
        //mousePos.z = sceneCamera.nearClipPlane;
        //ray = sceneCamera.ScreenPointToRay(mousePos);

        ray.origin = sceneCamera.transform.position;
        ray.direction = sceneCamera.transform.forward;


        /*
        ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        */

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, placementLayermask))
        {
            // UIButtonScript.Instance.DebuggingText("hitting : " +hit.collider.gameObject.name);
            lastPosition = hit.point;
            hitting = true;
        }
        else
            hitting = false;

        return lastPosition;
    }


    [Obsolete]
    public Vector3 GetSelectedMapPositionInComputer()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        ray = sceneCamera.ScreenPointToRay(mousePos);


        ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, placementLayermask))
        {
            // UIButtonScript.Instance.DebuggingText("hitting : " +hit.collider.gameObject.name);
            lastPosition = hit.point;
            hitting = true;
        }
        else
            hitting = false;
        return lastPosition;
    }

}
