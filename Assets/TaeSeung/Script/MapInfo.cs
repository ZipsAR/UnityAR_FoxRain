using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInfo : Singleton<MapInfo>
{
    // Start is called before the first frame update
    public Vector2Int Mapsize;
    public float MapScale;
    public bool initialize;

    [SerializeField]
    private GameObject TileMap, Tile, Cursor, Plane;
    [SerializeField]
    private Material gridgraph;

    private Vector3 TileMapscale, TileScale, CursorScale, PlaneScale, TileEffectScale;



    private void Start()
    {
        if(initialize) MapInitialize();
        TileEffectScale = Tile.transform.GetChild(0).localScale;
    }



    public void MapInitialize()
    {
        float reverseMapScale = 1 / MapScale;

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

    public void ResetTileScale()
    {
        Tile.transform.GetChild(0).localScale = TileEffectScale;

    }



}
