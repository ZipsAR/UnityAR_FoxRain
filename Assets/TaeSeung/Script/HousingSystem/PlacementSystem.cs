using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class PlacementSystem : Singleton<PlacementSystem>
{

    [SerializeField]
    private GameObject _mouseIndicatorObj, _cellIndicatorObj;

    [SerializeField]
    private CursorCollisionSystem _cursorSystem;

    [SerializeField]
    private GameObject _spawnPointObj;

    [SerializeField]
    private InputManager _inputManager;

    [SerializeField]
    private Grid _grid;

    [SerializeField]
    private GameObject _gridVisualizationObj;

    [SerializeField]
    private GameObject _objectLocationObj;

    [SerializeField]
    public AnimationCurve AniCurve;


    public GameObject CreateObj;
    public GameObject InsertObj;
    public GameObject CatchObj;
    public SelectEnterEventArgs CreateObjSelectEventArgs;
    public SelectEnterEventArgs InsertObjSelectEventArgs;
    public List<XRGrabInteractable> l_objGrabInteractables;

    private GameObject _cursorOriginObj, _cursorParentObj;
    private Vector2Int _currentObjSize;
    private Quaternion _currentRotation;
    private Vector3Int _currentPos;
    private XRGrabInteractable _XR_grabInteractable;
    private bool _isCatchMode = false;
    private bool _isInitialize = true;
    private int _selectedObjIndex = -2;



    //실제 에셋 정보는 여기서 가져옴(스크립터블오브젝트 형태)
    [SerializeField]
    private ItemDatabase itemdatabase;

    public void InitializePlace()
    {
        MapInfo.Instance.MapInitialize();
        MapInfo.Instance.MapUnGrabmode();
        Dictionary<int, ObjectLocation> newobjectlocation = new();

        foreach(KeyValuePair<int, ObjectLocation> objectlocation in FileIOSystem.Instance.HousingDatabase.objectsLocation)
        {
            int id = objectlocation.Value.id;

            ItemData itemdata = itemdatabase.Itemdictionary[id];
            GameObject newitemObject = Instantiate(itemdata.Prefab);
            newitemObject.transform.localScale = newitemObject.transform.localScale / MapInfo.Instance.MapScale;

            Vector3Int loc = objectlocation.Value.location;
            Quaternion rot = objectlocation.Value.rotation;
            Vector2Int size = objectlocation.Value.size;
            objectlocation.Value.InstanceId = newitemObject.GetInstanceID();

            newobjectlocation.Add(newitemObject.GetInstanceID(), objectlocation.Value);
            
            MakeNewObject(id, _objectLocationObj.transform, loc, rot, size, "PlaceObject", newitemObject);
            
            _XR_grabInteractable = newitemObject.GetComponent<XRGrabInteractable>();
            l_objGrabInteractables.Add(_XR_grabInteractable);
            SelectEnterEventArgs enterArgs = makeEnterEventArgs(_XR_grabInteractable, _XR_grabInteractable.firstInteractorSelecting, _XR_grabInteractable.interactionManager);
            SelectExitEventArgs exitargs = makeExitEventArgs(_XR_grabInteractable, _XR_grabInteractable.firstInteractorSelecting, _XR_grabInteractable.interactionManager);
            _XR_grabInteractable.selectEntered.AddListener((a) => InsertEnterEvent(enterArgs));
            _XR_grabInteractable.selectExited.AddListener((a) => InsertCompleteEvent(exitargs));
        }

        FileIOSystem.Instance.HousingDatabase.objectsLocation = newobjectlocation;
        StopPlacement(true);
        _cursorOriginObj = _mouseIndicatorObj;
        _cursorParentObj = _cursorOriginObj.transform.parent.gameObject;


    }


    private void Update()
    {
       if (!_mouseIndicatorObj)
            return;

       if (_isInitialize)
        {
            InitializePlace();
            MapInfo.Instance.SetInvisiblemode();
            _isInitialize = false;
        }


       //if catch some obj
        if (_isCatchMode)
        {
            PlaceCheck(CatchObj);
            RotateRealTimebyHand();
            if (!_inputManager.IsHit()) _cellIndicatorObj.SetActive(false);
            else _cellIndicatorObj.SetActive(true);
        }

    }


    public void SetCatchmode(bool mode)
    {
        _isCatchMode = mode;
    }

    public void StartPlacement(int ID)
    {
        if (MapInfo.Instance.l_IsHousingTutorialFinish[1])
        {
            InteractEventManager.NotifyClearDialog();
            InteractEventManager.NotifyDialogShow("하우징 맵 가운데에 있는 가구를 핀치하여\n가구를 집으세요! ");
        }
        if (CreateObj)
        {
            Destroy(CreateObj);
            CreateObj = null;
        }
        StopPlacement(true);
        _selectedObjIndex = FileIOSystem.Instance.InvenDatabase.mydata.FindIndex(data => data.id == ID);
        ItemData item = itemdatabase.Itemdictionary[ID];

        if (_selectedObjIndex < 0)
        {
            Debug.LogError($"No ID found {ID}");
            return;
        }

        if (FileIOSystem.Instance.InvenDatabase.mydata[_selectedObjIndex].count <= 0)
        {
            Debug.Log($"No Object");
            return;
        }

        _currentObjSize = item.Housingsize;
        MapInfo.Instance.SetTileScale(new Vector3(_currentObjSize.x, _currentObjSize.y, 1));
        _currentRotation = new();

        GameObject newObject = Instantiate(item.Prefab, _spawnPointObj.transform.parent);
        CreateObj = newObject;
        newObject.transform.position = _spawnPointObj.transform.position;
        newObject.transform.localScale = newObject.transform.localScale * (1 / MapInfo.Instance.MapScale);

        //이펙트 및 사운드 효과 파트
        EffectSystem.Instance.playspawneffect(_spawnPointObj.transform.position);


        //grip관련 이벤트 추가
        _XR_grabInteractable = newObject.GetComponent<XRGrabInteractable>();
        SelectEnterEventArgs enterargs = makeEnterEventArgs(_XR_grabInteractable, _XR_grabInteractable.firstInteractorSelecting, _XR_grabInteractable.interactionManager);
        _XR_grabInteractable.selectEntered.AddListener((a) => PlaceEnterEvent(enterargs));

        SelectExitEventArgs exitargs = makeExitEventArgs(_XR_grabInteractable, _XR_grabInteractable.firstInteractorSelecting, _XR_grabInteractable.interactionManager);
        _XR_grabInteractable.selectExited.AddListener((a)=>PlaceEvent(exitargs));
    }


    public void ProtectGrib()
    {
        foreach(XRGrabInteractable obj in l_objGrabInteractables) {
            obj.enabled = false;
        }
    }


    public void ReleaseGrib() {
        foreach (XRGrabInteractable obj in l_objGrabInteractables)
        {
            obj.enabled = true;
        }

    } 



    public void Startinsertion()
    {
        StopPlacement(false);
        _gridVisualizationObj.SetActive(true);
    }

    public void StopPlacement(bool indexinitialize)
    {
        if(indexinitialize)
            _selectedObjIndex = -1;


        _gridVisualizationObj.SetActive(false);
        _cellIndicatorObj.SetActive(false);
        _isCatchMode = false;
    }

    private void PlaceStartStructure()
    {
        if (MapInfo.Instance.l_IsHousingTutorialFinish[1])
        {
            InteractEventManager.NotifyClearDialog();
            InteractEventManager.NotifyDialogShow("이제 가구를 원하는 위치에 두세요!\n 핀치를 풀면 가구가 배치된답니다!");
        }
        if (!_gridVisualizationObj.activeSelf)
            _gridVisualizationObj.SetActive(true);
        if (!_cellIndicatorObj.activeSelf)
            _cellIndicatorObj.SetActive(true);

        _isCatchMode = true;
    }

    private void PlaceCheck(GameObject catchobject)
    {
        Vector3 mousePosition = _inputManager.GetSelectedMapPositionbyObject(catchobject.transform);
        Vector3Int gridPosition = _grid.WorldToCell(mousePosition);

        bool placementValidity = _cursorSystem.Iscollision();
        _mouseIndicatorObj.transform.position = mousePosition;
        _cellIndicatorObj.transform.localPosition = PlacePosition(gridPosition, _currentObjSize);

    }

    private Vector3 PlacePosition(Vector3Int gridposition, Vector2Int size)
    {
        Vector3 Cellposition = gridposition;
        float z = Math.Abs(_cellIndicatorObj.transform.eulerAngles.y);
        int sizex = size.x;
        int sizey = size.y;

        if ((z == 90 || z == 270) && (sizex+sizey) % 2 == 1)
        {
            int temp = sizey;
            sizey = sizex;
            sizex = temp;

        }

        if (sizex%2 == 1)
        Cellposition.x = Cellposition.x + 0.5f;
        if(sizey%2 == 1)
        Cellposition.z = Cellposition.z + 0.5f;

        Cellposition.y = 0.1f;
        return Cellposition;
    }
    
    private void PlaceStructure(GameObject gameObject)
    {
        InteractEventManager.NotifyClearDialog();
        Vector3 mousePosition = _inputManager.GetSelectedMapPositionbyObject(gameObject.transform);
        if (!_inputManager.IsHit())
        {
            if (MapInfo.Instance.l_IsHousingTutorialFinish[1]) InteractEventManager.NotifyDialogShow("이런! 올바르지 않은 위치에 배치하시면\n 처음 위치로 돌아간답니다!");
            gameObject.transform.position = _spawnPointObj.transform.position;
            gameObject.transform.rotation = _spawnPointObj.transform.rotation;
            EffectSystem.Instance.playspawneffect(_spawnPointObj.transform.position);
            StopPlacement(false);
            return;
        }

        Vector3Int gridPosition = _grid.WorldToCell(mousePosition);
        ObjectLocation newlocation = new ObjectLocation();
        bool placementValidity = _cursorSystem.Iscollision();
        if (placementValidity == false)
        {
            if (MapInfo.Instance.l_IsHousingTutorialFinish[1]) InteractEventManager.NotifyDialogShow("이런! 이미 가구가 있는 위치에 배치하시면\n 처음 위치로 돌아간답니다!");

            gameObject.transform.position = _spawnPointObj.transform.position;
            gameObject.transform.rotation = _spawnPointObj.transform.rotation;
            EffectSystem.Instance.playspawneffect(_spawnPointObj.transform.position);
            StopPlacement(false);
            return;
        }


        if (MapInfo.Instance.l_IsHousingTutorialFinish[1])
        {
            InteractEventManager.NotifyDialogShow("좋아요! 가구 배치에 성공했어요! \n이번엔 배치된 가구를 집어볼까요(핀치)?");
            MapInfo.Instance.l_IsHousingTutorialFinish[1] = false;
        }
        int id = FileIOSystem.Instance.InvenDatabase.mydata[_selectedObjIndex].id;
        newlocation.location = gridPosition;
        newlocation.rotation = _currentRotation;
        newlocation.id = id;
        newlocation.size = _currentObjSize;
        newlocation.InstanceId = gameObject.GetInstanceID();
        newlocation.placementstatus = true;

        float k = RotateRealTimebyHand();
        RotatePlacementByHand(k);

        MakeNewObject(_selectedObjIndex, _objectLocationObj.transform, gridPosition, _currentRotation, newlocation.size, "PlaceObject",gameObject);
        gameObject.transform.localPosition = _cellIndicatorObj.transform.localPosition;
        EffectSystem.Instance.playplaceeffect(_cellIndicatorObj.transform.position);
        SoundSystem.Instance.PlayAudio(_cellIndicatorObj.transform.position);

        FileIOSystem.Instance.HousingDatabase.objectsLocation.Add(newlocation.InstanceId,newlocation);
        FileIOSystem.Instance.InvenDatabase.mydata[_selectedObjIndex].count -= 1;

        _XR_grabInteractable = gameObject.GetComponent<XRGrabInteractable>();
        l_objGrabInteractables.Add(_XR_grabInteractable);
        SelectEnterEventArgs enterArgs = makeEnterEventArgs(_XR_grabInteractable, _XR_grabInteractable.firstInteractorSelecting, _XR_grabInteractable.interactionManager);
        SelectExitEventArgs exitargs = makeExitEventArgs(_XR_grabInteractable, _XR_grabInteractable.firstInteractorSelecting, _XR_grabInteractable.interactionManager);
        _XR_grabInteractable.selectEntered.RemoveAllListeners();
        _XR_grabInteractable.selectExited.RemoveAllListeners();
        _XR_grabInteractable.selectEntered.AddListener((a) => InsertEnterEvent(enterArgs));
        _XR_grabInteractable.selectExited.AddListener((a) => InsertCompleteEvent(exitargs));


        if (FileIOSystem.Instance.InvenDatabase.mydata[_selectedObjIndex].count <= 0)
        {
            Debug.Log($"No Object");
            HousingUISystem.Instance.d_Count_Object[id].GetComponent<Button>().interactable = false;
        }
        HousingUISystem.Instance.ObjCountupdate(id ,_selectedObjIndex);
        FileIOSystem.Instance.Save(FileIOSystem.Instance.HousingDatabase,FileIOSystem.HousingFileName);
        FileIOSystem.Instance.Save(FileIOSystem.Instance.InvenDatabase, FileIOSystem.InvenFileName);

        CreateObj = null;
        StopPlacement(true);
    }


    public void InsertionStartStructure(GameObject gameObject)
    {
        if (MapInfo.Instance.l_IsHousingTutorialFinish[2])
        {
            InteractEventManager.NotifyClearDialog();
            InteractEventManager.NotifyDialogShow("가구를 바깥으로 내보내면 삭제,\n다른 위치에 배치하면 위치를 수정할 수 있어요!");
        }
        ObjectLocation currentlocationinfo = FileIOSystem.Instance.HousingDatabase.objectsLocation[gameObject.GetInstanceID()];
        int currentgrabid = currentlocationinfo.id;
        if (currentgrabid < 0) return;

        //오브젝트 사이즈에 맞게 아래 타일 크기를 수정해요

        ItemData currentItemdata = itemdatabase.Itemdictionary[currentgrabid];
        _currentObjSize = currentItemdata.Housingsize;
        _currentPos = currentlocationinfo.location;
        MapInfo.Instance.SetTileScale(new Vector3(_currentObjSize.x, _currentObjSize.y, 1));

        _cellIndicatorObj.SetActive(true);
        _isCatchMode = true;
    }


    public void ResetCatchObjectPosition()
    {
        CreateObj.transform.position = _spawnPointObj.transform.position;
        CreateObj.transform.rotation = _spawnPointObj.transform.rotation;
        EffectSystem.Instance.playspawneffect(_spawnPointObj.transform.position);
        StopPlacement(false);
    }

    private void InsertionStructure(GameObject gameObject) {

        Vector3 mousePosition = _inputManager.GetSelectedMapPositionbyObject(gameObject.transform);
        Vector3Int gridPosition = _grid.WorldToCell(mousePosition);

        //배치 가능한 영역이 아닌 경우
            if (!_inputManager.IsHit())
            {
                _cursorOriginObj.transform.SetParent(_cursorParentObj.transform);
                int index = FileIOSystem.Instance.HousingDatabase.objectsLocation[gameObject.GetInstanceID()].id;

                if (index >= 0)
                {
                    Vector2Int size = FileIOSystem.Instance.HousingDatabase.objectsLocation[gameObject.GetInstanceID()].size;
                    int myindex= FileIOSystem.Instance.InvenDatabase.mydata.FindIndex(data => data.id == index);
                    int grapindex = l_objGrabInteractables.FindIndex(data => data == gameObject.GetComponent<XRGrabInteractable>());
                    l_objGrabInteractables.RemoveAt(grapindex);
                    
                    if (myindex != -1)
                    {
                        FileIOSystem.Instance.InvenDatabase.mydata[myindex].count++;
                        FileIOSystem.Instance.HousingDatabase.objectsLocation.Remove(gameObject.GetInstanceID());
                        
                        if(HousingUISystem.Instance != null) HousingUISystem.Instance.ObjCountupdate(index, myindex);

                    _isCatchMode = false;
                    _cellIndicatorObj.SetActive(false);
                    if (gameObject.GetComponent<Rigidbody>())
                    {
                        gameObject.GetComponent<Rigidbody>().useGravity = true;
                        gameObject.GetComponent<Rigidbody>().isKinematic = false;
                    }

                    InsertObj = null;
                    StartCoroutine(throwdelete(gameObject));
                    StartCoroutine(throwdelete2(gameObject));

                    if (MapInfo.Instance.l_IsHousingTutorialFinish[2])
                    {
                        InteractEventManager.NotifyClearDialog();
                        InteractEventManager.NotifyDialogShow("좋아요! 가구를 삭제했어요!\n 이번엔 가구를 생성하고 위치를 수정해볼까요?");
                    }

                    FileIOSystem.Instance.Save(FileIOSystem.Instance.InvenDatabase, FileIOSystem.InvenFileName);
                    FileIOSystem.Instance.Save(FileIOSystem.Instance.HousingDatabase, FileIOSystem.HousingFileName);

                }
       
            }
            }

            //배치 가능한 영역인 경우
            else
            {

                ObjectLocation CurrentObjectlocation = FileIOSystem.Instance.HousingDatabase.objectsLocation[gameObject.GetInstanceID()];
                int index = FileIOSystem.Instance.HousingDatabase.objectsLocation[gameObject.GetInstanceID()].id;

                if (index >= 0)
                {
                    int id = CurrentObjectlocation.id;
                    Vector3Int pos = CurrentObjectlocation.location;
                    Quaternion rot = CurrentObjectlocation.rotation;
                    Vector2Int size = CurrentObjectlocation.size;
                    

                    if (_cursorSystem.Iscollision())
                    {
                        CurrentObjectlocation.location = gridPosition;
                        CurrentObjectlocation.rotation = _currentRotation;
                        CurrentObjectlocation.size = _currentObjSize;
                        gameObject.transform.rotation = _currentRotation;
                        gameObject.transform.localPosition = _cellIndicatorObj.transform.localPosition;

                        EffectSystem.Instance.playplaceeffect(_cellIndicatorObj.transform.position);
                        SoundSystem.Instance.PlayAudio(_cellIndicatorObj.transform.position);

                        FileIOSystem.Instance.Save(FileIOSystem.Instance.HousingDatabase , FileIOSystem.HousingFileName);

                    if (MapInfo.Instance.l_IsHousingTutorialFinish[2])
                    {
                        InteractEventManager.NotifyClearDialog();
                        InteractEventManager.NotifyDialogShow("좋아요! 가구위치를 수정했어요!!");
                        StartCoroutine(EndTutorial());
                        MapInfo.Instance.l_IsHousingTutorialFinish[2] = false;
                    }
                }

                    else
                    {
                        Vector2Int tempsize = _currentObjSize;
                        tempsize.x = size.y;
                        tempsize.y = size.x;

                        gameObject.transform.localRotation = rot;
                        gameObject.transform.localPosition = PlacePosition(_currentPos, size);

                    EffectSystem.Instance.playplaceeffect(_cellIndicatorObj.transform.position);

                    if (MapInfo.Instance.l_IsHousingTutorialFinish[2])
                    {
                        InteractEventManager.NotifyClearDialog();
                        InteractEventManager.NotifyDialogShow("이런, 올바르지 않은 위치에 배치했네요...!");
                    }

                }
                    _isCatchMode = false;
                    _cellIndicatorObj.SetActive(false);
                 }
            InsertObj.GetComponent<XRGrabInteractable>().throwOnDetach = false;
            //InsertObj.GetComponent<XRGrabInteractable>().throwSmoothingCurve = AniCurve;
            InsertObj = null;
            }
    }


    private IEnumerator throwdelete(GameObject obj)
    {
         while (Vector3.Distance(obj.transform.position, _objectLocationObj.transform.position) < 30)
        {
            float distance = Vector3.Distance(obj.transform.position, _objectLocationObj.transform.position);
            if(distance> 25  && obj!=null)
            {
                
                Destroy(obj);
                yield break;
            }
            if (obj)
                yield break;
            
            yield return null;
            }
        }
    private IEnumerator throwdelete2(GameObject obj)
    {
        float nowtime = Time.time;
        float latertime = Time.time;
        while (Mathf.Abs(nowtime - latertime) <= 3)
        {
            latertime = Time.time;
            yield return null;
        }
        if (obj != null)  Destroy(obj);
    }


    private IEnumerator EndTutorial()
    {
        float nowtime = Time.time;
        float latertime = Time.time;
        while (Mathf.Abs(nowtime-latertime) <= 3)
        {
            latertime = Time.time;
            yield return null;
        }

        InteractEventManager.NotifyClearDialog();
        InteractEventManager.NotifyDialogShow("튜토리얼 끝! \n이제 마음껏 펫 하우스를 꾸며봅시다!");
     }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public float RotateRealTimebyHand()
    {
        float y = CatchObj.transform.rotation.eulerAngles.y;
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


        _cellIndicatorObj.transform.rotation = Quaternion.Euler(new Vector3(90, 0, tempy));
        return y;
    }


    /// <summary>
    /// 
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
            int tempx = _currentObjSize.x;
            _currentObjSize.x = _currentObjSize.y;
            _currentObjSize.y = tempx;

        }

        else if (zangle % 360 >= 135 && zangle % 360 < 225)
            z = 180;

        else if (zangle % 360 >= 225 && zangle % 360 < 315)
        {
            z = 270;
            int tempx = _currentObjSize.x;
            _currentObjSize.x = _currentObjSize.y;
            _currentObjSize.y = tempx;

        }


        Vector3 euler = new Vector3(0, z, 0);
        _currentRotation = Quaternion.Euler(euler);

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
            newObject.transform.SetParent(_objectLocationObj.transform);
            newObject.transform.localRotation = rot;

            newObject.transform.localPosition = PlacePosition(loc, size);    
            newObject.layer = LayerMask.NameToLayer(layer);

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



    /// <summary>
    /// Place Start Event
    /// </summary>
    /// <param name="p"></param>
    private void PlaceEnterEvent(SelectEnterEventArgs p)
    {
        CreateObj = p.interactableObject.transform.gameObject;
        CatchObj = CreateObj;
        CreateObjSelectEventArgs = p;

        HousingUISystem.Instance.EnableButton(false);
        PlaceStartStructure();
    }

    /// <summary>
    /// Place Complete Event
    /// </summary>
    /// <param name="p"></param>
    private void PlaceEvent(SelectExitEventArgs p)
    {
        if (CreateObj != null)
        {
            PlaceStructure(p.interactableObject.transform.gameObject);
            if (HousingUISystem.Instance != null) HousingUISystem.Instance.EnableButton(true);
            MapInfo.Instance.ResetTileScale();
        }
        _cursorSystem.ColorCursorsetting(new Vector4(1, 1, 1, 1), new Vector4(0, 1, 0, 0.5f));
    }


    /// <summary>
    /// Insertion Start Event
    /// </summary>
    /// <param name="p"></param>
    private void InsertEnterEvent(SelectEnterEventArgs p)
    {
        InsertObj = p.interactableObject.transform.gameObject;
        CatchObj = InsertObj;
        InsertObjSelectEventArgs = p;
        InsertObj.GetComponent<XRGrabInteractable>().throwOnDetach = true;
        InsertObj.GetComponent<XRGrabInteractable>().throwSmoothingCurve = AniCurve;
        Startinsertion();
        HousingUISystem.Instance.EnableButton(false);
        InsertionStartStructure(p.interactableObject.transform.gameObject);

        
    }

    /// <summary>
    /// Insertion End Event
    /// </summary>
    /// <param name="p"></param>
    private void InsertCompleteEvent(SelectExitEventArgs p)
    {
            RotatePlacementByHand(RotateRealTimebyHand());
            InsertionStructure(p.interactableObject.transform.gameObject);
            StopPlacement(false);
            if(HousingUISystem.Instance != null) HousingUISystem.Instance.EnableButton(true);
            MapInfo.Instance.ResetTileScale();
            _cursorSystem.ColorCursorsetting(new Vector4(1, 1, 1, 1), new Vector4(0, 1, 0, 0.5f));
    }
}






