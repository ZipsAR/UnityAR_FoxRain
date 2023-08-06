using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logger = ZipsAR.Logger;

public class GiftBox : MonoBehaviour
{
    private GameObject gift;
    [SerializeField] private Transform giftTransform;
    private bool isOpened;
    [SerializeField] private Animator lidAnimator;
    private GameObject curEffect;
    
    [SerializeField] private GameObject idleEffect;
    [SerializeField] private GameObject openEffect;
    [SerializeField] private GameObject afterEffect;
    private static readonly int Open = Animator.StringToHash("Open");

    private void Start()
    {
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
        }    
    }

    public void SetGift(GameObject giftObj)
    {
        gift = Instantiate(giftObj, giftTransform);
        if (gift.TryGetComponent(out Rigidbody component)) component.isKinematic = true;
    }

}
