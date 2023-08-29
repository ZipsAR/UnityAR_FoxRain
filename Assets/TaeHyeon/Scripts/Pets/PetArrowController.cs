using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Logger = ZipsAR.Logger;

public class PetArrowController : MonoBehaviour
{
    private bool isPetInitialized;
    private float angleDegrees;
    private Camera mainCamera;
    [SerializeField] private Canvas uiCanvas;

    // Pet
    private Transform petTransform;
    private Vector3 petScreenPos;

    // Arrow
    [SerializeField] private GameObject arrowPrefab;
    private GameObject arrowInstance;
    private RectTransform arrowRectTransform;
    private float circleBoundaryRadius;

    // Screen
    private int screenWidthHalf;
    private int screenHeightHalf;
    
    private void OnEnable()
    {
        InteractEventManager.OnPetInitializedToAll -= OnPetInitializedToAll;
        InteractEventManager.OnPetInitializedToAll += OnPetInitializedToAll;
    }

    private void OnDisable()
    {
        InteractEventManager.OnPetInitializedToAll -= OnPetInitializedToAll;
    }

    private void OnPetInitializedToAll(object sender, PetArgs e)
    {
        mainCamera = Camera.main;
        
        screenWidthHalf = Screen.width / 2;
        screenHeightHalf = Screen.height / 2;

        petTransform = e.petObj.transform;
        circleBoundaryRadius = screenWidthHalf - 600f;
        isPetInitialized = true;
    }

    private void Update()
    {
        if(!isPetInitialized) return;

        petScreenPos = GetPetScreenPos();

        if (IsPetInView())
        {
            if (arrowInstance != null)
            {
                Destroy(arrowInstance);
                arrowInstance = null;
            }
            return;
        }
        
        if (arrowInstance == null)
        {
            arrowInstance = Instantiate(arrowPrefab, uiCanvas.transform);
            arrowRectTransform = arrowInstance.GetComponent<RectTransform>();
        }

        // Set arrow Rotation
        Vector2 cameraToPet = new Vector2(petScreenPos.x, petScreenPos.y).normalized;
        angleDegrees = Mathf.Atan2(cameraToPet.y, cameraToPet.x) * Mathf.Rad2Deg;
        arrowRectTransform.localRotation = Quaternion.Euler(0,0,angleDegrees);

        // Set arrow Position
        arrowRectTransform.localPosition = cameraToPet * circleBoundaryRadius;
        Vector3 arrowposition = arrowRectTransform.localPosition;

        if ((screenHeightHalf - 75f) < Mathf.Abs(arrowRectTransform.localPosition.y))
        {
            arrowposition.y = (arrowRectTransform.localPosition.y/Mathf.Abs(arrowRectTransform.localPosition.y))*(screenHeightHalf - 75f);
            arrowRectTransform.localPosition = arrowposition;
        }
    }

    private bool IsPetInView()
    {
        if (petScreenPos.x < -screenWidthHalf || petScreenPos.x > screenWidthHalf ||
            petScreenPos.y < -screenHeightHalf || petScreenPos.y > screenHeightHalf ||
            petScreenPos.z < 0)
        {
            return false;
        }

        return true;
    }

    private Vector3 GetPetScreenPos()
    {
        Vector3 petScreenPosLeftBottomAnchor = mainCamera.WorldToScreenPoint(petTransform.position);
        Vector3 result;
        result = new Vector3(
            petScreenPosLeftBottomAnchor.x - screenWidthHalf, 
            petScreenPosLeftBottomAnchor.y - screenHeightHalf, 
            petScreenPosLeftBottomAnchor.z);

        if (result.z < 0)
        {
            result.x *= -1;
            result.y *= -1;
        }
        
        return result;
        
    }
}
