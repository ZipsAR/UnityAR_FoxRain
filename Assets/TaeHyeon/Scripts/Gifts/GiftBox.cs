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

    // Effects
    private GameObject curEffect;
    [SerializeField] private GameObject idleEffect;
    [SerializeField] private GameObject openEffect;
    [SerializeField] private GameObject afterEffect;
    [SerializeField] private GameObject giftEffect;
    
    // Sound
    [SerializeField] private AudioClip spawningBoxClip;
    [SerializeField] private AudioClip openingBoxClip;
    [SerializeField] private AudioClip earnedClip;

    private static readonly int Open = Animator.StringToHash("Open");
    
    private float risingTime;
    private int coinEarnedValue;
    
    private void Start()
    {
        risingTime = 2f;

        curEffect = Instantiate(idleEffect, transform);
        lid = lidObj.GetComponent<Lid>();
        lid.SetCloseAfterSecond(risingTime);

        GameManager.Instance.interactAudioManager.PlayEffectSound(spawningBoxClip);
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

            GameObject parentOfGift = new GameObject();
            parentOfGift.transform.position = giftTransform.position;
            gift.transform.SetParent(parentOfGift.transform);

            GiftMovement giftMovement = parentOfGift.AddComponent<GiftMovement>();
            giftMovement.gameObject.name = "parentOfGift";
            giftMovement.SetGift(gift);
            giftMovement.StartRotating();
            giftMovement.StartRising(risingTime);
            giftMovement.SetCoinEarnedValue(coinEarnedValue);
            giftMovement.SetSound(earnedClip);
                
            Instantiate(giftEffect, parentOfGift.transform);

            // Sound
            GameManager.Instance.interactAudioManager.PlayEffectSound(openingBoxClip);
        }    
    }

    public void SpawnGift(GameObject giftObj)
    {
        gift = Instantiate(giftObj);
        gift.transform.position = giftTransform.position;
        if (gift.TryGetComponent(out Rigidbody component)) component.isKinematic = true;
    }
    

    public void Vanish()
    {
        Logger.Log("vanish call");
        StartCoroutine(VanishAfterSec(1f));
    }

    private IEnumerator VanishAfterSec(float sec)
    {
        float timer = 0;

        while (true)
        {
            timer += Time.deltaTime;
            if (timer > sec)
            {
                Logger.Log("destroy gift box");
                
                Destroy(gameObject);
            }

            yield return null;
        }
    }
    
    public void SetCoinEarnedValue(int value) => coinEarnedValue = value;

}
