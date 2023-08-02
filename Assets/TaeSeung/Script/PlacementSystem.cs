using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using static UnityEngine.InputSystem.InputControlExtensions;


//��ġ�� ���õ� ��� �Լ��� ���.

public class PlacementSystem : Singleton<PlacementSystem>
{
    //mouseindicator : cursor ������Ʈ, cellindicator : cell��ġ ǥ�����ִ� ������Ʈ
    [SerializeField]
    GameObject mouseIndicator, cellIndicator;

    //������Ʈ ������ġ�� �� ��
    [SerializeField]
    private GameObject spawnpoint;

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


    //object�� ���� �����ɶ�, �� �θ� �� ������Ʈ
    [SerializeField]
    private GameObject ObjectLocation;

    //��ġ�� object�� Ÿ��
    private GridData floorData, funitureData;
    private Renderer previewRenderer;
    private List<GameObject> placedGameObjects = new();

    //���� ������Ʈ�� size ����(rotation������ �߰���), �� �κ��� ���߿� deprecated��ų���� �ֽ��̴�
    private GameObject CreateObject; //���� ������ ������Ʈ
    private GameObject CatchObject;  //������ ������Ʈ�� �� ���� ���� ������Ʈ
    private Vector2Int currentobjsize;  //������Ʈ�� �����ϴ� ĭ ������
    private Quaternion currentrotation; //������Ʈ�� �����̼�
    private float changerrotationzvalue; //������Ʈ �����̼ǵ� ��
    private Vector3Int currentpos;  //������Ʈ�� ��ġ
    private bool catchmode = false; //������Ʈ�� ���� ��������?
    private Vector3 beforepos;
    private XRGrabInteractable interact;    //������Ʈ�� interactable ������Ʈ

    private GameObject cursororigin, cursorparent;

    [Obsolete]
    [SerializeField]
    CursorCollisionSystem cursorsystem;

    public GameObject debugyoung;

    public GameObject gridv;

    
    
    private void Start()
    {
        funitureData = new();
        floorData = new();

        MapInfo.Instance.MapInitialize();

        //��ũ���ͺ� ������Ʈ���� �̹� ��ġ�� �����͵� ��������
        for (short i = 0; i < database.objectsLocation.Count; i++) {
            int id = database.objectsLocation[i].OBJID;
            GameObject newObject = Instantiate(database.objectsData[id].Prefab);
            newObject.transform.localScale = newObject.transform.localScale / MapInfo.Instance.MapScale;

            Vector3Int loc = database.objectsLocation[i].location;
            Quaternion rot = database.objectsLocation[i].rotation;
            Vector2Int size = database.objectsLocation[i].size;
            database.objectsLocation[i].InstanceId = newObject.GetInstanceID();

            MakeNewObject(id, ObjectLocation.transform, loc, rot, size, "PlaceObject", newObject);

            interact = newObject.GetComponent<XRGrabInteractable>();
            SelectEnterEventArgs enterArgs = makeEnterEventArgs(interact, interact.firstInteractorSelecting, interact.interactionManager);
            SelectExitEventArgs exitargs = makeExitEventArgs(interact, interact.firstInteractorSelecting, interact.interactionManager);
            interact.selectEntered.AddListener((a) => InsertEnterEvent(enterArgs));
            interact.selectExited.AddListener((a) => InsertCompleteEvent(exitargs));
        }
        StopPlacement(true);

        previewRenderer = cellIndicator.GetComponentInChildren<Renderer>();
        cursororigin = mouseIndicator;
        cursorparent = cursororigin.transform.parent.gameObject;


    }



    private void Update()
    {
       if (!mouseIndicator)
            return;

        if (catchmode)
        {
            PlaceCheck(CatchObject);
            RotateRealTimebyHand();
            Vector3 test = inputManager.GetSelectedMapPositionbyObjectForward(CatchObject.transform);
            debugyoung.transform.position = test;
        }
    }


    public void StartPlacement(int ID)
    {
        //�̹� ������ ������Ʈ�� ���� ���, �� �� �ı���Ű�� ���� ���� ������Ʈ�� ������ų ����, �ٵ� ��ȹ�� ���� ����ɼ��� ����
        if (CreateObject)
            Destroy(CreateObject);

        StopPlacement(true);
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

        //AR ȯ��󿡼��� �� �ڵ带 �־�� ��.
        GameObject newObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);
        CreateObject = newObject;
        newObject.transform.position = spawnpoint.transform.position;
        newObject.transform.localScale = newObject.transform.localScale * (1 / MapInfo.Instance.MapScale);
        interact = newObject.GetComponent<XRGrabInteractable>();

