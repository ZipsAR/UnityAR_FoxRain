using QCHT.Interactions.Core;
using QCHT.Interactions.Hands;
using QCHT.Interactions.Proximal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PoseDetection : MonoBehaviour
{
    // Start is called before the first frame update
    public HandPose pose;

    public XRHandTrackingManager trackingmanager;
    private XRHandController controller;
    private Hand hand;
    private HandDriver[] drivers;

    public ZipsarHandAction zipsar;
    public InputAction input;

    private List<FingerData> fingerdatas;
    
    bool asd = true;

    private void Awake()
    {
        zipsar = new();
        input = zipsar.PlayerGesture.Grab;
    }

    void Start()
    {
        trackingmanager = XRHandTrackingManager.InstantiateHandTrackingManager();

        
    
        

        //StartCoroutine(GetHandClass());
    }

    private IEnumerator GetHandClass()                                           
    {
        while (hand == null && drivers == null) {
            try
            {
                controller = trackingmanager.RightHand.GetComponent<XRHandController>();
                hand = controller.model.GetComponent<Hand>();
                drivers = controller.model.GetComponentsInChildren<HandDriver>();
            } 
            catch {}
            
            yield return null;  
        }
        print("end");
    }

    private void Update()
    {


        if (hand != null)
        {
            
         //  hand.UpdateData(pose.Space, ,1)
        }
        if (drivers != null) {
           
        }
        //Recognizepose();

        //print(drivers[0].transform.name);
        
    }

    void Recognizepose()
    {
        int i = 0;
        foreach(FingerData finger in fingerdatas)
        {
            print(i);
            print(finger.MiddleData.Position);
            print(finger.TopData.Position);
            i++;
        }
        i = 0;
    }



    private void OnEnable()
    {
        input.Enable();
        input.performed -= Onon;
        input.canceled -= Offoff;

        input.performed += Onon;
        input.canceled += Offoff;
        
    }

    private void OnDisable()
    {
        input.Disable();
        input.performed -= Onon;
        input.canceled -= Offoff;

    }

    // Update is called once per frame


    private void Onon(InputAction.CallbackContext context)
    {
        DebugTestUI.Instance.DebugText(context.performed,0) ;
    }

    private void Offoff(InputAction.CallbackContext context)
    {
        DebugTestUI.Instance.DebugText(context.performed,0);
    }


}
