using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Logger = ZipsAR.Logger;

public class GiftBox : MonoBehaviour
{
    private bool isOpened;
    private GameObject gift;
    [SerializeField] private GameObject lidObj;
    private Lid lid;
    [SerializeField] private Transform giftTransform;
    // [SerializeField] private Animator lidAnimator;

    // Effects
    private GameObject curEffect;
    [SerializeField] private GameObject idleEffect;
    [SerializeField] private GameObject openEffect;
    [SerializeField] private GameObject afterEffect;
    [SerializeField] private GameObject giftEffect;
    [SerializeField] private Transform giftEffectPos;
    
    private static readonly int Open = Animator.StringToHash("Open");

    private float roatationTime;
    private float rotationSpeed; // Rotation angle per second
    private float risingTime;
    private float risingDistance;

    private void Start()
    {
        roatationTime = 10f;
        rotationSpeed = 270f;
        risingTime = 2f;
        risingDistance = 0.08f;

        curEffect = Instantiate(idleEffect, transform);
        lid = lidObj.GetComponent<Lid>();
        lid.SetCloseAfterSecond(risingTime);

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
            lid.GetComponent<Animator>().SetTrigger(Open);
            GetComponent<Collider>().enabled = false;
            
            // Effect
            Instantiate(giftEffect, giftEffectPos);
            
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

    public void Vanish()
    {
        Logger.Log("vanish call");
        StartCoroutine(VanishAfterSec(2f));
    }

    private IEnumerator VanishAfterSec(float sec)
    {
        float timer = 0;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > sec)
            {
                Logger.Log("destroy");
                
                Destroy(gift);
                Destroy(gameObject);
            }

            yield return null;
        }
    }
}
