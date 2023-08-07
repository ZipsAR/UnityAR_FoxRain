using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Logger = ZipsAR.Logger;

public class Lid : MonoBehaviour
{
    public GiftBox giftBox;
    
    private Animator animator;
    private float closeAfterSecond;
    private static readonly int Stay = Animator.StringToHash("Stay");
    private static readonly int Close = Animator.StringToHash("Close");

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetCloseAfterSecond(float sec) => closeAfterSecond = sec;
    
    public void FullyOpened()
    {
        animator.SetTrigger(Stay);
        StartCoroutine(CloseBox());
    }

    private IEnumerator CloseBox()
    {
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;

            if (timer > closeAfterSecond)
            {
                animator.SetTrigger(Close);
                break;
            }

            yield return null;
        }
    }

    public void FullyClosed()
    {
        Logger.Log("FullyClosed");
        giftBox.Vanish();
    }
}
