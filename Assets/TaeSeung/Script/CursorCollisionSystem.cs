using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ŀ���� �浹�� ������Ʈ�� ���� ������ �ľ����ְų� �� ������Ʈ�� ���𰡸� �ٲ��ִ� Ŭ���� 
/// </summary>
public class CursorCollisionSystem : MonoBehaviour
{
    // Start is called before the first frame update

    private MeshRenderer[] childmaterial;
    private MeshRenderer thismaterial;

    Color childInitializecolor;
    Color thisInitializecolor;

    bool iscollision;


    private void Start()
    {
        thismaterial = this.GetComponent<MeshRenderer>();
        childmaterial = this.GetComponentsInChildren<MeshRenderer>();

    }

    private void Update()
    {
        print(iscollision);
    }

    private void OnTriggerStay(Collider other)
    {
        int mask = 7;

        if (PlacementSystem.Instance.CatchObject != null)
        { 
            if (other.gameObject.layer == mask && PlacementSystem.Instance.CatchObject.transform.GetInstanceID() != other.gameObject.transform.parent.GetInstanceID())
            {
                iscollision = false;
                thismaterial.material.SetColor("_Color", new Vector4(1,0,0,0.5f));
                childmaterial[1].material.SetColor("_Color", new Vector4(1,0,0,0.5f));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        iscollision = true;
        thismaterial.material.SetColor("_Color", new Vector4(1,1,1,1));
        childmaterial[1].material.SetColor("_Color", new Vector4(0, 1, 0, 0.5f));

    }


    public bool Iscollision() => iscollision;



}
