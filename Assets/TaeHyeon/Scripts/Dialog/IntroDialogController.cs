using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroDialogController : MonoBehaviour
{
    [SerializeField] private Sprite pinchSprite;
    
    private IEnumerator Start()
    {
        yield return null;
        InteractEventManager.NotifyDialogShow("왼손바닥을 바라보면\n여우비의 메뉴창을 불러올 수 있어요.", pinchSprite);
    }
}
