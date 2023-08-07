using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftMovement : MonoBehaviour
{
    private bool isTopPosition;

    private GameObject gift;
    
    private float rotationSpeed; // Rotation angle per second
    private float risingDistance;
    
    private void Awake()
    {
        isTopPosition = false;

        rotationSpeed = 180f;
        risingDistance = 0.08f;
    }
    
    public void setGift(GameObject g) => gift = g;

    private IEnumerator RotationCoroutine()
    {
        while (true)
        {
            gift.transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
            yield return null;
        }
    }

    private IEnumerator RisingCoroutine(float risingTime)
    {
        float elapsedTime = 0f;
        Vector3 initialPosition = transform.position;
        Vector3 targetPosition = initialPosition + Vector3.up * risingDistance;
        
        while (elapsedTime < risingTime)
        {
            float t = elapsedTime / risingTime;
            transform.position = Vector3.Lerp(initialPosition, targetPosition, t);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        isTopPosition = true;
    }

    public void StartRotating()
    {
        StartCoroutine(RotationCoroutine());
    }

    public void StartRising(float f)
    {
        StartCoroutine(RisingCoroutine(f));
    }
}
