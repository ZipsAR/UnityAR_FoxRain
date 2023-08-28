using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using QCHT.Interactions.Core;
using UnityEngine.XR.ARSubsystems;

public class PlaneDetection : MonoBehaviour
{
    // Start is called before the first frame update
    Ray ray;

    // Update is called once per frame
    void Update()
    {
        RayPlaneDetection();
    }
    

    void RayPlaneDetection()
    {
        Vector3 Cameraforward = Camera.main.transform.forward;
        ray.direction = Cameraforward;
        ray.origin = Camera.main.transform.position;
        RaycastHit[] hits = Physics.RaycastAll(ray, 99999999f);

        DebugTestUI.Instance.HitsDebugText(hits,1);
    }


    public void applicationquit()
    {
        Application.Quit();
    }


}
