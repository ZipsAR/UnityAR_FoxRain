using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


//��ġ�� ���õ� ��� �Լ��� ���.

public class PlacementSystem : MonoBehaviour
{
    //mouseindicator : cursor ������Ʈ, cellindicator : cell��ġ ǥ�����ִ� ������Ʈ
    [SerializeField]
    GameObject mouseIndicator, cellIndicator;
    [SerializeField]
    CursorCollisionSystem cursorsystem;


    //inputmanagerŬ���� 
    [SerializeField]
    private InputManager inputManager;

    //�׸��� ������Ʈ
    [SerializeField]
    private Grid grid;

    //database ��ũ���ͺ� ������Ʈ 
    //�����ͺ��̽� �ε���
    [SerializeField]
    private ObjectDatabaseSO database;
    private int selectedObjectIndex = -2;

    //grid�ð�ȭ ������Ʈ
    [SerializeField]
    private GameObject gridVisualization;

    //UI���� ��ũ��Ʈ �� ������Ʈ
    [SerializeField]
    private GameObject StructureControlUI;

    [SerializeField]
    private GameObject ObjectLocation;

    private GridData floorData, funitureData;
    private Renderer previewRenderer;
    private List<GameObject> placedGameObjects = new();


    //���� ������Ʈ�� size ����(rotation������ �߰���)
    private Vector2Int currentobjsize;
    private Quaternion currentrotation;

    private GameObject cursororigin;
    private GameObject cursorparent;
    private bool catchmode = false;
    private Vector3Int currentpos;


    public GameObject Debugpoint;

    private void Start()
    {
        funitureData = new();
        floorData = new();

        //��ũ���ͺ� ������Ʈ���� �̹� ��ġ�� �����͵� ��������
        for (short i = 0; i < database.objectsLocation.Count; i++) {
            Vector3Int loc = database.objectsLocation[i].location;
            Quaternion rot = database.objectsLocation[i].rotation;
            Vector2Int size = database.objectsLocation[i].size;
            int id = database.objectsLocation[i].OBJID;
            GameObject newObject = MakeNewObject(id, ObjectLocation.transform, loc, rot, size, "PlaceObject");
        }

        StopPlacement();

        previewRenderer = cellIndicator.GetComponentInChildren<Renderer>();
        cursororigin = mouseIndicator;
        cursorparent = cursororigin.transform.parent.gameObject;

        MapInfo.Instance.MapInitialize();
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

        mousePosition = mousePosition * MapInfo.Instance.MapScale;
        mousePosition = new Vector3(Mathf.Round(mousePosition.x), Mathf.Round(mousePosition.y), Mathf.Round(mousePosition.z));
        cellIndicator.transform.position = mousePosition / MapInfo.Instance.MapScale;

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

        //cellindicator ũ�� ����(���� ����)
        currentobjsize = database.objectsData[selectedObjectIndex].Size;

        MapInfo.Instance.SetTileScale(new Vector3(currentobjsize.x, currentobjsize.y, 1));

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
        //InputManagerEventControlManager(PlaceStructure);
    }


    private void InputManagerEventControlManager(Action onclick, Action onexit, bool onclickcondi, bool onexitcondi)
    {
        if(!onclickcondi)
            inputManager.OnClicked -= PlaceStructure;

        inputManager.OnClicked -= InsertionStructure;
        inputManager.OnExit -= StopPlacement;


        if (onclickcondi)
            inputManager.OnClicked += onclick;
        if(onexitcondi)
            inputManager.OnExit += onexit;


        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnClicked -= InsertionStructure;
        inputManager.OnExit -= StopPlacement;

    }


