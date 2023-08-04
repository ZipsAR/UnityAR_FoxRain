using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 커서와 충돌된 오브젝트에 대한 정보를 파악해주거나 그 오브젝트의 무언가를 바꿔주는 클래스 
/// </summary>
public class CursorCollisionSystem : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    Material transparentmaterial;

    GameObject obj;

    Renderer[] renders;


    // Update is called once per frame
    void Update()
    {
        int layermask = 1 << LayerMask.NameToLayer("ObjectCursorCollision");
        SphereCollider sphere  = this.GetComponent<SphereCollider>();
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, sphere.radius,layermask);

        //충돌 판정이 된 경우, 충돌된 물체에 대한 정보를 담는다.
        foreach (Collider collider in colliders)
        {
            obj = collider.gameObject.transform.parent.gameObject;
            renders = obj.GetComponentsInChildren<Renderer>(true);

            //이건 충돌된 물체에 대한 머터리얼 바꾸기를 해보려는건데... 어렵네? ㅎ
            //print("render: "+renders.Length);
            foreach (Renderer temprender in renders)
            {
                Material[] tempmaterials = temprender.materials;
                for (int i = 0; i < tempmaterials.Length; i++)
                {
                    tempmaterials[i] = transparentmaterial;
                }
            }
            break;
        }

        //아무것도 안 닿아져 있을 경우, obj를 null로 판정
        if (colliders.Length == 0)
            obj = null;

    }




    /// <summary>
    /// 충돌된 오브젝트에 대한 정보를 보내줌
    /// </summary>
    /// <returns></returns>
    public GameObject GetCollisionobject() => obj;

}
