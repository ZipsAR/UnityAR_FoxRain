using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogController : MonoBehaviour
{
    [SerializeField] private GameObject dialogCanvas;
    private GameObject curDialog;
    
    private float forward;
    private float up;
    
    private void Awake()
    {
        forward = 2f;
        up = 1f;
        StartCoroutine(Spawn());
    }

    public void OnNextBtnClicked()
    {
        Destroy(curDialog);
    }

    private IEnumerator Spawn()
    {
        while (true)
        {
            curDialog = Instantiate(dialogCanvas, transform);

            var playerTransform = GameManager.Instance.player.transform;
            Vector3 playerForward = playerTransform.forward;
            playerForward = new Vector3(playerForward.x, 0f, playerForward.z).normalized;
            Vector3 playerPos = playerTransform.position;

            Vector3 moveVector = playerForward * forward + Vector3.up * up;

            Vector3 newPosition = playerPos + moveVector;
            curDialog.transform.position = newPosition;
            curDialog.transform.LookAt(playerTransform);
            curDialog.transform.Rotate(0,180f,0);
            curDialog.GetComponent<DialogFrame>().SetText("hello");

            yield return new WaitForSeconds(5f);
        }
    }
}
