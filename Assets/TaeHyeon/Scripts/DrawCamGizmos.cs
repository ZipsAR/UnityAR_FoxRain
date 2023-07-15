using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCamGizmos : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        
        Gizmos.DrawLine(transform.position, transform.forward * 10f + transform.position);
        
    }
}
