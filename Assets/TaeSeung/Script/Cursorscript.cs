using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursorscript : MonoBehaviour
{
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        int layermask = 1 << LayerMask.NameToLayer("PlaceObject");
        
        Collider[] colliders = Physics.OverlapCapsule(new Vector3(this.transform.position.x, this.transform.position.y-20, this.transform.position.z), new Vector3(this.transform.position.x, this.transform.position.y + 20, this.transform.position.z), 0.5f,layermask);
        GameObject obj;

        foreach (Collider collider in colliders)
        {
            obj = collider.gameObject;
            Debug.Log("overlap : " + obj.name);
            break;
        }


        if (Input.GetMouseButtonDown(0))
        {
            
        }
            
    }

}
