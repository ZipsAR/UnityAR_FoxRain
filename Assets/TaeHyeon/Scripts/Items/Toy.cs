using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Logger = ZipsAR.Logger;

public class Toy : MonoBehaviour
{
    private XRGrabInteractable xrGrabInteractable;
    public float responseTime; // Pet moves toward the Toy after this time
    
    private void Awake()
    {
        xrGrabInteractable = GetComponent<XRGrabInteractable>();
        xrGrabInteractable.selectExited.AddListener(SelectedExited);
    }

    private void SelectedExited(SelectExitEventArgs args)
    {
        Logger.Log("selected Exited");
        GetComponent<Rigidbody>().useGravity = true;

        // Disable Grabbing of this object
        GetComponent<XRGrabInteractable>().enabled = false;

        // Event occurs after a certain period of time as soon as the user places the Grab snack
        StartCoroutine(NotifyToInteractManagerAfterTSeconds(responseTime));
    }

    private IEnumerator NotifyToInteractManagerAfterTSeconds(float waitSeconds)
    {
        yield return new WaitForSeconds(waitSeconds);

        GetComponent<Rigidbody>().isKinematic = true;
        
        Logger.Log("Notify toy Drop to InteractManager");
        GameManager.Instance.interactManager.NotifyToyDrop(transform);
    }
}
