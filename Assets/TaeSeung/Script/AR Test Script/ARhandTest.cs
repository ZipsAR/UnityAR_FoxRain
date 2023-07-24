using QCHT.Interactions.Core;
using QCHT.Interactions.Hands;
using UnityEngine;
using filter = UnityEngine.XR.Interaction.Toolkit.Filtering;

public class ARhandTest : MonoBehaviour
{
    [SerializeField]
    private GameObject ARSessionorigin;
    private Transform Trackables;
    XRHandController[] handcontrollers;

    [SerializeField]
    XRHandTrackingManager hand;
    

    // Start is called before the first frame update
    void Start()
    {
        Trackables = ARSessionorigin.transform.Find("Trackables");
        
        //leftaction = handcontrollers[0];
        //rightaction = handcontrollers[1];

    }

    // Update is called once per frame
    void Update()
    {        
        if (handcontrollers == null)
            handcontrollers = Trackables.GetComponentsInChildren<XRHandController>();

        string debugmsg = "";
        foreach(XRHandController handcontroller in handcontrollers)
        {
            debugmsg += handcontroller.Handedness + ": " + handcontroller.currentControllerState.selectInteractionState.active + "\n";
        }

        UIButtonScript.Instance.DebuggingText(debugmsg);

        //print("controllstate: "+rightaction.currentControllerState);
        //GestureType.PINCH;

    }

    public void click()
    {
        print("click ÇØÂÇ¿è");
    }
   

}
