using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroDialogController : MonoBehaviour
{
    [SerializeField] private Sprite pinchSprite;

    private bool showAllDialog;

    private void Awake()
    {
        showAllDialog = false;
    }

    private void OnEnable()
    {
        InteractEventManager.OnClickedDialogExitBtn -= OnClickedDialogExitBtn;
        InteractEventManager.OnClickedDialogExitBtn += OnClickedDialogExitBtn;
    }

    private void OnDisable()
    {
        InteractEventManager.OnClickedDialogExitBtn -= OnClickedDialogExitBtn;
    }

    private void OnClickedDialogExitBtn(object sender, EventArgs e)
    {
        if (!showAllDialog)
        {
            InteractEventManager.NotifyDialogShow("왼손바닥을 바라보면\n여우비의 메뉴창을 불러올 수 있어요");
            showAllDialog = true;
        }
    }

    private IEnumerator Start()
    {
        yield return null;
        InteractEventManager.NotifyDialogShow("여우비에 오신것을 환영합니다!\n오른쪽 버튼을 핀치해서\n대화창을 끌 수 있어요", pinchSprite);
    }
}
