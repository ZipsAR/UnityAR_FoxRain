using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MapInfo : Singleton<MapInfo>
{
    // Start is called before the first frame update
    public Vector2Int Mapsize;
    public float MapScale;
    private float FirstScale;
    public bool initialize;

    [SerializeField]
    private GameObject TileMap, Tile, Cursor, Plane;
    [SerializeField]
    private Material gridgraph;

    private Vector3 TileMapscale, TileScale, CursorScale, TileEffectScale;


    public List<bool> housingtutorial;

    private void Start()
    {
        if(initialize) MapInitialize();
        TileEffectScale = Tile.transform.GetChild(0).localScale;
        SetOrigin();
    }

    public void SetOrigin()
    {
        this.transform.position = new Vector3(0, -0.6f, 1);
        this.transform.eulerAngles = new Vector3(0, 0, 0);
    }

    public void SetAnotherPosition(Vector3 newposition)
    {
        this.transform.position = newposition;
    }


    public void MapInitialize()
    {
        float reverseMapScale = 1 / MapScale;
        FirstScale = MapScale;
        if (TileMap)
        {
            TileMapscale = TileMap.transform.localScale;
            TileMap.transform.localScale = TileMapscale * reverseMapScale;
        }
        if (Cursor)
        {
            CursorScale = Cursor.transform.localScale;
            Cursor.transform.localScale = CursorScale * reverseMapScale;
        }
        if (gridgraph)
        {
            gridgraph.SetVector("_Size", new Vector2(1, 1));
            TileScale = gridgraph.GetVector("_Size");
            gridgraph.SetVector("_Size", TileScale * MapScale);
        }
    }

    
    public void SetTileScale(Vector3 scale)
    {
        Tile.transform.localScale = scale;
        Vector3 effectscale = TileEffectScale;
        effectscale.x = effectscale.x / scale.x;
        effectscale.y = effectscale.y / scale.y;
        effectscale.z = effectscale.z / scale.z;
        Tile.transform.GetChild(0).localScale = effectscale;
    }




    public void SetReScale(float rescale){

        MapScale = rescale;
        float reverseMapScale = 1 / MapScale;

        if (TileMap)
            TileMap.transform.localScale = TileMapscale * reverseMapScale;

        if (Cursor)
            Cursor.transform.localScale = CursorScale * reverseMapScale;

        if (gridgraph)
            gridgraph.SetVector("_Size", TileScale * MapScale);
    }

    public void ResetTileScale()
    {
        Tile.transform.GetChild(0).localScale = TileEffectScale;
    }

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
        Tile.SetActive(false);
        if (HousingUISystem.Instance != null)  HousingUISystem.Instance.transform.gameObject.SetActive(false);
       if (EffectSystem.Instance != null) EffectSystem.Instance.gameObject.SetActive(false);
       if (SoundSystem.Instance != null)SoundSystem.Instance.gameObject.SetActive(false);
       this.gameObject.SetActive(true);
    }

    public void SetInvisiblemode()
    {
        Tile.SetActive(false);
        this.gameObject.SetActive(false);
    }

    public void SetVisiblemode()
    {
        this.gameObject.SetActive(true);
    }


    public void MapGrabmode(){
        this.GetComponent<XRGrabInteractable>().enabled = true;
    }

    public void MapUnGrabmode() {
        this.GetComponent<XRGrabInteractable>().enabled = false;
    }

    public void CatchObjectInitialize()
    {

            if (PlacementSystem.Instance.InsertObject != null)
            {
                SelectEnterEventArgs p = PlacementSystem.Instance.InsertManager;
                p.manager.CancelInteractableSelection(p.interactableObject);
                p.manager.CancelInvoke();
            }
            if(PlacementSystem.Instance.CreateObject != null)
            {
                SelectEnterEventArgs p = PlacementSystem.Instance.CreateManager;
                GameObject a = PlacementSystem.Instance.CreateObject;
                PlacementSystem.Instance.CreateObject = null;
                PlacementSystem.Instance.SetCatchmode(false);    
                Destroy(a);

            if (p != null)
            {
                p.manager.CancelInteractableSelection(p.interactableObject);
            }
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
