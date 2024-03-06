using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MapInfo : Singleton<MapInfo>
{
    [SerializeField]
    private GameObject _tileMapObj, _tileObj, _cursorObj, _planeObj;
    [SerializeField]
    private Material _gridGraphMaterial;

    // Start is called before the first frame update
    public List<bool> l_IsHousingTutorialFinish;
    public Vector2Int Mapsize;
    public float MapScale;
    public bool IsInitialize;

    private Vector3 _tileMapScale, _tileScale, _cursorScale, _tileEffectScale;
    private float _firstScale;

    private void Start()
    {
        if(IsInitialize) MapInitialize();
        _tileEffectScale = _tileObj.transform.GetChild(0).localScale;
        SetOrigin();
    }

    public void SetOrigin()
    {
        this.transform.position = new Vector3(0, -0.6f, 1);
        this.transform.eulerAngles = new Vector3(0, 0, 0);
    }

    public void SetAnotherPosition(Vector3 newposition) => this.transform.position = newposition;
    


    public void MapInitialize()
    {
        float reverseMapScale = 1 / MapScale;
        _firstScale = MapScale;
        if (_tileMapObj)
        {
            _tileMapScale = _tileMapObj.transform.localScale;
            print(_tileMapScale);
            _tileMapObj.transform.localScale = _tileMapScale * reverseMapScale;
        }
        if (_cursorObj)
        {
            _cursorScale = _cursorObj.transform.localScale;
            _cursorObj.transform.localScale = _cursorScale * reverseMapScale;
        }
        if (_gridGraphMaterial)
        {
            _gridGraphMaterial.SetVector("_Size", new Vector2(1, 1));
            _tileScale = _gridGraphMaterial.GetVector("_Size");
            _gridGraphMaterial.SetVector("_Size", _tileScale * MapScale);
        }
    }

    
    public void SetTileScale(Vector3 scale)
    {
        _tileObj.transform.localScale = scale;
        Vector3 effectscale = _tileEffectScale;
        effectscale.x = effectscale.x / scale.x;
        effectscale.y = effectscale.y / scale.y;
        effectscale.z = effectscale.z / scale.z;
        _tileObj.transform.GetChild(0).localScale = effectscale;
    }




    public void SetReScale(float rescale){

        MapScale = rescale;
        float reverseMapScale = 1 / MapScale;

        if (_tileMapObj)
            _tileMapObj.transform.localScale = _tileMapScale * reverseMapScale;

        if (_cursorObj)
            _cursorObj.transform.localScale = _cursorScale * reverseMapScale;

        if (_gridGraphMaterial)
            _gridGraphMaterial.SetVector("_Size", _tileScale * MapScale);
    }

    public void ResetTileScale() => _tileObj.transform.GetChild(0).localScale = _tileEffectScale;
    

    public void SetMapHousingmode()
    {
        PlacementSystem.Instance.ReleaseGrib();
        if(HousingUISystem.Instance!=null) HousingUISystem.Instance.transform.gameObject.SetActive(true);
        if (EffectSystem.Instance != null) EffectSystem.Instance.gameObject.SetActive(true);
        if(SoundSystem.Instance != null) SoundSystem.Instance.gameObject.SetActive(true);
        this.gameObject.SetActive(true);
    }

    public void SetMapNormalmode()
    {
       PlacementSystem.Instance.ProtectGrib();
        _tileObj.SetActive(false);
        if (HousingUISystem.Instance != null)  HousingUISystem.Instance.transform.gameObject.SetActive(false);
       if (EffectSystem.Instance != null) EffectSystem.Instance.gameObject.SetActive(false);
       if (SoundSystem.Instance != null)SoundSystem.Instance.gameObject.SetActive(false);
       this.gameObject.SetActive(true);
    }

    public void SetInvisiblemode()
    {
        _tileObj.SetActive(false);
        this.gameObject.SetActive(false);
    }

    public void SetVisiblemode() => this.gameObject.SetActive(true);
    public void MapGrabmode() => this.GetComponent<XRGrabInteractable>().enabled = true;
    public void MapUnGrabmode()=> this.GetComponent<XRGrabInteractable>().enabled = false;
    

    public void CatchObjectInitialize()
    {

            if (PlacementSystem.Instance.InsertObj != null)
            {
                SelectEnterEventArgs p = PlacementSystem.Instance.InsertObjSelectEventArgs;
                p.manager.CancelInteractableSelection(p.interactableObject);
                p.manager.CancelInvoke();
            }
            if(PlacementSystem.Instance.CreateObj != null)
            {
                SelectEnterEventArgs p = PlacementSystem.Instance.CreateObjSelectEventArgs;
                GameObject a = PlacementSystem.Instance.CreateObj;
                PlacementSystem.Instance.CreateObj = null;
                PlacementSystem.Instance.SetCatchmode(false);    
                Destroy(a);

            if (p != null)  p.manager.CancelInteractableSelection(p.interactableObject);
            }
    }


    public void Catchout()
    {
        Vector3 rotate = this.gameObject.transform.rotation.eulerAngles;
        rotate.x = 0;
        rotate.y = 0;
        rotate.z = 0;
        this.transform.eulerAngles = rotate;
    }

}
