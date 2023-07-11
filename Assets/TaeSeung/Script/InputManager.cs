using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Camera sceneCamera;

    private Vector3 lastPosition;

    [SerializeField]
    private LayerMask placementLayermask;

    public event Action OnClicked, OnExit;


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
        Vector3 moustPos = Input.mousePosition;
        moustPos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(moustPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, placementLayermask)) { 
            lastPosition = hit.point;
         }

        return lastPosition;
    }
}
