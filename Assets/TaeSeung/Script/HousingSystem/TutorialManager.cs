using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class TutorialManager : MonoBehaviour
{
    public DialogController dialog;
    public Transform SessionOrigin;
    public Transform SessionOriginCamera;
    //public List<bool> l_isPlayingHousingTutorial;

    private Transform _dialogPanel;
    private bool isRotateFinish = false;

    void Start()
    {
        SessionOrigin = Camera.main.transform.parent;
        SessionOriginCamera =  Camera.main.transform;
        StartCoroutine(GetDialogpanel());
    }

    private IEnumerator GetDialogpanel()
    {
        int count = 0;
        while (true)
        {
          if(dialog.GetComponentInChildren<RectTransform>() != null)
            {
                _dialogPanel = dialog.GetComponentInChildren<RectTransform>().transform;
                yield break;
            }
            
            if (count == 1500) yield break;
            count++;
            yield return new WaitForSeconds(0.05f);
        }
    }


    public void CameraRotateOrigin()
    {
        Quaternion origin = new();
        SessionOrigin.transform.rotation = origin;
    }

    public IEnumerator CameraRotateToSomeObject(Transform transform, float Rotatetime)
    {
       // SessionOrigin.LookAt(transform);

        Quaternion targetRotation = Quaternion.LookRotation(transform.position - SessionOriginCamera.position);
        SessionOriginCamera.GetComponent<TrackedPoseDriver>().enabled = false;

        while (SessionOriginCamera.rotation != targetRotation)
        {
            targetRotation = Quaternion.LookRotation(transform.position - SessionOriginCamera.position);
            SessionOriginCamera.rotation = Quaternion.Lerp(SessionOriginCamera.rotation, targetRotation, Time.deltaTime * Rotatetime);
            yield return null;
        }

        SessionOriginCamera.GetComponent<TrackedPoseDriver>().enabled = true;
    }

    public IEnumerator CameraRotateToDialog()
    {
        //SessionOrigin.LookAt(_dialogPanel);
        SessionOrigin.transform.position = new Vector3(0, 0, -1f);

        Quaternion targetRotation = Quaternion.LookRotation(_dialogPanel.position - SessionOriginCamera.position);
        SessionOriginCamera.GetComponent<TrackedPoseDriver>().enabled = false;

        while (SessionOriginCamera.rotation != targetRotation)
        {
                targetRotation = Quaternion.LookRotation(_dialogPanel.position - SessionOriginCamera.position);
                SessionOriginCamera.rotation = Quaternion.Lerp(SessionOriginCamera.rotation, targetRotation, Time.deltaTime * 2.5f);
                yield return null;
        }

        SessionOriginCamera.GetComponent<TrackedPoseDriver>().enabled = true;
        SessionOrigin.transform.position = new Vector3(0, 0, 0f);
    }


    public bool IsCompleteRotate() => isRotateFinish;

    


}
