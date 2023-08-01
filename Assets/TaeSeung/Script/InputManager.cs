using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;

// 커서 위치에 따른 world 좌표계에서의 맵 위치를 계산해주는 클래스 + 마우스 인풋 이벤트를 처리해주는 클래스

public class InputManager: Singleton<InputManager>
{
    // Start is called before the first frame update
    [SerializeField]
    private Camera sceneCamera;
    private Vector3 lastPosition;

    [SerializeField]
    private LayerMask placementLayermask;
    private bool hitting;
    public GameObject Plane;
    //public GameObject Wall;



    private Ray ray;


    public event Action OnClicked, OnExit;

    private void Start()
    {

    }

    private void Update()
    {
        /*
        if (Input.GetMouseButtonDown(0))
            OnClicked?.Invoke();
        if (Input.GetKeyDown(KeyCode.Escape))
            OnExit?.Invoke();
        */
        //GetSelectedMapPosition();

    }

    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();

    
    /// <summary>
    /// object 위치를 기준으로 그리드 포지션을 가져와주는 함수
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


    /// <summary>
    /// object 위치를 기준으로 Ray를 쐈을때 벽의 포지션을 가져와주는 함수
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    public Vector3 GetSelectedMapPositionbyObjectForward(Transform transform)
    {
        ray.origin = transform.position;
        ray.direction = transform.forward * -1;

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, placementLayermask))
        {
            // UIButtonScript.Instance.DebuggingText("hitting : " +hit.collider.gameObject.name);
            print(hit.transform.name);
            lastPosition = hit.point;
            hitting = true;
        }
        else
            hitting = false;

        return lastPosition;
    }


    public RaycastHit[] GetSelectedMapPositionbyVision()
    {
        ray.origin = sceneCamera.transform.position;
        ray.direction = sceneCamera.transform.forward;

        RaycastHit[] hit;


        if ((hit = Physics.RaycastAll(ray, 10000)).Length > 0)
        {
            for (int i = 0; i < hit.Length; i++)
            {
                UIInitialize.Instance.DebuggingText("hitting : " + hit[i].collider.gameObject.name);
                lastPosition = hit[i].point;
                hitting = true;
            }
        }
        else
            hitting = false;


        return hit;
    }



    public bool ishit() => hitting;




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
