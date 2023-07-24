using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInfo : Singleton<MapInfo>
{
    // Start is called before the first frame update
    public Vector2Int Mapsize;
    public float MapScale;

    [SerializeField]
    private GameObject TileMap, Tile, Cursor, Plane;
    [SerializeField]
    private Material gridgraph;

    private Vector3 TileMapscale, TileScale, CursorScale, PlaneScale;


    private void Start()
    {
        MapInitialize();
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
        if (Plane)
        {
            PlaneScale = Plane.transform.localScale;
            Plane.transform.localScale = PlaneScale * reverseMapScale;
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
        float reverseMapScale = 1 / MapScale;
        Tile.transform.localScale = scale * reverseMapScale;
    }


}
