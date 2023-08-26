using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class TutorialManager : MonoBehaviour
{
    public DialogController dialog;

    private Transform dialogpanel;
    public Transform Sessionorigin;
    public Transform Sessionorigincamera;
    private bool isRotateFinish = false;

    public List<bool> housingtutorial;


    void Start()
    {
        Sessionorigin = Camera.main.transform.parent;
        Sessionorigincamera =  Camera.main.transform;
        StartCoroutine(GetDialogpanel());
    }

    private IEnumerator GetDialogpanel()
    {
        int count = 0;
        while (true)
        {
          if(dialog.GetComponentInChildren<RectTransform>() != null)
            {
                dialogpanel = dialog.GetComponentInChildren<RectTransform>().transform;
                yield break;
            }
            
            if (count == 1500)
            {
                yield break;
            }

            count++;

            yield return new WaitForSeconds(0.05f);
        }
    }


    public void CameraRotateOrigin()
    {
        Quaternion origin = new();
        Sessionorigin.transform.rotation = origin;
    }

    public IEnumerator CameraRotateToSomeObject(Transform transform, float Rotatetime)
    {
       // Sessionorigin.LookAt(transform);

        Quaternion targetRotation = Quaternion.LookRotation(transform.position - Sessionorigincamera.position);
        Sessionorigincamera.GetComponent<TrackedPoseDriver>().enabled = false;

        while (Sessionorigincamera.rotation != targetRotation)
        {
            targetRotation = Quaternion.LookRotation(transform.position - Sessionorigincamera.position);
            Sessionorigincamera.rotation = Quaternion.Lerp(Sessionorigincamera.rotation, targetRotation, Time.deltaTime * Rotatetime);
            yield return null;
        }

        Sessionorigincamera.GetComponent<TrackedPoseDriver>().enabled = true;
    }

    public IEnumerator CameraRotateToDialog()
    {
        //Sessionorigin.LookAt(dialogpanel);
        Sessionorigin.transform.position = new Vector3(0, 0, -1f);

        Quaternion targetRotation = Quaternion.LookRotation(dialogpanel.position - Sessionorigincamera.position);
        Sessionorigincamera.GetComponent<TrackedPoseDriver>().enabled = false;

        while (Sessionorigincamera.rotation != targetRotation)
        {
                targetRotation = Quaternion.LookRotation(dialogpanel.position - Sessionorigincamera.position);
                Sessionorigincamera.rotation = Quaternion.Lerp(Sessionorigincamera.rotation, targetRotation, Time.deltaTime * 2.5f);
                yield return null;
        }

        Sessionorigincamera.GetComponent<TrackedPoseDriver>().enabled = true;
        Sessionorigin.transform.position = new Vector3(0, 0, 0f);
    }


    public bool IsCompleteRotate() => isRotateFinish;

    


}
