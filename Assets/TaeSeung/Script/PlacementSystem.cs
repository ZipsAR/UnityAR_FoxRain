using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


//배치와 관련된 모든 함수가 담김.

public class PlacementSystem : MonoBehaviour
{
    //mouseindicator : cursor 오브젝트, cellindicator : cell위치 표시해주는 오브젝트
    [SerializeField]
    GameObject mouseIndicator, cellIndicator;
    [SerializeField]
    CursorCollisionSystem cursorsystem;


    //inputmanager클래스 
    [SerializeField]
    private InputManager inputManager;

    //그리드 컴포넌트
    [SerializeField]
    private Grid grid;

    //database 스크립터블 오브젝트 
    //데이터베이스 인덱스
    [SerializeField]
    private ObjectDatabaseSO database;
    private int selectedObjectIndex = -2;

    //grid시각화 오브젝트
    [SerializeField]
    private GameObject gridVisualization;

    //UI관련 스크립트 및 오브젝트
    [SerializeField]
    private GameObject StructureControlUI;
    [SerializeField]
    private UIInitialize UIscript;



    private GridData floorData, funitureData;
    private Renderer previewRenderer;
    private List<GameObject> placedGameObjects = new();


    //현재 오브젝트의 size 상태(rotation때문에 추가됨)
    private Vector2Int currentobjsize;
    private Quaternion currentrotation;

    private GameObject cursororigin;
    private GameObject cursorparent;
    private bool catchmode = false;
    private Vector3Int currentpos;

    private void Start()
    {
        funitureData = new();
        floorData = new();

        //스크립터블 오브젝트에서 이미 배치된 데이터들 가져오기
        for (short i = 0; i < database.objectsLocation.Count; i++) {
            Vector3Int loc = database.objectsLocation[i].location;
            Quaternion rot = database.objectsLocation[i].rotation;
            Vector2Int size = database.objectsLocation[i].size;
            int id = database.objectsLocation[i].OBJID;

            GameObject newObject = Instantiate(database.objectsData[id].Prefab);
            newObject.transform.rotation = rot;
            newObject.transform.position = grid.CellToWorld(loc);
            newObject.layer = LayerMask.NameToLayer("PlaceObject");

            placedGameObjects.Add(newObject);
            //GridData selectedData = database.objectsData[i].ID == 0 ? floorData : funitureData;
            GridData selectedData = funitureData;
            selectedData.AddObjectAt(loc, size, id, placedGameObjects.Count - 1);
        }

        StopPlacement();
        previewRenderer = cellIndicator.GetComponentInChildren<Renderer>();
        cursororigin = mouseIndicator;
        cursorparent = cursororigin.transform.parent.gameObject;
    }

    private void Update()
    {
        if ((selectedObjectIndex < 0 && selectedObjectIndex !=-2))
            return;
        if (!mouseIndicator)
            return;

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = new Vector3Int(Mathf.RoundToInt(mousePosition.x), Mathf.RoundToInt(mousePosition.y), Mathf.RoundToInt(mousePosition.z));

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        previewRenderer.material.color = placementValidity ? Color.white : Color.red;

        mouseIndicator.transform.position = mousePosition;
        cellIndicator.transform.position = new Vector3(Mathf.Round(mousePosition.x), Mathf.Round(mousePosition.y), Mathf.Round(mousePosition.z));
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

        //cellindicator 크기 설정(차지 영역)
        currentobjsize = database.objectsData[selectedObjectIndex].Size;
        Vector3 scale = new Vector3(currentobjsize.x, currentobjsize.y, 1);
        cellIndicator.transform.localScale = scale;

        currentrotation = new();
        gridVisualization.SetActive(true);
        cellIndicator.SetActive(true);
        StructureControlUI.SetActive(true);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }


    public void Startinsertion()
    {
        StopPlacement();
        selectedObjectIndex = -2;
        gridVisualization.SetActive(true);
        //cellIndicator.SetActive(true);
        StructureControlUI.SetActive(true);
     
        inputManager.OnClicked += InsertionStructure;
        inputManager.OnExit += StopPlacement;

    }



    public void StopPlacement()
    {
        selectedObjectIndex = -1;
        gridVisualization.SetActive(false);
        cellIndicator.SetActive(false);
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnClicked -= InsertionStructure;
        inputManager.OnExit -= StopPlacement;
    }