        //��ü�� grab�Ҷ��� �̺�Ʈ �߰�
        SelectEnterEventArgs enterargs = makeEnterEventArgs(interact, interact.firstInteractorSelecting, interact.interactionManager);
        interact.selectEntered.AddListener((a) => PlaceEnterEvent(enterargs));

        //��ü�� ���� grab�� Ǯ���� �̺�Ʈ �߰�
        SelectExitEventArgs exitargs = makeExitEventArgs(interact, interact.firstInteractorSelecting, interact.interactionManager);
        interact.selectExited.AddListener((a)=>PlaceEvent(exitargs));


        //��ǻ�Ϳ��� �Ҷ� �� �ڵ带 ������ �˴ϴ�.
        //inputManager.OnClicked += PlaceStructure;
        //inputManager.OnExit += StopPlacement;
    }

 
    public void Startinsertion()
    {
        StopPlacement(false);
        gridVisualization.SetActive(true);
     
        //inputManager.OnClicked += InsertionStructure;
        //inputManager.OnExit += StopPlacement;

    }

    public void StopPlacement(bool indexinitialize)
    {
        if(indexinitialize)
            selectedObjectIndex = -1;

        gridVisualization.SetActive(false);
        cellIndicator.SetActive(false);
        catchmode = false;
        //��ǻ�� ȯ�濡���� ��� ����
        //inputManager.OnClicked -= PlaceStructure;
        //inputManager.OnClicked -= InsertionStructure;
        //inputManager.OnExit -= StopPlacement;
        //InputManagerEventControlManager(PlaceStructure);


    }


    private void PlaceStartStructure()
    {
        if (!gridVisualization.activeSelf)
            gridVisualization.SetActive(true);
        if (!cellIndicator.activeSelf)
            cellIndicator.SetActive(true);

        catchmode = true;
    }


    private void PlaceCheck(GameObject catchobject)
    {
        Vector3 mousePosition = inputManager.GetSelectedMapPositionbyObject(catchobject.transform);
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        
        //��ġ �Ұ����ϸ� cellindicator�� ���͸����� �ٲ㼭 ǥ������.
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        previewRenderer.material.color = placementValidity ? Color.white : Color.red;

        mouseIndicator.transform.position = mousePosition;
        cellIndicator.transform.localPosition = PlacePosition(gridPosition, currentobjsize);

    }

    private Vector3 PlacePosition(Vector3Int gridposition, Vector2Int size)
    {
        Vector3 Cellposition = gridposition;
        float z = Math.Abs(cellIndicator.transform.eulerAngles.y);
        int sizex = size.x;
        int sizey = size.y;

        if ((z == 90 || z == 270) && (sizex+sizey) % 2 == 1)
        {
            int temp = sizey;
            sizey = sizex;
            sizex = temp;

            /*
            Vector3 last = cellIndicator.transform.GetChild(0).localPosition;
            last.y = 0.5f;
            cellIndicator.transform.GetChild(0).localPosition = last;
            */

        }

        if (sizex%2 == 1)
        Cellposition.x = Cellposition.x + 0.5f;

        if(sizey%2 == 1)
        Cellposition.z = Cellposition.z + 0.5f;

        Cellposition.y = 0.1f;





        return Cellposition;
    }
    

    //������ ��ġ (ARȯ��)
    private void PlaceStructure(GameObject gameObject)
    {
        print(currentobjsize);
        print("ToQLd!");
        //���� Ŀ�� ��ġ ������
        Vector3 mousePosition = inputManager.GetSelectedMapPositionbyObject(gameObject.transform);

        if (!inputManager.ishit())
        {
            gameObject.transform.position = spawnpoint.transform.position;
            gameObject.transform.rotation = spawnpoint.transform.rotation;
            StopPlacement(false);

            return;
        }

        //mousePosition = mousePosition * MapInfo.Instance.MapScale;

        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        ObjectLocation newlocation = new ObjectLocation();

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (placementValidity == false)
        {
            gameObject.transform.position = spawnpoint.transform.position;
            gameObject.transform.rotation = spawnpoint.transform.rotation;
            StopPlacement(false);
            return;
        }


        //���� ��ġ�� ��ü�� ��ġ, ȸ�� ������ �����ͺ��̽��� �ִ� ����
        newlocation.location = gridPosition;
        newlocation.rotation = currentrotation;
        newlocation.OBJID = selectedObjectIndex;
        newlocation.size = currentobjsize;
        newlocation.InstanceId = gameObject.GetInstanceID();
        newlocation.placementstatus = true;

        float k = RotateRealTimebyHand();
        RotatePlacementByHand(k);

        MakeNewObject(selectedObjectIndex, ObjectLocation.transform, gridPosition, currentrotation, newlocation.size, "PlaceObject",gameObject);
        gameObject.transform.localPosition = cellIndicator.transform.localPosition;
        EffectSystem.Instance.playplaceeffect(cellIndicator.transform.localPosition);
        SoundSystem.Instance.TurnAudio(cellIndicator.transform.position);


        //������� ������Ʈ�� ���� ��������, ��ġ���� ���� + �� ������Ʈ�� ���� ��� ���� ����Ʈ�� �߰�
        database.objectsLocation.Add(newlocation);
        database.objectsData[selectedObjectIndex].ObjectCount -= 1;

        interact = gameObject.GetComponent<XRGrabInteractable>();
        SelectEnterEventArgs enterArgs = makeEnterEventArgs(interact, interact.firstInteractorSelecting, interact.interactionManager);
        SelectExitEventArgs exitargs = makeExitEventArgs(interact, interact.firstInteractorSelecting, interact.interactionManager);
        interact.selectEntered.RemoveAllListeners();
        interact.selectExited.RemoveAllListeners();
        interact.selectEntered.AddListener((a) => InsertEnterEvent(enterArgs));
        interact.selectExited.AddListener((a) => InsertCompleteEvent(exitargs));



        //������ ���� ��� �ش� ������Ʈ ��ư ��ü�� ��Ȱ��ȭ
        if (database.objectsData[selectedObjectIndex].ObjectCount <= 0)
        {
            Debug.Log($"No Object");
            UIInitialize.Instance.countlist[selectedObjectIndex].GetComponent<Button>().interactable = false;
        }
        UIInitialize.Instance.ObjCountupdate(selectedObjectIndex);



        CreateObject = null;
        StopPlacement(true);
    }


    public void InsertionStartStructure(GameObject gameObject)
    {
        int index = database.objectsLocation.FindIndex(data => data.InstanceId == gameObject.GetInstanceID());
        if (index < 0) return;

        currentobjsize =  database.objectsData[database.objectsLocation[index].OBJID].Size;
        MapInfo.Instance.SetTileScale(new Vector3(currentobjsize.x, currentobjsize.y, 1));

        //���� ���������� ��ü ��ġ ���� (�������� �������� �����)
        CatchObject = gameObject;
        cellIndicator.SetActive(true);
        catchmode = true;
        currentpos = database.objectsLocation[index].location;
    }

    private void InsertionStructure(GameObject gameObject) {

        Vector3 mousePosition = inputManager.GetSelectedMapPositionbyObject(gameObject.transform);
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        //�� �ٱ����� Ŀ���� �̵����״ٸ� ����
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
                    catchmode = false;
                    cellIndicator.SetActive(false);

                    if (gameObject.GetComponent<Rigidbody>())
                    {
                        gameObject.GetComponent<Rigidbody>().useGravity = true;
                        gameObject.GetComponent<Rigidbody>().isKinematic = false;
                    }
                    print(Vector3.Distance(gameObject.transform.position, ObjectLocation.transform.position));
                    CreateObject = null;
                    StartCoroutine(throwdelete(gameObject));
                    //Destroy(gameObject);
                }
            }

            //���� Ŀ���� �� �ȿ� �ִٸ� ��ȿ�����̹Ƿ� ��ġ ����
            else
            {
                int index = database.objectsLocation.FindIndex(data => data.InstanceId == gameObject.GetInstanceID());

                //�ϴ� �����ϴ� ������ �´��� ���� Ȯ���ؿ�
                if (index >= 0)
                {
                    int id = database.objectsLocation[index].OBJID;
                    Vector3Int pos = database.objectsLocation[index].location;
                    Quaternion rot = database.objectsLocation[index].rotation;
                    Vector2Int size = database.objectsLocation[index].size;
                    
                    PlacementData data = funitureData.GetObjectAt(currentpos);
                    int placeindex = data.PlacedObjectIndex;

                    //��ġ ��ġ ������ ��ġ�� �̹� �ٸ� ������ ��ġ�Ǿ� ������ �� �ſ�. ��, ������ġ�� ���ġ�ϴ� �� �뼭����
                    if (funitureData.CanPlaceObjectAt(gridPosition, currentobjsize) && gridPosition != pos)
                    {
                        //��ġ�� ��ġ ���� �����͸� �ٲ����
                        funitureData.RemoveObjectAt(currentpos, size);
                        funitureData.AddObjectAt(gridPosition, size, id, placeindex);
                        database.objectsLocation[index].location = gridPosition;
                        database.objectsLocation[index].rotation = currentrotation;
                        database.objectsLocation[index].size = currentobjsize;

                    //��ġ�� ��ġ�� �����ؿ�
                        gameObject.transform.rotation = currentrotation;
                        gameObject.transform.localPosition = cellIndicator.transform.localPosition;
                        EffectSystem.Instance.playplaceeffect(cellIndicator.transform.localPosition);
                        SoundSystem.Instance.TurnAudio(cellIndicator.transform.position);
                        //�� �ٺ����� �滩����
                    }
                    //�߸� ��ġ������ �׳� ���� �ִ��ڸ��� ������
                    else
                    {
                        Vector2Int tempsize = currentobjsize;
                        tempsize.x = size.y;
                        tempsize.y = size.x;

                        gameObject.transform.rotation = rot;
                        gameObject.transform.localPosition = PlacePosition(currentpos, size);
                }
                    catchmode = false;
                    cellIndicator.SetActive(false);
                 }
            CreateObject = null;
        }
    }


    /// <summary>
    /// ��ġ �� ������Ʈ ������ �߷� Ǯ�� �� �ָ��� ��������
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private IEnumerator throwdelete(GameObject obj)
    {
         while (Vector3.Distance(obj.transform.position, ObjectLocation.transform.position) < 30)
        {
            float distance = Vector3.Distance(obj.transform.position, ObjectLocation.transform.position);
            if(distance> 25  && obj)
            {
                Destroy(obj);
                yield break;
            }
            
            yield return null;
        }

    }


    /// <summary>
    /// ��ġ�� ������ �������� �ľ�.
    /// </summary>
    /// <param name="gridPosition">��ġ�� ��ġ</param>
    /// <param name="selectedObjectIndex">�̰� ���߿� ����� ���ڿ��� �ϴ� ����</param>
    /// <returns></returns>
    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        //GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : funitureData;
        GridData selectedData = funitureData;
        return selectedData.CanPlaceObjectAt(gridPosition, currentobjsize);
    }


    /// <summary>
    /// �ǽð����� ��ü �����̼� ���� �޾Ƽ� ȸ���� ������ �����ִ� �Լ�
    /// </summary>
    /// <returns></returns>
    public float RotateRealTimebyHand()
    {
        float y = CatchObject.transform.rotation.eulerAngles.y;
        float tempy = 0;
        
        y = Mathf.Abs(y);



        if (y % 360 >= 315 || y % 360 < 45)
        {
            y = 0;
            tempy = 180;
        }

        else if (y % 360 >= 45 && y % 360 < 135)
        {
            y = 90;
            tempy = 90;
        }

        else if (y % 360 >= 135 && y % 360 < 225)
        {
            y = 180;
            tempy = 0;
        }

        else if (y % 360 >= 225 && y % 360 < 315)
        {
            y = 270;
            tempy = 270;
        }


        cellIndicator.transform.rotation = Quaternion.Euler(new Vector3(90, 0, tempy));
        //cellIndicator.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, tempy)); 
        return y;
    }


    /// <summary>
    /// ���������� ��ü ��ġ�� ȸ�� ������ �°� �Ͽ�¡ ������ ȸ���ؼ� ��ġ���ִ� �Լ�
    /// </summary>
    /// <param name="zangle"></param>
    private void RotatePlacementByHand(float zangle)
    {
        float z = 0;

        if (zangle % 360 >= 315 || zangle % 360 < 45)
        {
            z = 0;
        }

        else if (zangle % 360 >= 45 && zangle % 360 < 135)
        {
            z = 90;
            int tempx = currentobjsize.x;
            currentobjsize.x = currentobjsize.y;
            currentobjsize.y = tempx;

        }

        else if (zangle % 360 >= 135 && zangle % 360 < 225)
            z = 180;

        else if (zangle % 360 >= 225 && zangle % 360 < 315)
        {
            z = 270;
            int tempx = currentobjsize.x;
            currentobjsize.x = currentobjsize.y;
            currentobjsize.y = tempx;

        }


        Vector3 euler = new Vector3(0, z, 0);
        //CatchObject.transform.rotation = Quaternion.Euler(euler);
        currentrotation = Quaternion.Euler(euler);

    }



    /// <summary>
    /// �� ������Ʈ ������Ű�� ��ġ�����ִ� �Լ�
    /// </summary>
    /// <param name="id">�ش� ������Ʈ�� id</param>
    /// <param name="parent">�ش� ������Ʈ�� �� ������ �θ�� �� ������Ʈ</param>
    /// <param name="loc">�ش� ������Ʈ�� ��ġ�� �׸��� ��ġ</param>
    /// <param name="rot">�ش� ������Ʈ�� ȸ��</param>
    /// <param name="size">�ش� ������Ʈ�� ���� ĭ ��</param>
    /// <param name="layer">�ش� ������Ʈ�� ��ġ�� ���̾�</param>
    /// <param name="newObject">��ġ�� ������Ʈ</param>
    /// <returns></returns>
    /// 
    private GameObject MakeNewObject(int id, Transform parent, Vector3Int loc, Quaternion rot, Vector2Int size, String layer, GameObject newObject)
    {
        if (id >= 0)
        {
            newObject.transform.SetParent(ObjectLocation.transform);
            newObject.transform.rotation = rot;

            newObject.transform.localPosition = PlacePosition(loc, size);    
            newObject.layer = LayerMask.NameToLayer(layer);

            placedGameObjects.Add(newObject);

            //GridData selectedData = database.objectsData[i].ID == 0 ? floorData : funitureData; 
            //Ȥ�� �ε��� �ʿ䰡 ���� ������ �����ϴ� ��� �� �ڵ带 Ȱ���ؾ���.

            GridData selectedData = funitureData;
            selectedData.AddObjectAt(loc, size, id, placedGameObjects.Count - 1);
            return newObject;
        }
        else return null;
    }



    /// <summary>
    /// ������ select�Ǵ� event���� �ʼ������� ������ �ϴ� SelectEnterEventArgs�� ���������ִ� �Լ�.
    /// </summary>
    /// <param name="interactable">������� ������Ʈ�� ���� ����</param>
    /// <param name="InteractorSelect">��� ������Ʈ�� ���� ����</param>
    /// <param name="interactionManager">interactionManagerŸ������ ��ӹ޾��� Ŭ������ ��� ����</param>
    /// <returns> �� ���� ������ �������� SelectEnterEventArgs��ü�� ����� ��ȯ��</returns>
    private SelectEnterEventArgs makeEnterEventArgs(XRGrabInteractable interactable, IXRSelectInteractor InteractorSelect, XRInteractionManager interactionManager)
    {
        SelectEnterEventArgs exiteventargs = new SelectEnterEventArgs();
        exiteventargs.interactableObject = interactable;
        exiteventargs.interactorObject = InteractorSelect;
        exiteventargs.manager = interactionManager;

        return exiteventargs;
    }

    /// <summary>
    /// select���� ����ԵǴ� event���� �ʼ������� ������ �ϴ� SelectExitEventArgs�� ���������ִ� �Լ�.
    /// </summary>
    /// <param name="interactable">������� ������Ʈ�� ���� ����</param>
    /// <param name="InteractorSelect">��� ������Ʈ�� ���� ����</param>
    /// <param name="interactionManager">interactionManagerŸ������ ��ӹ޾��� Ŭ������ ��� ����</param>
    /// <returns> �� ���� ������ �������� SelectExitEventArgs��ü�� ����� ��ȯ��</returns>
    private SelectExitEventArgs makeExitEventArgs(XRGrabInteractable interactable, IXRSelectInteractor InteractorSelect, XRInteractionManager interactionManager)
    {
        SelectExitEventArgs exiteventargs = new SelectExitEventArgs();
        exiteventargs.interactableObject = interactable;
        exiteventargs.interactorObject = InteractorSelect;
        exiteventargs.manager = interactionManager;

        return exiteventargs;
    }


    //�̺�Ʈ �Լ�

    /// <summary>
    /// ��ġ �غ� �̺�Ʈ
    /// </summary>
    /// <param name="p"></param>
    private void PlaceEnterEvent(SelectEnterEventArgs p)
    {
        CatchObject = p.interactableObject.transform.gameObject;
        PlaceStartStructure();
    }

    /// <summary>
    /// ��ġ �Ϸ� �̺�Ʈ
    /// </summary>
    /// <param name="p"></param>
    private void PlaceEvent(SelectExitEventArgs p)
    {
        PlaceStructure(p.interactableObject.transform.gameObject);
        MapInfo.Instance.ResetTileScale();
    }


    /// <summary>
    /// ��ġ ���� �غ� �̺�Ʈ
    /// </summary>
    /// <param name="p"></param>
    private void InsertEnterEvent(SelectEnterEventArgs p)
    {
        Startinsertion();
        InsertionStartStructure(p.interactableObject.transform.gameObject);
    }

    /// <summary>
    /// ��ġ ���� �Ϸ� �̺�Ʈ
    /// </summary>
    /// <param name="p"></param>
    private void InsertCompleteEvent(SelectExitEventArgs p)
    {
        RotatePlacementByHand(RotateRealTimebyHand());
        InsertionStructure(p.interactableObject.transform.gameObject);
        StopPlacement(false);
        MapInfo.Instance.ResetTileScale();
    }


    /// <summary>
    /// ������ �ƿ� �Ⱦ� �Լ����� �ƴѵ�, ���� AR ȯ��ȿ����� �Ⱦ� �Լ�����.Ȥ�� ���� deprecated��Ű��, �׳� �ŵ鶰 ���� �ʴ� �� ��õ 
    /// </summary>
    [Obsolete]
    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI()) return;

        //���� Ŀ�� ��ġ ������
        Vector3 mousePosition = inputManager.GetSelectedMapPositionInComputer();
        //���� �� �ٱ��̸� ��ġ�� �ʿ䰡 �����ϱ� �״�� ��ġ ���� ����
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
            StopPlacement(true);
        }
    }

    /// <summary>
    /// ������ �ƿ� �Ⱦ� �Լ����� �ƴѵ�, ���� AR ȯ��ȿ����� �Ⱦ� �Լ�����.Ȥ�� ���� deprecated��Ű��, �׳� �ŵ鶰 ���� �ʴ� �� ��õ 
    /// </summary>
    //��ġ�� ��ü�� ���� ��ġ���� �Ǵ� ����
    [Obsolete]
    public void InsertionStructure()
    {
        if (inputManager.IsPointerOverUI()) return;

        GameObject obj = cursorsystem.GetCollisionobject();
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
            inputManager.GetSelectedMapPositionInComputer();
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
                    if (funitureData.CanPlaceObjectAt(gridPosition, size))
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

    /// <summary>
    /// ������ �ƿ� �Ⱦ� �Լ����� �ƴѵ�, ���� AR ȯ��ȿ����� �Ⱦ� �Լ�����.Ȥ�� ���� deprecated��Ű��, �׳� �ŵ鶰 ���� �ʴ� �� ��õ 
    /// </summary>
    [Obsolete]
    private GameObject MakeNewObject(int id, Transform parent, Vector3Int loc, Quaternion rot, Vector2Int size, String layer)
    {
        if (id >= 0)
        {
            GameObject newObject = Instantiate(database.objectsData[id].Prefab);
            newObject.transform.SetParent(ObjectLocation.transform);
            newObject.transform.rotation = rot;
            //newObject.transform.position = grid.CellToWorld(loc);
            newObject.transform.position = ((Vector3)loc); //  / MapInfo.Instance.MapScale;
            newObject.transform.localScale = newObject.transform.localScale; // * (1/MapInfo.Instance.MapScale);
            newObject.layer = LayerMask.NameToLayer(layer);

            placedGameObjects.Add(newObject);
            //GridData selectedData = database.objectsData[i].ID == 0 ? floorData : funitureData;
            GridData selectedData = funitureData;
            selectedData.AddObjectAt(loc, size, id, placedGameObjects.Count - 1);
            return newObject;
        }
        else return null;
    }

    [Obsolete]
    /// <summary>
    ///������ �ƿ� �Ⱦ� �Լ����� �ƴѵ�, ���� AR ȯ��ȿ����� �Ⱦ� �Լ�����.Ȥ�� ���� deprecated��Ű��, �׳� �ŵ鶰 ���� �ʴ� �� ��õ 
    /// </summary>
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

    }

}
