using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PlaneDetection : MonoBehaviour
{
    // Start is called before the first frame update
    Ray ray;
    public TMP_Text Debugtext;
    public ZipsarHandAction zipsar;
    public InputAction input;

    void Awake()
    {
        zipsar = new();

        input = zipsar.PlayerGesture.DevicePositionTracking;

    }

    private void OnEnable()
    {
        input.Enable();
        input.performed += Onon;
    }

    private void OnDisable()
    {
        input.Disable();
        input.performed -= Onon;
 
    }

    // Update is called once per frame
    void Update()
    {
        Debugtext.text = "";
        Vector3 Cameraforward = Camera.main.transform.forward;

        ray.direction = Cameraforward;
        ray.origin = Camera.main.transform.position;

        RaycastHit[] hits = Physics.RaycastAll(ray, 99999999f);
        for(int i=0; i<hits.Length; i++)
        {
            Debugtext.text += hits[i].transform.name + " : " + hits[i].point;
        }

       //print("update: "+ input.ReadValueAsObject());

    }
    
    private void Onon(InputAction.CallbackContext context)
    {
        print(context.ReadValueAsObject());
    }


    public void applicationquit()
    {
        Application.Quit();
    }


}
