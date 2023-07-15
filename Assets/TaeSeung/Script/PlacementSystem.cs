using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    GameObject mouseIndicator, cellIndicator;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;

    [SerializeField]
    private ObjectDatabaseSO database;
    private int selectedObjectIndex = -1;

    [SerializeField]
    private GameObject gridVisualization;
    [SerializeField]
    private GameObject StructureControlUI;

    [SerializeField]
    private UIInitialize UIscript;

    private GridData floorData, funitureData;
    
    private Renderer previewRenderer;

    private List<GameObject> placedGameObjects = new();

    private void Start()
    {
        //Call Placed Funiture from database
        for (short i = 0; i < database.objectsLocation.Count; i++) {
            Vector3Int loc = database.objectsLocation[i].location;
            Quaternion rot = database.objectsLocation[i].rotation;
            int id = database.objectsLocation[i].OBJID;
            GameObject newObject = Instantiate(database.objectsData[id].Prefab);
            newObject.transform.rotation = rot;
            newObject.transform.position = grid.CellToWorld(loc);
            newObject.layer = LayerMask.NameToLayer("PlaceObject");
        }

        StopPlacement();
        funitureData = new();
        floorData = new();
        previewRenderer = cellIndicator.GetComponentInChildren<Renderer>();

    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex < 0)
        {
            Debug.LogError($"No ID found {ID}");
            return;
        }

        if (database.objectsData[selectedObjectIndex].ObjectCount <= 0)
        {
            Debug.Log($"No Object");
            return;
        }

        gridVisualization.SetActive(true);
        cellIndicator.SetActive(true);
        StructureControlUI.SetActive(true);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI())
        {
            return;
        }

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = new Vector3Int(Mathf.RoundToInt(mousePosition.x), Mathf.RoundToInt(mousePosition.y), Mathf.RoundToInt(mousePosition.z)); 
        ObjectLocation newlocation = new ObjectLocation();

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (placementValidity == false)
            return;

        //새로 배치될 물체의 위치, 회전 정보를 데이터베이스에 넣는 과정
        newlocation.location = gridPosition;
        newlocation.rotation = database.objectsData[selectedObjectIndex].Prefab.transform.rotation;
        newlocation.OBJID = selectedObjectIndex;
        database.objectsLocation.Add(newlocation);

        //실제 배치
        GameObject newObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);
        newObject.transform.position = grid.CellToWorld(gridPosition);
        newObject.layer = LayerMask.NameToLayer("PlaceObject");
        database.objectsData[selectedObjectIndex].ObjectCount -= 1;
        UIscript.countlist[selectedObjectIndex].GetComponentInChildren<TMP_Text>().text = "" + database.objectsData[selectedObjectIndex].ObjectCount;
        placedGameObjects.Add(newObject);
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ?
            floorData :
            funitureData;
        selectedData.AddObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size,
            database.objectsData[selectedObjectIndex].ID,
            placedGameObjects.Count - 1);

        //수량이 없는 경우 해당 오브젝트 비활성화
        if (database.objectsData[selectedObjectIndex].ObjectCount <= 0)
        {
                Debug.Log($"No Object");
                UIscript.countlist[selectedObjectIndex].GetComponent<Button>().interactable = false;
                StopPlacement();
        }
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? 
            floorData : 
            funitureData;


        return selectedData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }

    public void StopPlacement()
    {
        selectedObjectIndex = -1;
        gridVisualization.SetActive(false);
        cellIndicator.SetActive(false);
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
    }

    public void RotateStructure()
    {
        if (selectedObjectIndex == -1)
            return;
        
        Vector3 objrotation = database.objectsData[selectedObjectIndex].Prefab.transform.rotation.eulerAngles;
        objrotation.y += 90;
        database.objectsData[selectedObjectIndex].Prefab.transform.rotation = Quaternion.Euler(objrotation);
  
    }

    private void Update()
    {
        if (selectedObjectIndex < 0)
            return;

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        previewRenderer.material.color = placementValidity ? Color.white : Color.red;


        mouseIndicator.transform.position = mousePosition;
        cellIndicator.transform.position = new Vector3(Mathf.Round(mousePosition.x), Mathf.Round(mousePosition.y), Mathf.Round(mousePosition.z));

        
    }
}
