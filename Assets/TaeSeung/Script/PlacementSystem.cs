using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using static UnityEngine.InputSystem.InputControlExtensions;


//배치와 관련된 모든 함수가 담김.

public class PlacementSystem : Singleton<PlacementSystem>
{
    //mouseindicator : cursor 오브젝트, cellindicator : cell위치 표시해주는 오브젝트
    [SerializeField]
    GameObject mouseIndicator, cellIndicator;

    //오브젝트 생성위치가 될 곳
    [SerializeField]
    private GameObject spawnpoint;

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


    //object가 새로 생성될때, 그 부모가 될 오브젝트
    [SerializeField]
    private GameObject ObjectLocation;

    //배치될 object의 타입
    private GridData floorData, funitureData;
    private Renderer previewRenderer;
    private List<GameObject> placedGameObjects = new();


    //현재 오브젝트의 size 상태(rotation때문에 추가됨), 이 부분은 나중에 deprecated시킬수도 있슴미다
    private GameObject CreateObject; //현재 생성된 오브젝트
    private GameObject CatchObject;  //생성된 오브젝트들 중 내가 잡은 오브젝트
    private Vector2Int currentobjsize;  //오브젝트가 차지하는 칸 사이즈
    private Quaternion currentrotation; //오브젝트의 로테이션
    private float changerrotationzvalue; //오브젝트 로테이션될 값
    private Vector3Int currentpos;  //오브젝트의 위치
    private bool catchmode = false; //오브젝트가 잡힌 상태인지?
    private XRGrabInteractable interact;    //오브젝트의 interactable 컴포넌트

    private GameObject cursororigin, cursorparent;

    [Obsolete]
    [SerializeField]
    CursorCollisionSystem cursorsystem;



    private void Start()
    {
        funitureData = new();
        floorData = new();

        MapInfo.Instance.MapInitialize();
        //스크립터블 오브젝트에서 이미 배치된 데이터들 가져오기
        for (short i = 0; i < database.objectsLocation.Count; i++) {

            int id = database.objectsLocation[i].OBJID;
            GameObject newObject = Instantiate(database.objectsData[id].Prefab);
            newObject.transform.localScale = newObject.transform.localScale * (1 / MapInfo.Instance.MapScale);

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
        cellIndicator.transform.localScale = cellIndicator.transform.localScale / MapInfo.Instance.MapScale;
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
          
        }


    }


    public void StartPlacement(int ID)
    {
        //이미 생성된 오브젝트가 있을 경우, 그 놈 파괴시키고 새로 누른 오브젝트를 생성시킬 예정, 근데 기획상 구현 변경될수도 있음
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

        //cellindicator 크기 설정(차지 영역)
        currentobjsize = database.objectsData[selectedObjectIndex].Size;
        MapInfo.Instance.SetTileScale(new Vector3(currentobjsize.x, currentobjsize.y, 1));
        currentrotation = new();

        //AR 환경상에서는 이 코드를 넣어야 함.
        GameObject newObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);
        CreateObject = newObject;
        newObject.transform.position = spawnpoint.transform.position;
        newObject.transform.localScale = newObject.transform.localScale * (1 / MapInfo.Instance.MapScale);
        interact = newObject.GetComponent<XRGrabInteractable>();

        //물체를 grab할때의 이벤트 추가
        SelectEnterEventArgs enterargs = makeEnterEventArgs(interact, interact.firstInteractorSelecting, interact.interactionManager);
        interact.selectEntered.AddListener((a) => PlaceEnterEvent(enterargs));

        //물체에 대한 grab을 풀때의 이벤트 추가
        SelectExitEventArgs exitargs = makeExitEventArgs(interact, interact.firstInteractorSelecting, interact.interactionManager);
        interact.selectExited.AddListener((a)=>PlaceEvent(exitargs));


        //컴퓨터에서 할땐 이 코드를 넣으면 됩니다.
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
        //컴퓨터 환경에서는 사용 가능
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
        Vector3Int gridPosition = new Vector3Int(Mathf.RoundToInt(mousePosition.x), Mathf.RoundToInt(mousePosition.y), Mathf.RoundToInt(mousePosition.z));

