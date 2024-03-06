using UnityEngine;

/// <summary>
/// Ŀ���� �浹�� ������Ʈ�� ���� ������ �ľ����ְų� �� ������Ʈ�� ���𰡸� �ٲ��ִ� Ŭ���� 
/// </summary>
public class CursorCollisionSystem : MonoBehaviour
{
    private MeshRenderer[] _childMaterials;
    private MeshRenderer _thisMaterial;

    private bool _isCollision = true;

    private void Start()
    {
        _thisMaterial = this.GetComponent<MeshRenderer>();
        _childMaterials = this.GetComponentsInChildren<MeshRenderer>();

    }

    private void OnTriggerStay(Collider other)
    {
        //ObjectCursorcollision
        int mask = 7;

        if (PlacementSystem.Instance.CatchObj != null)
        { 
            if (other.gameObject.layer == mask && PlacementSystem.Instance.CatchObj.transform.GetInstanceID() != other.gameObject.transform.parent.GetInstanceID())
            {
                _isCollision = false;
                _thisMaterial.material.SetColor("_Color", new Vector4(1,0,0,0.5f));
                _childMaterials[1].material.SetColor("_Color", new Vector4(1,0,0,0.5f));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _isCollision = true;
        _thisMaterial.material.SetColor("_Color", new Vector4(1,1,1,1));
        _childMaterials[1].material.SetColor("_Color", new Vector4(0, 1, 0, 0.5f));

    }


    public void ColorCursorsetting(Vector4 Color1, Vector4 Color2)
    {
        _thisMaterial.material.SetColor("_Color", Color1);
        _childMaterials[1].material.SetColor("_Color", Color2);
    }

    public bool Iscollision() => _isCollision;
}
