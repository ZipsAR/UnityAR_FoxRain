using System;
using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using UnityEngine;

public class DialogController : MonoBehaviour
{
    [SerializeField] private GameObject dialogCanvas;
    private List<Tuple<int, GameObject>> curDialogList;
    
    private float forward;
    private float up;
    private float side;
    
    private int id;
    
    private void Awake()
    {
        forward = 3f;
        up = 1f;
        side = 3f;
        
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
        Vector3 playerPos = playerTransform.position;
        
        Vector3 playerForward = playerTransform.forward;
        playerForward = new Vector3(playerForward.x, 0f, playerForward.z).normalized;

        Vector3 playerRight = playerTransform.right;
        playerRight = new Vector3(playerRight.x, 0f, playerRight.z).normalized;

        Vector3 moveVector = e.dialogOrient switch
        {
            DialogOrient.Center => playerForward * forward + Vector3.up * up,
            DialogOrient.Left => -playerRight * side + Vector3.up * up,
            DialogOrient.Right => playerRight * side + Vector3.up * up,
            _ => throw new ArgumentOutOfRangeException()
        };

        Vector3 newPosition = playerPos + moveVector;
        
        newDialog.transform.position = newPosition;
        newDialog.transform.LookAt(playerTransform);
        newDialog.transform.Rotate(0,180f,0);
        newDialog.GetComponent<DialogFrame>().InitDialog(id, e.content);
        
        newDialog.GetComponent<DialogFrame>().SetImg(e.infoSprite);
        
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