        //배치 불가능하면 cellindicator의 머터리얼을 바꿔서 표시해줌.
        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (placementValidity)
            print("asd");
        else
            print("asdgggb");

        previewRenderer.material.color = placementValidity ? Color.white : Color.red;

        mouseIndicator.transform.position = mousePosition;

        mousePosition = mousePosition * MapInfo.Instance.MapScale;
        mousePosition = new Vector3(Mathf.Round(mousePosition.x), Mathf.Round(mousePosition.y), Mathf.Round(mousePosition.z));
        cellIndicator.transform.position = mousePosition / MapInfo.Instance.MapScale;
    }


    //구조물 배치 (AR환경)
    private void PlaceStructure(GameObject gameObject)
    {
        //현재 커서 위치 가져옴
        Vector3 mousePosition = inputManager.GetSelectedMapPositionbyObject(gameObject.transform);

        if (!inputManager.ishit())
        {
            gameObject.transform.position = spawnpoint.transform.position;
            gameObject.transform.rotation = spawnpoint.transform.rotation;
            StopPlacement(false);
            return;
        }

        //현재 커서 위치를 기반으로 grid.WorldToCell하면 월드좌표계를 grid컴포넌트의 그리드로 즉시 변환해주지만, 이상하게 변환되서(버림연산함) 그냥 round시키는 방식으로 바꿈
        mousePosition = mousePosition * MapInfo.Instance.MapScale;
        Vector3Int gridPosition = new Vector3Int(Mathf.RoundToInt(mousePosition.x), Mathf.RoundToInt(mousePosition.y), Mathf.RoundToInt(mousePosition.z));
        ObjectLocation newlocation = new ObjectLocation();

        bool placementValidity = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        if (placementValidity == false)
        {
            gameObject.transform.position = spawnpoint.transform.position;
            gameObject.transform.rotation = spawnpoint.transform.rotation;
            StopPlacement(false);
            return;
        }

        //새로 배치될 물체의 위치, 회전 정보를 데이터베이스에 넣는 과정
        newlocation.location = gridPosition;
        newlocation.rotation = currentrotation;
        newlocation.OBJID = selectedObjectIndex;
        newlocation.size = currentobjsize;
        newlocation.InstanceId = gameObject.GetInstanceID();
        newlocation.placementstatus = true;

        MakeNewObject(selectedObjectIndex, ObjectLocation.transform, gridPosition, currentrotation, newlocation.size, "PlaceObject",gameObject);

        //만들어진 오브젝트에 대한 고유정보, 배치정보 수정 + 그 오브젝트에 대한 모든 정보 리스트에 추가
        database.objectsLocation.Add(newlocation);
        database.objectsData[selectedObjectIndex].ObjectCount -= 1;

        interact = gameObject.GetComponent<XRGrabInteractable>();
        SelectEnterEventArgs enterArgs = makeEnterEventArgs(interact, interact.firstInteractorSelecting, interact.interactionManager);
        SelectExitEventArgs exitargs = makeExitEventArgs(interact, interact.firstInteractorSelecting, interact.interactionManager);
        interact.selectEntered.RemoveAllListeners();
        interact.selectExited.RemoveAllListeners();
        interact.selectEntered.AddListener((a) => InsertEnterEvent(enterArgs));
        interact.selectExited.AddListener((a) => InsertCompleteEvent(exitargs));



        //수량이 없는 경우 해당 오브젝트 버튼 자체를 비활성화
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

        //잡은 시점에서의 물체 위치 저장 (삭제에서 사용될지도 몰라요)
        CatchObject = gameObject;
        cellIndicator.SetActive(true);
        catchmode = true;
        currentpos = database.objectsLocation[index].location;
    }

    private void InsertionStructure(GameObject gameObject) {

        Vector3 mousePosition = inputManager.GetSelectedMapPositionbyObject(gameObject.transform) * MapInfo.Instance.MapScale;
        Vector3Int gridPosition = new Vector3Int(Mathf.RoundToInt(mousePosition.x), Mathf.RoundToInt(mousePosition.y), Mathf.RoundToInt(mousePosition.z));


        //맵 바깥으로 커서를 이동시켰다면 삭제
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
                        gameObject.GetComponent<Rigidbody>().SetDensity(0.5f);
                    }
                    print(Vector3.Distance(gameObject.transform.position, ObjectLocation.transform.position));
                    CreateObject = null;
                StartCoroutine(throwdelete(gameObject));
                    //Destroy(gameObject);
                }
            }

            //아직 커서가 맵 안에 있다면 유효범위이므로 위치 수정
            else
            {
                int index = database.objectsLocation.FindIndex(data => data.InstanceId == gameObject.GetInstanceID());

                //일단 존재하는 가구가 맞는지 부터 확인해요
                if (index >= 0)
                {
                    int id = database.objectsLocation[index].OBJID;
                    Vector3Int pos = database.objectsLocation[index].location;
                    Quaternion rot = database.objectsLocation[index].rotation;
                    Vector2Int size = database.objectsLocation[index].size;
                    
                    
                    PlacementData data = funitureData.GetObjectAt(currentpos);
                    int placeindex = data.PlacedObjectIndex;

                    //배치 위치 수정할 위치가 이미 다른 가구가 배치되어 있으면 안 돼요. 단, 동일위치에 재배치하는 건 용서해줌
                    if (funitureData.CanPlaceObjectAt(gridPosition, currentobjsize) && gridPosition != pos)
                    {
                        gameObject.transform.SetParent(null);
                        //배치될 위치 정보 데이터를 바꿔줘요
                        funitureData.RemoveObjectAt(currentpos, size);
                        funitureData.AddObjectAt(gridPosition, size, id, placeindex);
                        database.objectsLocation[index].location = gridPosition;
                        database.objectsLocation[index].rotation = currentrotation;
                        database.objectsLocation[index].size = currentobjsize;
                    //배치될 위치로 수정해요
                        gameObject.transform.rotation = currentrotation;
                        gameObject.transform.position = ((Vector3)gridPosition) * (1 / MapInfo.Instance.MapScale);
                        gameObject.transform.SetParent(ObjectLocation.transform);

                        //일 다봤으니 방빼세요
                    }
                    //잘못 배치했으면 그냥 원래 있던자리로 가세요
                    else
                    {
                        gameObject.transform.SetParent(null);
                        gameObject.transform.rotation = rot;
                        gameObject.transform.position = ((Vector3)currentpos) * (1 / MapInfo.Instance.MapScale);
                        gameObject.transform.SetParent(ObjectLocation.transform);
                }
                    catchmode = false;
                    cellIndicator.SetActive(false);
                 }
            CreateObject = null;
        }
    }


    /// <summary>
    /// 배치 된 오브젝트 삭제시 중력 풀고 저 멀리로 보내버림
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
    /// 배치가 가능한 영역인지 파악.
    /// </summary>
    /// <param name="gridPosition">배치할 위치</param>
    /// <param name="selectedObjectIndex">이건 나중에 써먹을 인자여서 일단 냅둠</param>
    /// <returns></returns>
    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        //GridData selectedData = database.objectsData[selectedObjectIndex].ID == 0 ? floorData : funitureData;
        GridData selectedData = funitureData;
        return selectedData.CanPlaceObjectAt(gridPosition, currentobjsize);
    }


    /// <summary>
    /// 실시간으로 물체 로테이션 값을 받아서 회전된 각도를 구해주는 함수
    /// </summary>
    /// <returns></returns>
    public float RotateRealTimebyHand()
    {
        float y = CatchObject.transform.rotation.eulerAngles.y;

        y = Mathf.Abs(y);

        if (y % 360 >= 0 && y % 360 < 90)
            y = 0;

        else if (y % 360 >= 90 && y % 360 < 180)
            y = 90;

        else if (y % 360 >= 180 && y % 360 < 270)
            y = 180;

        else if (y % 360 >= 270 && y % 360 < 360)
            y = 270;


        cellIndicator.transform.rotation = Quaternion.Euler(new Vector3(90, 0, y));
        return y;
    }


    /// <summary>
    /// 최종적으로 물체 배치시 회전 각도에 맞게 하우징 가구를 회전해서 배치해주는 함수
    /// </summary>
    /// <param name="zangle"></param>
    private void RotatePlacementByHand(float zangle)
    {
        float z = 0;

        if (zangle % 360 >= 90 && zangle % 360 < 180)
        {
            z = 90;
            int tempx = currentobjsize.x;
            currentobjsize.x = currentobjsize.y;
            currentobjsize.y = tempx;

        }

        else if (zangle % 360 >= 180 && zangle % 360 < 270)
            z = 180;

        else if (zangle % 360 >= 270 && zangle % 360 < 360)
        {
            z = 270;
            int tempx = currentobjsize.x;
            currentobjsize.x = currentobjsize.y;
            currentobjsize.y = tempx;

        }

        Vector3 euler = new Vector3(0, z, 0 );
        //CatchObject.transform.rotation = Quaternion.Euler(euler);
        currentrotation = Quaternion.Euler(euler);

    }



    /// <summary>
    /// 새 오브젝트 생성시키고 배치시켜주는 함수
    /// </summary>
    /// <param name="id">해당 오브젝트의 id</param>
    /// <param name="parent">해당 오브젝트가 씬 계층상 부모로 둘 오브젝트</param>
    /// <param name="loc">해당 오브젝트가 배치될 그리드 위치</param>
    /// <param name="rot">해당 오브젝트의 회전</param>
    /// <param name="size">해당 오브젝트의 차지 칸 수</param>
    /// <param name="layer">해당 오브젝트가 배치될 레이어</param>
    /// <param name="newObject">배치될 오브젝트</param>
    /// <returns></returns>
    /// 
    private GameObject MakeNewObject(int id, Transform parent, Vector3Int loc, Quaternion rot, Vector2Int size, String layer, GameObject newObject)
    {
        if (id >= 0)
        {
            newObject.transform.rotation = rot;
            newObject.transform.position = ((Vector3)loc) *  ( 1 / MapInfo.Instance.MapScale);         
            newObject.layer = LayerMask.NameToLayer(layer);
            newObject.transform.SetParent(ObjectLocation.transform);

            placedGameObjects.Add(newObject);

            //GridData selectedData = database.objectsData[i].ID == 0 ? floorData : funitureData; 
            //혹시 부딪힐 필요가 없는 에셋이 존재하는 경우 위 코드를 활용해야함.

            GridData selectedData = funitureData;
            selectedData.AddObjectAt(loc, size, id, placedGameObjects.Count - 1);
            return newObject;
        }
        else return null;
    }



    /// <summary>
    /// 손으로 select되는 event에서 필수적으로 보내야 하는 SelectEnterEventArgs를 생성시켜주는 함수.
    /// </summary>
    /// <param name="interactable">쥐어지는 오브젝트에 대한 정보</param>
    /// <param name="InteractorSelect">쥐는 오브젝트에 대한 정보</param>
    /// <param name="interactionManager">interactionManager타입으로 상속받아진 클래스면 모두 가능</param>
    /// <returns> 위 인자 세개를 바탕으로 SelectEnterEventArgs객체를 만들어 반환함</returns>
    private SelectEnterEventArgs makeEnterEventArgs(XRGrabInteractable interactable, IXRSelectInteractor InteractorSelect, XRInteractionManager interactionManager)
    {
        SelectEnterEventArgs exiteventargs = new SelectEnterEventArgs();
        exiteventargs.interactableObject = interactable;
        exiteventargs.interactorObject = InteractorSelect;
        exiteventargs.manager = interactionManager;

        return exiteventargs;
    }

    /// <summary>
    /// select에서 벗어나게되는 event에서 필수적으로 보내야 하는 SelectExitEventArgs를 생성시켜주는 함수.
    /// </summary>
    /// <param name="interactable">쥐어지는 오브젝트에 대한 정보</param>
    /// <param name="InteractorSelect">쥐는 오브젝트에 대한 정보</param>
    /// <param name="interactionManager">interactionManager타입으로 상속받아진 클래스면 모두 가능</param>
    /// <returns> 위 인자 세개를 바탕으로 SelectExitEventArgs객체를 만들어 반환함</returns>
    private SelectExitEventArgs makeExitEventArgs(XRGrabInteractable interactable, IXRSelectInteractor InteractorSelect, XRInteractionManager interactionManager)
    {
        SelectExitEventArgs exiteventargs = new SelectExitEventArgs();
        exiteventargs.interactableObject = interactable;
        exiteventargs.interactorObject = InteractorSelect;
        exiteventargs.manager = interactionManager;

        return exiteventargs;
    }


    //이벤트 함수

    /// <summary>
    /// 배치 준비 이벤트
    /// </summary>
    /// <param name="p"></param>
    private void PlaceEnterEvent(SelectEnterEventArgs p)
    {
        CatchObject = p.interactableObject.transform.gameObject;
        PlaceStartStructure();
    }

    /// <summary>
    /// 배치 완료 이벤트
    /// </summary>
    /// <param name="p"></param>
    private void PlaceEvent(SelectExitEventArgs p)
    {
        float k = RotateRealTimebyHand();
        RotatePlacementByHand(k);
        PlaceStructure(p.interactableObject.transform.gameObject);

    }


    /// <summary>
    /// 배치 수정 준비 이벤트
    /// </summary>
    /// <param name="p"></param>
    private void InsertEnterEvent(SelectEnterEventArgs p)
    {
        Startinsertion();
        InsertionStartStructure(p.interactableObject.transform.gameObject);
    }

    /// <summary>
    /// 배치 수정 완료 이벤트
    /// </summary>
    /// <param name="p"></param>
    private void InsertCompleteEvent(SelectExitEventArgs p)
    {
        float k = RotateRealTimebyHand();
        RotatePlacementByHand(k);
        InsertionStructure(p.interactableObject.transform.gameObject);
        StopPlacement(false);
    }


    /// <summary>
    /// 앞으로 아예 안쓸 함수들은 아닌데, 현재 AR 환경안에서는 안쓸 함수들임.혹시 몰라서 deprecated시키고, 그냥 거들떠 보지 않는 걸 추천 
    /// </summary>
    [Obsolete]
    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI()) return;

        //현재 커서 위치 가져옴
        Vector3 mousePosition = inputManager.GetSelectedMapPositionInComputer();
        //만약 맵 바깥이면 배치할 필요가 없으니까 그대로 배치 실행 무시
        if (!inputManager.ishit()) return;

        //현재 커서 위치를 기반으로 grid.WorldToCell하면 월드좌표계를 grid컴포넌트의 그리드로 즉시 변환해주지만, 이상하게 변환되서(버림연산함) 그냥 round시키는 방식으로 바꿈
        mousePosition = mousePosition * MapInfo.Instance.MapScale;
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

        //실제배치
        GameObject newobject = MakeNewObject(selectedObjectIndex, ObjectLocation.transform, gridPosition, currentrotation, newlocation.size, "PlaceObject");
        UIInitialize.Instance.countlist[selectedObjectIndex].GetComponentInChildren<TMP_Text>().text = "" + database.objectsData[selectedObjectIndex].ObjectCount;

        //수량이 없는 경우 해당 오브젝트 비활성화
        if (database.objectsData[selectedObjectIndex].ObjectCount <= 0)
        {
            Debug.Log($"No Object");
            UIInitialize.Instance.countlist[selectedObjectIndex].GetComponent<Button>().interactable = false;
            StopPlacement(true);
        }
    }

    /// <summary>
    /// 앞으로 아예 안쓸 함수들은 아닌데, 현재 AR 환경안에서는 안쓸 함수들임.혹시 몰라서 deprecated시키고, 그냥 거들떠 보지 않는 걸 추천 
    /// </summary>
    //배치된 물체에 대한 위치수정 또는 삭제
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

                    //배치 위치 수정할 위치가 이미 다른 가구가 배치되어 있으면 안 돼요.
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
    /// 앞으로 아예 안쓸 함수들은 아닌데, 현재 AR 환경안에서는 안쓸 함수들임.혹시 몰라서 deprecated시키고, 그냥 거들떠 보지 않는 걸 추천 
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
    ///앞으로 아예 안쓸 함수들은 아닌데, 현재 AR 환경안에서는 안쓸 함수들임.혹시 몰라서 deprecated시키고, 그냥 거들떠 보지 않는 걸 추천 
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
