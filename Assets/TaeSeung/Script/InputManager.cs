using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


// 커서 위치에 따른 world 좌표계에서의 맵 위치를 계산해주는 클래스 + 마우스 인풋 이벤트를 처리해주는 클래스

public class InputManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Camera sceneCamera;
    private Vector3 lastPosition;

    [SerializeField]
    private LayerMask placementLayermask;

    public event Action OnClicked, OnExit;

    public bool hitting;

    Ray ray;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            OnClicked?.Invoke();
        if (Input.GetKeyDown(KeyCode.Escape))
            OnExit?.Invoke();
    }

    public bool IsPointerOverUI()
        => EventSystem.current.IsPointerOverGameObject();


    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, placementLayermask))
        {
            lastPosition = hit.point;
            hitting = true;
        }
        else
            hitting = false;


        return lastPosition;
    }

    public bool ishit() => hitting;

}
