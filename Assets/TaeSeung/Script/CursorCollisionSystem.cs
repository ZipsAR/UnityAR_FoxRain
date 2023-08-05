using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 커서와 충돌된 오브젝트에 대한 정보를 파악해주거나 그 오브젝트의 무언가를 바꿔주는 클래스 
/// </summary>
public class CursorCollisionSystem : MonoBehaviour
{
    // Start is called before the first frame update

    GameObject Parent;
    MeshRenderer thismaterial;
    Color Initializecolor;
    bool iscollision;


    private void Start()
    {
        Parent = this.transform.parent.gameObject;
        thismaterial = this.GetComponent<MeshRenderer>();
        Initializecolor = thismaterial.material.color;
        
    }

    private void Update()
    {
        int mask = LayerMask.GetMask("ObjectCursorCollision");
        //print(mask);
    }

    private void OnTriggerStay(Collider other)
    {
        int mask = 7;

        if (PlacementSystem.Instance.CatchObject != null)
        { 
            if (other.gameObject.layer == mask && PlacementSystem.Instance.CatchObject.transform.GetInstanceID() != other.gameObject.transform.parent.GetInstanceID())
            {
                iscollision = true;

                Parent.GetComponent<MeshRenderer>().material.color = Color.red;
                thismaterial.material.color = Color.red;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        iscollision = false;
        thismaterial.material.color = Initializecolor;
    }


    public bool Iscollision() => iscollision;



}
