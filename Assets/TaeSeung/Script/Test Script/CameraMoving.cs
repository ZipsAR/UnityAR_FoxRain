using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoving : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 pos;
    Vector3 rot;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        pos = this.transform.position;
        rot = this.transform.rotation.eulerAngles;

        if (Input.GetKey(KeyCode.W)) {
            relocation(true,0.01f);
        }
        if (Input.GetKey(KeyCode.S))
        {
            relocation(true,-0.01f);
        }
        if (Input.GetKey(KeyCode.A))
        {
            relocation(false, 0.01f);
        }
        if (Input.GetKey(KeyCode.D))
        {
            relocation(false, -0.01f);
        }        

    }


    void relocation(bool xz , float dif)
    {
        if (xz)
        {
            pos.z = pos.z + dif;
            this.transform.position = pos;
        }
        else
        {
            pos.x = pos.x - dif;
            this.transform.position = pos;
        }
    }

}
