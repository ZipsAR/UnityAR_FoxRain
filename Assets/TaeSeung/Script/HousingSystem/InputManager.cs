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
    [SerializeField]
    private LayerMask placementLayermask;

    public GameObject PlaneObj;
    private Vector3 _lastPos;
    private Ray ray;
    private bool _isHit;


    public bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();
    public bool IsHit() => _isHit;

    /// <summary>
    /// object 위치를 기준으로 그리드 포지션을 가져와주는 함수
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    public Vector3 GetSelectedMapPositionbyObject(Transform transform)
    {
        Vector3 Planepos = transform.position;
        Planepos.y = PlaneObj.transform.position.y - 1;

        ray.origin = Planepos;
        //ray.origin = transform.position;
        //ray.direction = PlaneObj.transform.up * -1;
        ray.direction = PlaneObj.transform.up;

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, placementLayermask))
        {
            // UIButtonScript.Instance.DebuggingText("_isHit : " +hit.collider.gameObject.name);
            _lastPos = hit.point;
            _isHit = true;
        }
        else
            _isHit = false;

        return _lastPos;
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
            // UIButtonScript.Instance.DebuggingText("_isHit : " +hit.collider.gameObject.name);
            print(hit.transform.name);
            _lastPos = hit.point;
            _isHit = true;
        }
        else
            _isHit = false;

        return _lastPos;
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
                HousingUISystem.Instance.DebuggingText("_isHit : " + hit[i].collider.gameObject.name);
                _lastPos = hit[i].point;
                _isHit = true;
            }
        }
        else
            _isHit = false;
        return hit;
    }
}
