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
        //SetReScale(FirstScale);
        
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

        /*
        Vector3 last = cellIndicator.transform.GetChild(0).localPosition;
        last.y = 0.5f;
        cellIndicator.transform.GetChild(0).localPosition = last;
        */

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
        this.gameObject.SetActive(true);
    }

    public void SetMapNormalmode()
    {
        PlacementSystem.Instance.ProtectGrib();
        this.gameObject.SetActive(true);
    }

    public void SetInvisiblemode()
    {
        this.gameObject.SetActive(false);
    }

    public void MapGrabmode(){
        this.GetComponent<XRGrabInteractable>().enabled = true;
    }

    public void MapUnGrabmode() {
        this.GetComponent<XRGrabInteractable>().enabled = false;
    }


    public void Catchout()
    {
        Vector3 rotate = this.gameObject.transform.rotation.eulerAngles;
        rotate.x = 0;
        rotate.z = 0;
        this.transform.eulerAngles = rotate;
    }

}
