using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogController : MonoBehaviour
{
    [SerializeField] private GameObject dialogCanvas;
    private List<Tuple<int, GameObject>> curDialogList;
    
    private float forward;
    private float up;

    private int id;
    
    private void Awake()
    {
        forward = 2f;
        up = 1f;
        id = 0;
        curDialogList = new List<Tuple<int, GameObject>>();
        
        InteractEventManager.OnDialogCall -= OnDialogCall;
        InteractEventManager.OnDialogCall += OnDialogCall;
        InteractEventManager.OnClearDialog -= OnClearDialog;
        InteractEventManager.OnClearDialog += OnClearDialog;
    }

    private void OnDisable()
    {
        InteractEventManager.OnDialogCall -= OnDialogCall;
        InteractEventManager.OnClearDialog -= OnClearDialog;
    }

    private void OnClearDialog(object sender, EventArgs e)
    {
        foreach (Tuple<int,GameObject> tuple in curDialogList)
        {
            if (tuple.Item2 != null)
            {
                Destroy(tuple.Item2);
            }
        }
        curDialogList.Clear();
    }
    
    private void OnDialogCall(object sender, DialogArgs e)
    {
        GameObject newDialog = Instantiate(dialogCanvas, transform);

        var playerTransform = GameManager.Instance.player.transform;
        Vector3 playerForward = playerTransform.forward;
        playerForward = new Vector3(playerForward.x, 0f, playerForward.z).normalized;
        Vector3 playerPos = playerTransform.position;
        Vector3 moveVector = playerForward * forward + Vector3.up * up;

        Vector3 newPosition = playerPos + moveVector;
        
        newDialog.transform.position = newPosition;
        newDialog.transform.LookAt(playerTransform);
        newDialog.transform.Rotate(0,180f,0);
        newDialog.GetComponent<DialogFrame>().InitDialog(id, e.content);
        
        curDialogList.Add(new Tuple<int, GameObject>(id, newDialog));
        
        id++;
    }

    public void OnNextBtnClicked(int inputId)
    {
        Tuple<int, GameObject> foundTuple = curDialogList.Find(tuple => tuple.Item1 == inputId);

        if (foundTuple != null)
        {
            curDialogList.Remove(foundTuple);
            Destroy(foundTuple.Item2);
        }
    }
}
