using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class PlacementSystem : Singleton<PlacementSystem>
{

    [SerializeField]
    GameObject mouseIndicator, cellIndicator;

    [SerializeField]
    CursorCollisionSystem Cursorsystem;

    [SerializeField]
    private GameObject spawnpoint;

    [SerializeField]
    private InputManager inputManager;

    [SerializeField]
    private Grid grid;
    private int selectedObjectIndex = -2;

    [SerializeField]
    private GameObject gridVisualization;

    [SerializeField]
    private GameObject ObjectLocation;

    //private GridData floorData, funitureData;
    //private List<GameObject> placedGameObjects = new();


    private bool Initilaize = true;

    public GameObject CreateObject;
    public GameObject InsertObject;
    public GameObject CatchObject;
    public SelectEnterEventArgs CreateManager;
    public SelectEnterEventArgs InsertManager;
    private Vector2Int currentobjsize;  
    private Quaternion currentrotation; 
    private Vector3Int currentpos;  
    private bool catchmode = false;
    private XRGrabInteractable interact;

    public List<XRGrabInteractable> ObjGrabcomponents;


    private GameObject cursororigin, cursorparent;

    [SerializeField]
    public AnimationCurve curve;

    //실제 에셋 정보는 여기서 가져옴(스크립터블오브젝트 형태)
    [SerializeField]
    private ItemDatabase itemdatabase;

    public void InitializePlace()
    {
        MapInfo.Instance.MapInitialize();
        MapInfo.Instance.MapUnGrabmode();

        for (short i = 0; i < FileIOSystem.Instance.housingdatabase.objectsLocation.Count; i++) {
            //id는 키값
            int id = FileIOSystem.Instance.housingdatabase.objectsLocation[i].id;
            int dataindex = itemdatabase.ItemData.FindIndex(data => data.ID == id);

            GameObject newObject = Instantiate(itemdatabase.ItemData[dataindex].Prefab);
            newObject.transform.localScale = newObject.transform.localScale / MapInfo.Instance.MapScale;

            Vector3Int loc = FileIOSystem.Instance.housingdatabase.objectsLocation[i].location;
            Quaternion rot = FileIOSystem.Instance.housingdatabase.objectsLocation[i].rotation;
            Vector2Int size = FileIOSystem.Instance.housingdatabase.objectsLocation[i].size;
            FileIOSystem.Instance.housingdatabase.objectsLocation[i].InstanceId = newObject.GetInstanceID();

            MakeNewObject(id, ObjectLocation.transform, loc, rot, size, "PlaceObject", newObject);

            interact = newObject.GetComponent<XRGrabInteractable>();
            ObjGrabcomponents.Add(interact);
            SelectEnterEventArgs enterArgs = makeEnterEventArgs(interact, interact.firstInteractorSelecting, interact.interactionManager);
            SelectExitEventArgs exitargs = makeExitEventArgs(interact, interact.firstInteractorSelecting, interact.interactionManager);
            interact.selectEntered.AddListener((a) => InsertEnterEvent(enterArgs));
            interact.selectExited.AddListener((a) => InsertCompleteEvent(exitargs));
        }
        StopPlacement(true);

        cursororigin = mouseIndicator;
        cursorparent = cursororigin.transform.parent.gameObject;

        //
    }


    private void Update()
    {
       if (!mouseIndicator)
            return;

       if (Initilaize)
        {
            InitializePlace();
            MapInfo.Instance.SetInvisiblemode();
            Initilaize = false;
        }


       //if catch some obj
        if (catchmode)
        {
            PlaceCheck(CatchObject);
            RotateRealTimebyHand();
            if (!inputManager.ishit()) cellIndicator.SetActive(false);
            else cellIndicator.SetActive(true);
        }

    }


    public void SetCatchmode(bool mode)
    {
        catchmode = mode;
    }

    public void StartPlacement(int ID)
    {
        if (MapInfo.Instance.housingtutorial[1])
        {
            InteractEventManager.NotifyClearDialog();
            InteractEventManager.NotifyDialogShow("하우징 맵 가운데에 있는 가구를 핀치하여\n가구를 집으세요! ");
        }
        if (CreateObject)
        {
            Destroy(CreateObject);
            CreateObject = null;
        }
        StopPlacement(true);
        selectedObjectIndex = FileIOSystem.Instance.invendatabase.mydata.FindIndex(data => data.id == ID);
        int infoindex = itemdatabase.ItemData.FindIndex(data => data.ID == ID);

        if (selectedObjectIndex < 0)
        {
            Debug.LogError($"No ID found {ID}");
            return;
        }

        if (FileIOSystem.Instance.invendatabase.mydata[selectedObjectIndex].count <= 0)
        {
            Debug.Log($"No Object");
            return;
        }

        currentobjsize = itemdatabase.ItemData[infoindex].Housingsize;
        MapInfo.Instance.SetTileScale(new Vector3(currentobjsize.x, currentobjsize.y, 1));
        currentrotation = new();

        GameObject newObject = Instantiate(itemdatabase.ItemData[infoindex].Prefab, spawnpoint.transform.parent);
        CreateObject = newObject;
        newObject.transform.position = spawnpoint.transform.position;
        newObject.transform.localScale = newObject.transform.localScale * (1 / MapInfo.Instance.MapScale);

        //이펙트 및 사운드 효과 파트
        EffectSystem.Instance.playspawneffect(spawnpoint.transform.position);


        //grip관련 이벤트 추가
        interact = newObject.GetComponent<XRGrabInteractable>();
        SelectEnterEventArgs enterargs = makeEnterEventArgs(interact, interact.firstInteractorSelecting, interact.interactionManager);
        interact.selectEntered.AddListener((a) => PlaceEnterEvent(enterargs));

        SelectExitEventArgs exitargs = makeExitEventArgs(interact, interact.firstInteractorSelecting, interact.interactionManager);
        interact.selectExited.AddListener((a)=>PlaceEvent(exitargs));
    }


    public void ProtectGrib()
    {
        foreach(XRGrabInteractable obj in ObjGrabcomponents) {
            obj.enabled = false;
        }
    }


    public void ReleaseGrib() {
        foreach (XRGrabInteractable obj in ObjGrabcomponents)
        {
            obj.enabled = true;
        }

    } 



    public void Startinsertion()
    {
        StopPlacement(false);
        gridVisualization.SetActive(true);
    }

    public void StopPlacement(bool indexinitialize)
    {
        if(indexinitialize)
            selectedObjectIndex = -1;


        gridVisualization.SetActive(false);
        cellIndicator.SetActive(false);
        catchmode = false;
    }

    private void PlaceStartStructure()
    {
        if (MapInfo.Instance.housingtutorial[1])
        {
            InteractEventManager.NotifyClearDialog();
            InteractEventManager.NotifyDialogShow("이제 가구를 원하는 위치에 두세요!\n 핀치를 풀면 가구가 배치된답니다!");
        }
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

        bool placementValidity = Cursorsystem.Iscollision();
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
        Vector3 mousePosition = inputManager.GetSelectedMapPositionbyObject(gameObject.transform);
        if (!inputManager.ishit())
        {
            if (MapInfo.Instance.housingtutorial[1]) InteractEventManager.NotifyDialogShow("이런! 올바르지 않은 위치에 배치하시면\n 처음 위치로 돌아간답니다!");
            gameObject.transform.position = spawnpoint.transform.position;
            gameObject.transform.rotation = spawnpoint.transform.rotation;
            EffectSystem.Instance.playspawneffect(spawnpoint.transform.position);
            StopPlacement(false);
            return;
        }

        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        ObjectLocation newlocation = new ObjectLocation();
        bool placementValidity = Cursorsystem.Iscollision();
        if (placementValidity == false)
        {
            if (MapInfo.Instance.housingtutorial[1]) InteractEventManager.NotifyDialogShow("이런! 이미 가구가 있는 위치에 배치하시면\n 처음 위치로 돌아간답니다!");

            gameObject.transform.position = spawnpoint.transform.position;
            gameObject.transform.rotation = spawnpoint.transform.rotation;
            EffectSystem.Instance.playspawneffect(spawnpoint.transform.position);
            StopPlacement(false);
            return;
        }


        if (MapInfo.Instance.housingtutorial[1])
        {
            InteractEventManager.NotifyDialogShow("좋아요! 가구 배치에 성공했어요! \n이번엔 배치된 가구를 집어볼까요(핀치)?");
            MapInfo.Instance.housingtutorial[1] = false;
        }
        int id = FileIOSystem.Instance.invendatabase.mydata[selectedObjectIndex].id;
        newlocation.location = gridPosition;
        newlocation.rotation = currentrotation;
        newlocation.id = id;
        newlocation.size = currentobjsize;
        newlocation.InstanceId = gameObject.GetInstanceID();
        newlocation.placementstatus = true;

        float k = RotateRealTimebyHand();
        RotatePlacementByHand(k);

        MakeNewObject(selectedObjectIndex, ObjectLocation.transform, gridPosition, currentrotation, newlocation.size, "PlaceObject",gameObject);
        gameObject.transform.localPosition = cellIndicator.transform.localPosition;
        EffectSystem.Instance.playplaceeffect(cellIndicator.transform.position);
        SoundSystem.Instance.PlayAudio(cellIndicator.transform.position);

        FileIOSystem.Instance.housingdatabase.objectsLocation.Add(newlocation);
        FileIOSystem.Instance.invendatabase.mydata[selectedObjectIndex].count -= 1;

        interact = gameObject.GetComponent<XRGrabInteractable>();
        ObjGrabcomponents.Add(interact);
        SelectEnterEventArgs enterArgs = makeEnterEventArgs(interact, interact.firstInteractorSelecting, interact.interactionManager);
        SelectExitEventArgs exitargs = makeExitEventArgs(interact, interact.firstInteractorSelecting, interact.interactionManager);
        interact.selectEntered.RemoveAllListeners();
        interact.selectExited.RemoveAllListeners();
        interact.selectEntered.AddListener((a) => InsertEnterEvent(enterArgs));
        interact.selectExited.AddListener((a) => InsertCompleteEvent(exitargs));


        if (FileIOSystem.Instance.invendatabase.mydata[selectedObjectIndex].count <= 0)
        {
            Debug.Log($"No Object");
            HousingUISystem.Instance.countlist[id].GetComponent<Button>().interactable = false;
        }
        HousingUISystem.Instance.ObjCountupdate(id ,selectedObjectIndex);

        FileIOSystem.Instance.Save(FileIOSystem.Instance.housingdatabase,FileIOSystem.HousingFilename);
        FileIOSystem.Instance.Save(FileIOSystem.Instance.invendatabase, FileIOSystem.InvenFilename);

        CreateObject = null;
        StopPlacement(true);
    }


    public void InsertionStartStructure(GameObject gameObject)
    {
        if (MapInfo.Instance.housingtutorial[2])
        {
            InteractEventManager.NotifyClearDialog();
            InteractEventManager.NotifyDialogShow("가구를 바깥으로 내보내면 삭제,\n다른 위치에 배치하면 위치를 수정할 수 있어요!");
        }
        int index = FileIOSystem.Instance.housingdatabase.objectsLocation.FindIndex(data => data.InstanceId == gameObject.GetInstanceID());
        if (index < 0) return;

        //오브젝트 사이즈에 맞게 아래 타일 크기를 수정해요

        int dataindex = itemdatabase.ItemData.FindIndex(data => data.ID == FileIOSystem.Instance.housingdatabase.objectsLocation[index].id);
        currentobjsize = itemdatabase.ItemData[dataindex].Housingsize;
        currentpos = FileIOSystem.Instance.housingdatabase.objectsLocation[index].location;
        MapInfo.Instance.SetTileScale(new Vector3(currentobjsize.x, currentobjsize.y, 1));

        cellIndicator.SetActive(true);
        catchmode = true;
    }


    public void ResetCatchObjectPosition()
    {
        CreateObject.transform.position = spawnpoint.transform.position;
        CreateObject.transform.rotation = spawnpoint.transform.rotation;
        EffectSystem.Instance.playspawneffect(spawnpoint.transform.position);
        StopPlacement(false);
    }

    private void InsertionStructure(GameObject gameObject) {

        Vector3 mousePosition = inputManager.GetSelectedMapPositionbyObject(gameObject.transform);
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

            if (!inputManager.ishit())
            {
                cursororigin.transform.SetParent(cursorparent.transform);
                int index = FileIOSystem.Instance.housingdatabase.objectsLocation.FindIndex(data => data.location == currentpos);
            
            if (index >= 0)
                {
                    int id = FileIOSystem.Instance.housingdatabase.objectsLocation[index].id;
                    Vector2Int size = FileIOSystem.Instance.housingdatabase.objectsLocation[index].size;
                    int myindex= FileIOSystem.Instance.invendatabase.mydata.FindIndex(data => data.id == id);
                    int grapindex = ObjGrabcomponents.FindIndex(data => data == gameObject.GetComponent<XRGrabInteractable>());
                ObjGrabcomponents.RemoveAt(grapindex);
                    
                    if (myindex != -1)
                    {
                        FileIOSystem.Instance.invendatabase.mydata[myindex].count++;
                        FileIOSystem.Instance.housingdatabase.objectsLocation.RemoveAt(index);
                        if(HousingUISystem.Instance != null) HousingUISystem.Instance.ObjCountupdate(id, myindex);

                    catchmode = false;
                    cellIndicator.SetActive(false);
                    if (gameObject.GetComponent<Rigidbody>())
                    {
                        gameObject.GetComponent<Rigidbody>().useGravity = true;
                        gameObject.GetComponent<Rigidbody>().isKinematic = false;
                    }

                    InsertObject = null;
                    StartCoroutine(throwdelete(gameObject));
                    StartCoroutine(throwdelete2(gameObject));

                    if (MapInfo.Instance.housingtutorial[2])
                    {
                        InteractEventManager.NotifyClearDialog();
                        InteractEventManager.NotifyDialogShow("좋아요! 가구를 삭제했어요!\n 이번엔 가구를 생성하고 위치를 수정해볼까요?");
                    }

                    FileIOSystem.Instance.Save(FileIOSystem.Instance.invendatabase, FileIOSystem.InvenFilename);
                    FileIOSystem.Instance.Save(FileIOSystem.Instance.housingdatabase, FileIOSystem.HousingFilename);

                }
       
            }
            }

            else
            {
                int index = FileIOSystem.Instance.housingdatabase.objectsLocation.FindIndex(data => data.InstanceId == gameObject.GetInstanceID());

                if (index >= 0)
                {
                    int id = FileIOSystem.Instance.housingdatabase.objectsLocation[index].id;
                    Vector3Int pos = FileIOSystem.Instance.housingdatabase.objectsLocation[index].location;
                    Quaternion rot = FileIOSystem.Instance.housingdatabase.objectsLocation[index].rotation;
                    Vector2Int size = FileIOSystem.Instance.housingdatabase.objectsLocation[index].size;
                    

                    if (Cursorsystem.Iscollision())
                    {
                        FileIOSystem.Instance.housingdatabase.objectsLocation[index].location = gridPosition;
                        FileIOSystem.Instance.housingdatabase.objectsLocation[index].rotation = currentrotation;
                        FileIOSystem.Instance.housingdatabase.objectsLocation[index].size = currentobjsize;

                        gameObject.transform.rotation = currentrotation;
                        gameObject.transform.localPosition = cellIndicator.transform.localPosition;


                        EffectSystem.Instance.playplaceeffect(cellIndicator.transform.position);
                        SoundSystem.Instance.PlayAudio(cellIndicator.transform.position);

                        FileIOSystem.Instance.Save(FileIOSystem.Instance.housingdatabase , FileIOSystem.HousingFilename);

                    if (MapInfo.Instance.housingtutorial[2])
                    {
                        InteractEventManager.NotifyClearDialog();
                        InteractEventManager.NotifyDialogShow("좋아요! 가구위치를 수정했어요!!");
                        StartCoroutine(EndTutorial());
                        MapInfo.Instance.housingtutorial[2] = false;
                    }
                }
                    else
                    {
                        Vector2Int tempsize = currentobjsize;
                        tempsize.x = size.y;
                        tempsize.y = size.x;

                        gameObject.transform.localRotation = rot;
                        gameObject.transform.localPosition = PlacePosition(currentpos, size);

                    EffectSystem.Instance.playplaceeffect(cellIndicator.transform.position);

                    if (MapInfo.Instance.housingtutorial[2])
                    {
                        InteractEventManager.NotifyClearDialog();
                        InteractEventManager.NotifyDialogShow("이런, 올바르지 않은 위치에 배치했네요...!");
                    }

                }
                    catchmode = false;
                    cellIndicator.SetActive(false);
                 }
            InsertObject.GetComponent<XRGrabInteractable>().throwOnDetach = false;
            //InsertObject.GetComponent<XRGrabInteractable>().throwSmoothingCurve = curve;
            InsertObject = null;
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
        if (obj != null)
        {
            Destroy(obj);
        }
    }

    private IEnumerator EndTutorial()
    {
        float nowtime = Time.time;
        float latertime = Time.time;
        print("laterrL: "+latertime);
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


    //�̺�Ʈ �Լ�

    /// <summary>
    /// ��ġ �غ� �̺�Ʈ
    /// </summary>
    /// <param name="p"></param>
    private void PlaceEnterEvent(SelectEnterEventArgs p)
    {
        CreateObject = p.interactableObject.transform.gameObject;
        CatchObject = CreateObject;
        CreateManager = p;

        HousingUISystem.Instance.EnableButton(false);
        PlaceStartStructure();
    }

    /// <summary>
    /// ��ġ �Ϸ� �̺�Ʈ
    /// </summary>
    /// <param name="p"></param>
    private void PlaceEvent(SelectExitEventArgs p)
    {
        if (CreateObject != null)
        {
            PlaceStructure(p.interactableObject.transform.gameObject);
            if (HousingUISystem.Instance != null) HousingUISystem.Instance.EnableButton(true);
            MapInfo.Instance.ResetTileScale();
        }
    }


    /// <summary>
    /// ��ġ ���� �غ� �̺�Ʈ
    /// </summary>
    /// <param name="p"></param>
    private void InsertEnterEvent(SelectEnterEventArgs p)
    {
        InsertObject = p.interactableObject.transform.gameObject;
        CatchObject = InsertObject;
        InsertManager = p;
        InsertObject.GetComponent<XRGrabInteractable>().throwOnDetach = true;
        InsertObject.GetComponent<XRGrabInteractable>().throwSmoothingCurve = curve;

        Startinsertion();
        HousingUISystem.Instance.EnableButton(false);
        InsertionStartStructure(p.interactableObject.transform.gameObject);

        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="p"></param>
    private void InsertCompleteEvent(SelectExitEventArgs p)
    {
            RotatePlacementByHand(RotateRealTimebyHand());

            InsertionStructure(p.interactableObject.transform.gameObject);
            StopPlacement(false);
            if(HousingUISystem.Instance != null) HousingUISystem.Instance.EnableButton(true);
            MapInfo.Instance.ResetTileScale();
    }





    /*
    /// <summary>
    ///
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
        HousingUISystem.Instance.countlist[selectedObjectIndex].GetComponentInChildren<TMP_Text>().text = "" + database.objectsData[selectedObjectIndex].ObjectCount;

        //������ ���� ��� �ش� ������Ʈ ��Ȱ��ȭ
        if (database.objectsData[selectedObjectIndex].ObjectCount <= 0)
        {
            Debug.Log($"No Object");
            HousingUISystem.Instance.countlist[selectedObjectIndex].GetComponent<Button>().interactable = false;
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
    */
}






