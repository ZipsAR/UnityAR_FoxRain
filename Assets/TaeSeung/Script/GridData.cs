using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//grid 위치에 따른 오브젝트 배치를 관리해주는 함수
public class GridData 
{
    //배치 영역에 대한 정보가 담겨있음.
    Dictionary<Vector3Int, PlacementData> placedObjects = new();

    public void AddObjectAt(Vector3Int gridPosition,
        Vector2Int objectSize,
        int ID,
        int placedObjectIndex)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);
        foreach(var pos in positionToOccupy) {
            //if (placedObjects.ContainsKey(pos))
                //throw new System.Exception($"Dictionary already contains this cell position {pos}");
            placedObjects[pos] = data;

            } 
    }

    public void RemoveObjectAt(Vector3Int gridPosition,
    Vector2Int objectSize)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach (var pos in positionToOccupy)
            placedObjects.Remove(pos);
    }

    public PlacementData GetObjectAt(Vector3Int At)
    {
        PlacementData data = placedObjects[At];
        return data;
    }


    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition,Vector2Int objectSize)
    {
        List<Vector3Int> returnval = new();
        gridPosition = gridPosition - new Vector3Int(Mathf.FloorToInt(objectSize.x / 2), 0,Mathf.FloorToInt(objectSize.y /2));
        for (int x= 0; x<objectSize.x; x++)
        {
            for(int y=0; y<objectSize.y; y++)
            {
                returnval.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }

        return returnval;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach(var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
                return false;
    
        }
        return true;
    }
}

public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int ID { get; private set; }
    public int PlacedObjectIndex { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, int ID, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        this.ID = ID;
        this.PlacedObjectIndex = placedObjectIndex;
    }
}
