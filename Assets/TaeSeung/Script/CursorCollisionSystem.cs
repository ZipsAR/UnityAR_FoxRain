using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ŀ���� �浹�� ������Ʈ�� ���� ������ �ľ����ְų� �� ������Ʈ�� ���𰡸� �ٲ��ִ� Ŭ���� 
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

        //�浹 ������ �� ���, �浹�� ��ü�� ���� ������ ��´�.
        foreach (Collider collider in colliders)
        {
            obj = collider.gameObject.transform.parent.gameObject;
            renders = obj.GetComponentsInChildren<Renderer>(true);

            //�̰� �浹�� ��ü�� ���� ���͸��� �ٲٱ⸦ �غ����°ǵ�... ��Ƴ�? ��
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

        //�ƹ��͵� �� ����� ���� ���, obj�� null�� ����
        if (colliders.Length == 0)
            obj = null;

    }




    /// <summary>
    /// �浹�� ������Ʈ�� ���� ������ ������
    /// </summary>
    /// <returns></returns>
    public GameObject GetCollisionobject() => obj;

}