    //구조물 배치
    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI())
        {
            return;
        }

        //현재 커서 위치 가져옴
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();

        if (!inputManager.ishit()) return;

        //현재 커서 위치를 기반으로 grid.WorldToCell하면 월드좌표계를 grid컴포넌트의 그리드로 즉시 변환해주지만, 이상하게 변환되서(버림연산함) 그냥 round시키는 방식으로 바꿈
        Vector3Int gridPosition = new Vector3Int(Mathf.RoundToInt(mousePosition.x), Mathf.RoundToInt(mousePosition.y), Mathf.RoundToInt(mousePosition.z)); 
        
        ObjectLocation newlocation = new ObjectLocation();

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (placementValidity == false)
            return;

        //새로 배치될 물체의 위치, 회전 정보를 데이터베이스에 넣는 과정
        newlocation.location = gridPosition;
        newlocation.rotation = currentrotation;
        newlocation.OBJID = selectedObjectIndex;
        newlocation.size = currentobjsize;
        database.objectsLocation.Add(newlocation);

        //실제 배치
        GameObject newObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);
        newObject.transform.position = grid.CellToWorld(gridPosition);
        newObject.transform.rotation = currentrotation;
        newObject.layer = LayerMask.NameToLayer("PlaceObject");

        database.objectsData[selectedObjectIndex].ObjectCount -= 1;
        UIscript.countlist[selectedObjectIndex].GetComponentInChildren<TMP_Text>().text = "" + database.objectsData[selectedObjectIndex].ObjectCount;
        placedGameObjects.Add(newObject);

        //충돌 영역 추가
        //GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData :funitureData;
        GridData selectedData = funitureData;
        selectedData.AddObjectAt(gridPosition, currentobjsize,
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


    public void InsertionStructure()
    {
        if (inputManager.IsPointerOverUI())
        {
            return;
        }

        GameObject   obj = cursorsystem.GetCollisionobject();
        if (!obj) return;

        Vector3 mousePosition = obj.transform.position;
        Vector3Int gridPosition = new Vector3Int(Mathf.RoundToInt(mousePosition.x), Mathf.RoundToInt(mousePosition.y), Mathf.RoundToInt(mousePosition.z));
        
        if (!catchmode)
        {
            //잡은 시점에서의 물체 위치 저장 (삭제에서 사용될지도 몰라요)
            mouseIndicator = obj;
            currentpos = gridPosition;
            cursororigin.transform.SetParent(mouseIndicator.transform);
            catchmode = true;
            cellIndicator.SetActive(true);
        }
        else
        {
            //맵 바깥으로 커서를 이동시켰다면 삭제
            inputManager.GetSelectedMapPosition();
            if (!inputManager.ishit())
            {
                cursororigin.transform.SetParent(cursorparent.transform);
                int index = database.objectsLocation.FindIndex(data => data.location == currentpos);
                if (index >= 0)
                {
                    int id = database.objectsLocation[index].OBJID;
                    Vector2Int size = database.objectsLocation[index].size;
                    funitureData.RemoveObjectAt(currentpos, size);
                    database.objectsData[id].ObjectCount++;
                    database.objectsLocation.RemoveAt(index);
                    Destroy(mouseIndicator);
                    mouseIndicator = cursororigin;
                    catchmode = false;

                    cellIndicator.SetActive(false);
                }
            }

            //아직 커서가 맵 안에 있다면 유효범위이므로 위치 수정
            else
            {
                int index = database.objectsLocation.FindIndex(data => data.location == currentpos);
                if (index >= 0)
                {
                    int id = database.objectsLocation[index].OBJID;
                    Vector2Int size = database.objectsLocation[index].size;
                    PlacementData data = funitureData.GetObjectAt(currentpos);
                    int placeindex = data.PlacedObjectIndex;

                    if (funitureData.CanPlaceObjectAt(gridPosition,size))
                    {
                        print("dds");
                        cursororigin.transform.SetParent(cursorparent.transform);
                        funitureData.RemoveObjectAt(currentpos, size);
                        funitureData.AddObjectAt(gridPosition, size, id, placeindex);
                        database.objectsLocation[index].location = gridPosition;

                        mouseIndicator.transform.position = grid.CellToWorld(gridPosition);
                        mouseIndicator = cursororigin;

                        catchmode = false;
                        cellIndicator.SetActive(false);

                    }
                }
            }

        }


        //cursororigin.SetActive(false);
    }



    //배치 가능영역인지 파악
    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        //GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : funitureData;
        GridData selectedData = funitureData;
        print("objsize: " + currentobjsize);

        return selectedData.CanPlaceObjectAt(gridPosition, currentobjsize);
    }


    public void RotateStructure()
    {
        if (selectedObjectIndex == -1)
            return;


        Vector3 objrotation = currentrotation.eulerAngles; 

        objrotation.y += 90;
        int tempx = currentobjsize.x;
        currentobjsize.x = currentobjsize.y;
        currentobjsize.y = tempx;

        Vector3 scale = new Vector3(currentobjsize.x, currentobjsize.y, 1);
        cellIndicator.transform.localScale = scale;

        currentrotation = Quaternion.Euler(objrotation);
  
    }

}