    //������ ��ġ
    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI())
        {
            return;
        }

        //���� Ŀ�� ��ġ ������
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();

        if (!inputManager.ishit()) return;


        //���� Ŀ�� ��ġ�� ������� grid.WorldToCell�ϸ� ������ǥ�踦 grid������Ʈ�� �׸���� ��� ��ȯ��������, �̻��ϰ� ��ȯ�Ǽ�(����������) �׳� round��Ű�� ������� �ٲ�
        mousePosition = mousePosition * MapInfo.Instance.MapScale;
        Vector3Int gridPosition = new Vector3Int(Mathf.RoundToInt(mousePosition.x), Mathf.RoundToInt(mousePosition.y), Mathf.RoundToInt(mousePosition.z)); 
        ObjectLocation newlocation = new ObjectLocation();

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (placementValidity == false)
            return;

        //���� ��ġ�� ��ü�� ��ġ, ȸ�� ������ �����ͺ��̽��� �ִ� ����
        newlocation.location = gridPosition;
        newlocation.rotation = currentrotation;
        newlocation.OBJID = selectedObjectIndex;
        newlocation.size = currentobjsize;
        database.objectsLocation.Add(newlocation);


        //������ġ
        GameObject newobject = MakeNewObject(selectedObjectIndex, ObjectLocation.transform, gridPosition, currentrotation, newlocation.size, "PlaceObject");
        UIInitialize.Instance.countlist[selectedObjectIndex].GetComponentInChildren<TMP_Text>().text = "" + database.objectsData[selectedObjectIndex].ObjectCount;


        //������ ���� ��� �ش� ������Ʈ ��Ȱ��ȭ
        if (database.objectsData[selectedObjectIndex].ObjectCount <= 0)
        {
                Debug.Log($"No Object");
                UIInitialize.Instance.countlist[selectedObjectIndex].GetComponent<Button>().interactable = false;
                StopPlacement();
        }
    }


    //��ġ�� ��ü�� ���� ��ġ���� �Ǵ� ����
    public void InsertionStructure()
    {
        if (inputManager.IsPointerOverUI())
        {
            return;
        }

        GameObject   obj = cursorsystem.GetCollisionobject();
        if (!obj) return;

        Vector3 mousePosition = obj.transform.position * MapInfo.Instance.MapScale;
        Vector3Int gridPosition = new Vector3Int(Mathf.RoundToInt(mousePosition.x), Mathf.RoundToInt(mousePosition.y), Mathf.RoundToInt(mousePosition.z));
        
        if (!catchmode)
        {
            //���� ���������� ��ü ��ġ ���� (�������� �������� �����)
            mouseIndicator = obj;
            currentpos = gridPosition;
            cursororigin.transform.SetParent(mouseIndicator.transform);
            catchmode = true;
            cellIndicator.SetActive(true);
        }

        else
        {
            //�� �ٱ����� Ŀ���� �̵����״ٸ� ����
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

            //���� Ŀ���� �� �ȿ� �ִٸ� ��ȿ�����̹Ƿ� ��ġ ����
            else
            {
                int index = database.objectsLocation.FindIndex(data => data.location == currentpos);
                if (index >= 0)
                {
                    int id = database.objectsLocation[index].OBJID;
                    Vector2Int size = database.objectsLocation[index].size;
                    PlacementData data = funitureData.GetObjectAt(currentpos);
                    int placeindex = data.PlacedObjectIndex;

                    //��ġ ��ġ ������ ��ġ�� �̹� �ٸ� ������ ��ġ�Ǿ� ������ �� �ſ�.
                    if (funitureData.CanPlaceObjectAt(gridPosition,size))
                    {
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
    }

    //��ġ ���ɿ������� �ľ�
    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        //GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : funitureData;
        GridData selectedData = funitureData;

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
        cellIndicator.transform.localScale = scale / MapInfo.Instance.MapScale;

        currentrotation = Quaternion.Euler(objrotation);
  
    }

    private GameObject MakeNewObject(int id, Transform parent, Vector3Int loc, Quaternion rot, Vector2Int size, String layer)
    {
        if (id >= 0)
        {
            GameObject newObject = Instantiate(database.objectsData[id].Prefab);
            newObject.transform.SetParent(ObjectLocation.transform);
            newObject.transform.rotation = rot;
            //newObject.transform.position = grid.CellToWorld(loc);
            newObject.transform.position = ((Vector3)loc) / MapInfo.Instance.MapScale;
            newObject.transform.localScale = newObject.transform.localScale * (1/MapInfo.Instance.MapScale);
            newObject.layer = LayerMask.NameToLayer(layer);

            placedGameObjects.Add(newObject);
            //GridData selectedData = database.objectsData[i].ID == 0 ? floorData : funitureData;
            GridData selectedData = funitureData;
            selectedData.AddObjectAt(loc, size, id, placedGameObjects.Count - 1);
            return newObject;
        }
        else return null;
    }
}




public class CurrentPointInfo{
    //���� ������Ʈ�� size ����(rotation������ �߰���)
    private Vector2Int currentobjsize { get; set; }
    private Quaternion currentrotation { get; set; }

    private GameObject cursororigin { get; set; }
    private GameObject cursorparent { get; set; }

    private bool catchmode = false;

    private Vector3Int currentpos { get; set; }

}