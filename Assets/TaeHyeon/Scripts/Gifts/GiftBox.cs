using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = ZipsAR.Logger;

public class GiftBox : MonoBehaviour
{
    private bool isOpened;
    private GameObject gift;
    [SerializeField] private Transform giftTransform;
    [SerializeField] private Animator lidAnimator;

    // Effects
    private GameObject curEffect;
    [SerializeField] private GameObject idleEffect;
    [SerializeField] private GameObject openEffect;
    [SerializeField] private GameObject afterEffect;
    
    private static readonly int Open = Animator.StringToHash("Open");

    private float roatationTime;
    private float rotationSpeed; // Rotation angle per second
    private float risingTime;
    private float risingDistance;

    private void Start()
    {
        roatationTime = 4f;
        rotationSpeed = 180f;
        risingTime = 2f;
        risingDistance = 0.05f;

        curEffect = Instantiate(idleEffect, transform);
    }

    private void OnCollisionEnter(Collision other)
    {
        if(isOpened) return;
        
        if (other.gameObject.GetComponent<HandController>() != null)
        {
            Logger.Log("hand touch gift box");
            isOpened = true;
            
            Destroy(curEffect);
            curEffect = Instantiate(openEffect, transform);
            lidAnimator.SetTrigger(Open);
            GetComponent<Collider>().enabled = false;
            
            StartCoroutine(StartRotation());
            StartCoroutine(StartRising());
        }    
    }

    public void SpawnGift(GameObject giftObj)
    {
        gift = Instantiate(giftObj);
        gift.transform.position = giftTransform.position;
        if (gift.TryGetComponent(out Rigidbody component)) component.isKinematic = true;
    }

    private IEnumerator StartRotation()
    {
        while (roatationTime > 0)
        {
            gift.transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
            roatationTime -= Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator StartRising()
    {
        float elapsedTime = 0f;
        Vector3 initialPosition = gift.transform.position;
        Vector3 targetPosition = initialPosition + Vector3.up * risingDistance;
        
        while (elapsedTime < risingTime)
        {
            float t = elapsedTime / risingTime;
            gift.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);

            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }
}
