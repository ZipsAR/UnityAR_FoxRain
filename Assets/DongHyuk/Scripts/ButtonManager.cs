using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public GameObject buttonItSelf;
    public GameObject ButtonMng;
    private GameObject RadialObj1, RadialObj2, RadialObj3, RadialObj4, RadialObj5;

    [SerializeField]
    private ItemDatabase database;

    // Start is called before the first frame update
    void Start()
    {
        //RadialObj1.GetComponent<GameObject>();
        CurrentPage();
        if(buttonItSelf.name == "RadialPrev" && CurrentPage() == 0)
        {
            buttonItSelf.SetActive(false);
        }
        if(buttonItSelf.name == "RadialNext" && CurrentPage() == 2)
        {
            buttonItSelf.SetActive(false);
        }
        if(buttonItSelf.name == "ButtonBackpack")
        {
            ChangeRadial(CurrentPage(), buttonItSelf);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private int CurrentPage()
    {
        return ButtonMng.GetComponent<ButtonProperties>().curPage;
    }

    private void CheckAndDestroy(int radialNum)
    {
        if(ButtonMng.transform.parent.GetChild(radialNum - 1).transform.childCount == 0)
                {
                    Debug.Log("기존 Radial에 존재하던 오브젝트가 없어 새로 생성합니다.");
                }
                else
                {
                    Debug.Log("기존 Radial에 존재하던 오브젝트를 파괴하고 새 오브젝트로 대체합니다.");
                    Destroy(ButtonMng.transform.parent.GetChild(radialNum + 1).transform.GetChild(0));
                }
    } 

    // 오늘은 내가 스파게티 요리사~ (제출 이후 코드 리팩토링 할게요)
    private void ChangeRadial(int curPage, GameObject calledBtn)
    {
        int idx, invenIdx;
        switch(curPage)
        {
            case 0:
            {
                // 2번 버튼
                CheckAndDestroy(2);
                // 아이템 갯수가 1개 이상이면 ItemDB속 ID가 일치하는 prefab을 찾아 Radial에 생성합니다.
                idx = database.ItemData.FindIndex(data => data.ID == 2000);
                try
                {
                    invenIdx = FileIOSystem.Instance.invendatabase.mydata.FindIndex(data => data.id == 2000);
                    Debug.Log(invenIdx+" line 73");
                    if(FileIOSystem.Instance.invendatabase.mydata[invenIdx].count > 0)
                    {
                        // ButtonBackpack에서 함수가 실행된 경우에 Radial을 찾아서 child로 생성합니다. 생각해보니 calledBtn기준으로 parsing하는 게 아니라 ButtonManager기준으로 child tree 찾아가는게 낫겠네요(추후 수정할게요)
                        if(calledBtn.name == "ButtonBackpack")
                        {
                            RadialObj1 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform.GetChild(0).Find("RadialButton2")); 
                        }
                        // ButtonPrev, Next에서 실행된 경우에 Radial을 찾아서 child로 생성합니다.
                        else if(calledBtn.name == "ButtonPrev" || calledBtn.name == "ButtonNext")
                        {
                            RadialObj1 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform.parent.Find("RadialButton2"));
                        }
                        /*// 그 외(radialbutton에서 실행된 경우에, 본인의 child로 생성합니다.)
                        else
                        {
                            RadialObj1 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform);
                        }*/
                    }
                    // 중력속성, 크기, 위치 등 조정
                    RadialObj1.GetComponent<Rigidbody>().useGravity = false;  
                    RadialObj1.transform.localScale *= 1000f;
                    RadialObj1.transform.localPosition += new Vector3(0.0f, 0.0f, -20.0f); 
                    Debug.Log(RadialObj1.name);
                }
                catch
                {
                    Debug.Log("인벤토리에 해당 아이템이 추가된 적이 없습니다.");
                }
                
                
                
                
                // 3번 버튼
                CheckAndDestroy(3);
                // 아이템 갯수가 1개 이상이면 ItemDB속 ID가 일치하는 prefab을 찾아 Radial에 생성합니다.
                idx = database.ItemData.FindIndex(data => data.ID == 2001);
                try
                {
                    invenIdx = FileIOSystem.Instance.invendatabase.mydata.FindIndex(data => data.id == 2001);
                    Debug.Log(invenIdx);
                    if(FileIOSystem.Instance.invendatabase.mydata[invenIdx].count > 0)
                    {
                        // ButtonBackpack에서 함수가 실행된 경우에 Radial을 찾아서 child로 생성합니다. 생각해보니 calledBtn기준으로 parsing하는 게 아니라 ButtonManager기준으로 child tree 찾아가는게 낫겠네요(추후 수정할게요)
                        if(calledBtn.name == "ButtonBackpack")
                        {
                            RadialObj2 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform.GetChild(0).Find("RadialButton3")); 
                        }
                        // ButtonPrev, Next에서 실행된 경우에 Radial을 찾아서 child로 생성합니다.
                        else if(calledBtn.name == "ButtonPrev" || calledBtn.name == "ButtonNext")
                        {
                            RadialObj2 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform.parent.Find("RadialButton3"));
                        }
                        /*// 그 외(radialbutton에서 실행된 경우에, 본인의 child로 생성합니다.)
                        else
                        {
                            RadialObj2 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform);
                        }*/
                    }
                    // 중력속성, 크기, 위치 등 조정
                    RadialObj2.GetComponent<Rigidbody>().useGravity = false;  
                    RadialObj2.transform.localScale *= 1000f;
                    RadialObj2.transform.localPosition += new Vector3(0.0f, 0.0f, -20.0f); 
                    Debug.Log(RadialObj2.name);
                }
                catch
                {
                    Debug.Log("인벤토리에 해당 아이템이 추가된 적이 없습니다.");
                }

                // 4번 버튼
                CheckAndDestroy(4);
                // 아이템 갯수가 1개 이상이면 ItemDB속 ID가 일치하는 prefab을 찾아 Radial에 생성합니다.
                idx = database.ItemData.FindIndex(data => data.ID == 2002);
                try
                {
                    invenIdx = FileIOSystem.Instance.invendatabase.mydata.FindIndex(data => data.id == 2002);
                    Debug.Log(invenIdx);
                    if(FileIOSystem.Instance.invendatabase.mydata[invenIdx].count > 0)
                    {
                        // ButtonBackpack에서 함수가 실행된 경우에 Radial을 찾아서 child로 생성합니다. 생각해보니 calledBtn기준으로 parsing하는 게 아니라 ButtonManager기준으로 child tree 찾아가는게 낫겠네요(추후 수정할게요)
                        if(calledBtn.name == "ButtonBackpack")
                        {
                            RadialObj3 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform.GetChild(0).Find("RadialButton4")); 
                        }
                        // ButtonPrev, Next에서 실행된 경우에 Radial을 찾아서 child로 생성합니다.
                        else if(calledBtn.name == "ButtonPrev" || calledBtn.name == "ButtonNext")
                        {
                            RadialObj3 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform.parent.Find("RadialButton4"));
                        }
                        /*// 그 외(radialbutton에서 실행된 경우에, 본인의 child로 생성합니다.)
                        else
                        {
                            RadialObj3 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform);
                        }*/
                    }
                    // 중력속성, 크기, 위치 등 조정
                    RadialObj3.GetComponent<Rigidbody>().useGravity = false;  
                    RadialObj3.transform.localScale *= 1000f;
                    RadialObj3.transform.localPosition += new Vector3(0.0f, 0.0f, -20.0f); 
                    Debug.Log(RadialObj3.name);
                }
                catch
                {
                    Debug.Log("인벤토리에 해당 아이템이 추가된 적이 없습니다.");
                }

                // 5번 버튼
                CheckAndDestroy(5);
                // 아이템 갯수가 1개 이상이면 ItemDB속 ID가 일치하는 prefab을 찾아 Radial에 생성합니다.
                idx = database.ItemData.FindIndex(data => data.ID == 2003);
                try
                {
                    invenIdx = FileIOSystem.Instance.invendatabase.mydata.FindIndex(data => data.id == 2003);
                    Debug.Log(invenIdx);
                    if(FileIOSystem.Instance.invendatabase.mydata[invenIdx].count > 0)
                    {
                        // ButtonBackpack에서 함수가 실행된 경우에 Radial을 찾아서 child로 생성합니다. 생각해보니 calledBtn기준으로 parsing하는 게 아니라 ButtonManager기준으로 child tree 찾아가는게 낫겠네요(추후 수정할게요)
                        if(calledBtn.name == "ButtonBackpack")
                        {
                            RadialObj4 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform.GetChild(0).Find("RadialButton5")); 
                        }
                        // ButtonPrev, Next에서 실행된 경우에 Radial을 찾아서 child로 생성합니다.
                        else if(calledBtn.name == "ButtonPrev" || calledBtn.name == "ButtonNext")
                        {
                            RadialObj4 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform.parent.Find("RadialButton5"));
                        }
                        /*// 그 외(radialbutton에서 실행된 경우에, 본인의 child로 생성합니다.)
                        else
                        {
                            RadialObj4 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform);
                        }*/
                    }
                    // 중력속성, 크기, 위치 등 조정
                    RadialObj4.GetComponent<Rigidbody>().useGravity = false;  
                    RadialObj4.transform.localScale *= 1000f;
                    RadialObj4.transform.localPosition += new Vector3(0.0f, 0.0f, -20.0f); 
                    Debug.Log(RadialObj4.name);
                }
                catch
                {
                    Debug.Log("인벤토리에 해당 아이템이 추가된 적이 없습니다.");
                }

                // 6번 버튼
                CheckAndDestroy(6);
                // 아이템 갯수가 1개 이상이면 ItemDB속 ID가 일치하는 prefab을 찾아 Radial에 생성합니다.
                idx = database.ItemData.FindIndex(data => data.ID == 2004);
                try
                {
                    invenIdx = FileIOSystem.Instance.invendatabase.mydata.FindIndex(data => data.id == 2004);
                    Debug.Log(invenIdx);
                    if(FileIOSystem.Instance.invendatabase.mydata[invenIdx].count > 0)
                    {
                        // ButtonBackpack에서 함수가 실행된 경우에 Radial을 찾아서 child로 생성합니다. 생각해보니 calledBtn기준으로 parsing하는 게 아니라 ButtonManager기준으로 child tree 찾아가는게 낫겠네요(추후 수정할게요)
                        if(calledBtn.name == "ButtonBackpack")
                        {
                            RadialObj5 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform.GetChild(0).Find("RadialButton6")); 
                        }
                        // ButtonPrev, Next에서 실행된 경우에 Radial을 찾아서 child로 생성합니다.
                        else if(calledBtn.name == "ButtonPrev" || calledBtn.name == "ButtonNext")
                        {
                            RadialObj5 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform.parent.Find("RadialButton6"));
                        }
                        /*// 그 외(radialbutton에서 실행된 경우에, 본인의 child로 생성합니다.)
                        else
                        {
                            RadialObj5 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform);
                        }*/
                    }
                    // 중력속성, 크기, 위치 등 조정
                    RadialObj5.GetComponent<Rigidbody>().useGravity = false;  
                    RadialObj5.transform.localScale *= 1000f;
                    RadialObj5.transform.localPosition += new Vector3(0.0f, 0.0f, -20.0f); 
                    Debug.Log(RadialObj5.name);
                }
                catch
                {
                    Debug.Log("인벤토리에 해당 아이템이 추가된 적이 없습니다.");
                }
            }
            break;
            case 1:
            {
                // 2번 버튼
                CheckAndDestroy(2);
                // 아이템 갯수가 1개 이상이면 ItemDB속 ID가 일치하는 prefab을 찾아 Radial에 생성합니다.
                idx = database.ItemData.FindIndex(data => data.ID == 2005);
                try
                {
                    invenIdx = FileIOSystem.Instance.invendatabase.mydata.FindIndex(data => data.id == 2005);
                    Debug.Log(invenIdx+" line 73");
                    if(FileIOSystem.Instance.invendatabase.mydata[invenIdx].count > 0)
                    {
                        // ButtonBackpack에서 함수가 실행된 경우에 Radial을 찾아서 child로 생성합니다. 생각해보니 calledBtn기준으로 parsing하는 게 아니라 ButtonManager기준으로 child tree 찾아가는게 낫겠네요(추후 수정할게요)
                        if(calledBtn.name == "ButtonBackpack")
                        {
                            RadialObj1 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform.GetChild(0).Find("RadialButton2")); 
                        }
                        // ButtonPrev, Next에서 실행된 경우에 Radial을 찾아서 child로 생성합니다.
                        else if(calledBtn.name == "ButtonPrev" || calledBtn.name == "ButtonNext")
                        {
                            RadialObj1 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform.parent.Find("RadialButton2"));
                        }
                        /*// 그 외(radialbutton에서 실행된 경우에, 본인의 child로 생성합니다.)
                        else
                        {
                            RadialObj1 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform);
                        }*/
                    }
                    // 중력속성, 크기, 위치 등 조정
                    RadialObj1.GetComponent<Rigidbody>().useGravity = false;  
                    RadialObj1.transform.localScale *= 1000f;
                    RadialObj1.transform.localPosition += new Vector3(0.0f, 0.0f, -20.0f); 
                    Debug.Log(RadialObj1.name);
                }
                catch
                {
                    Debug.Log("인벤토리에 해당 아이템이 추가된 적이 없습니다.");
                }
                
                
                
                
                // 3번 버튼
                CheckAndDestroy(3);
                // 아이템 갯수가 1개 이상이면 ItemDB속 ID가 일치하는 prefab을 찾아 Radial에 생성합니다.
                idx = database.ItemData.FindIndex(data => data.ID == 2006);
                try
                {
                    invenIdx = FileIOSystem.Instance.invendatabase.mydata.FindIndex(data => data.id == 2006);
                    Debug.Log(invenIdx);
                    if(FileIOSystem.Instance.invendatabase.mydata[invenIdx].count > 0)
                    {
                        // ButtonBackpack에서 함수가 실행된 경우에 Radial을 찾아서 child로 생성합니다. 생각해보니 calledBtn기준으로 parsing하는 게 아니라 ButtonManager기준으로 child tree 찾아가는게 낫겠네요(추후 수정할게요)
                        if(calledBtn.name == "ButtonBackpack")
                        {
                            RadialObj2 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform.GetChild(0).Find("RadialButton3")); 
                        }
                        // ButtonPrev, Next에서 실행된 경우에 Radial을 찾아서 child로 생성합니다.
                        else if(calledBtn.name == "ButtonPrev" || calledBtn.name == "ButtonNext")
                        {
                            RadialObj2 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform.parent.Find("RadialButton3"));
                        }
                        /*// 그 외(radialbutton에서 실행된 경우에, 본인의 child로 생성합니다.)
                        else
                        {
                            RadialObj2 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform);
                        }*/
                    }
                    // 중력속성, 크기, 위치 등 조정
                    RadialObj2.GetComponent<Rigidbody>().useGravity = false;  
                    RadialObj2.transform.localScale *= 1000f;
                    RadialObj2.transform.localPosition += new Vector3(0.0f, 0.0f, -20.0f); 
                    Debug.Log(RadialObj2.name);
                }
                catch
                {
                    Debug.Log("인벤토리에 해당 아이템이 추가된 적이 없습니다.");
                }

                // 4번 버튼
                CheckAndDestroy(4);
                // 아이템 갯수가 1개 이상이면 ItemDB속 ID가 일치하는 prefab을 찾아 Radial에 생성합니다.
                idx = database.ItemData.FindIndex(data => data.ID == 2007);
                try
                {
                    invenIdx = FileIOSystem.Instance.invendatabase.mydata.FindIndex(data => data.id == 2007);
                    Debug.Log(invenIdx);
                    if(FileIOSystem.Instance.invendatabase.mydata[invenIdx].count > 0)
                    {
                        // ButtonBackpack에서 함수가 실행된 경우에 Radial을 찾아서 child로 생성합니다. 생각해보니 calledBtn기준으로 parsing하는 게 아니라 ButtonManager기준으로 child tree 찾아가는게 낫겠네요(추후 수정할게요)
                        if(calledBtn.name == "ButtonBackpack")
                        {
                            RadialObj3 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform.GetChild(0).Find("RadialButton4")); 
                        }
                        // ButtonPrev, Next에서 실행된 경우에 Radial을 찾아서 child로 생성합니다.
                        else if(calledBtn.name == "ButtonPrev" || calledBtn.name == "ButtonNext")
                        {
                            RadialObj3 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform.parent.Find("RadialButton4"));
                        }
                        /*// 그 외(radialbutton에서 실행된 경우에, 본인의 child로 생성합니다.)
                        else
                        {
                            RadialObj3 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform);
                        }*/
                    }
                    // 중력속성, 크기, 위치 등 조정
                    RadialObj3.GetComponent<Rigidbody>().useGravity = false;  
                    RadialObj3.transform.localScale *= 1000f;
                    RadialObj3.transform.localPosition += new Vector3(0.0f, 0.0f, -20.0f); 
                    Debug.Log(RadialObj3.name);
                }
                catch
                {
                    Debug.Log("인벤토리에 해당 아이템이 추가된 적이 없습니다.");
                }

                // 5번 버튼
                CheckAndDestroy(5);
                // 아이템 갯수가 1개 이상이면 ItemDB속 ID가 일치하는 prefab을 찾아 Radial에 생성합니다.
                idx = database.ItemData.FindIndex(data => data.ID == 2008);
                try
                {
                    invenIdx = FileIOSystem.Instance.invendatabase.mydata.FindIndex(data => data.id == 2008);
                    Debug.Log(invenIdx);
                    if(FileIOSystem.Instance.invendatabase.mydata[invenIdx].count > 0)
                    {
                        // ButtonBackpack에서 함수가 실행된 경우에 Radial을 찾아서 child로 생성합니다. 생각해보니 calledBtn기준으로 parsing하는 게 아니라 ButtonManager기준으로 child tree 찾아가는게 낫겠네요(추후 수정할게요)
                        if(calledBtn.name == "ButtonBackpack")
                        {
                            RadialObj4 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform.GetChild(0).Find("RadialButton5")); 
                        }
                        // ButtonPrev, Next에서 실행된 경우에 Radial을 찾아서 child로 생성합니다.
                        else if(calledBtn.name == "ButtonPrev" || calledBtn.name == "ButtonNext")
                        {
                            RadialObj4 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform.parent.Find("RadialButton5"));
                        }
                        /*// 그 외(radialbutton에서 실행된 경우에, 본인의 child로 생성합니다.)
                        else
                        {
                            RadialObj4 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform);
                        }*/
                    }
                    // 중력속성, 크기, 위치 등 조정
                    RadialObj4.GetComponent<Rigidbody>().useGravity = false;  
                    RadialObj4.transform.localScale *= 1000f;
                    RadialObj4.transform.localPosition += new Vector3(0.0f, 0.0f, -20.0f); 
                    Debug.Log(RadialObj4.name);
                }
                catch
                {
                    Debug.Log("인벤토리에 해당 아이템이 추가된 적이 없습니다.");
                }
            }
            break;
            case 2:
            {
                // 2번 버튼
                CheckAndDestroy(2);
                // 아이템 갯수가 1개 이상이면 ItemDB속 ID가 일치하는 prefab을 찾아 Radial에 생성합니다.
                idx = database.ItemData.FindIndex(data => data.ID == 3000);
                try
                {
                    invenIdx = FileIOSystem.Instance.invendatabase.mydata.FindIndex(data => data.id == 3000);
                    Debug.Log(invenIdx+" line 73");
                    if(FileIOSystem.Instance.invendatabase.mydata[invenIdx].count > 0)
                    {
                        // ButtonBackpack에서 함수가 실행된 경우에 Radial을 찾아서 child로 생성합니다. 생각해보니 calledBtn기준으로 parsing하는 게 아니라 ButtonManager기준으로 child tree 찾아가는게 낫겠네요(추후 수정할게요)
                        if(calledBtn.name == "ButtonBackpack")
                        {
                            RadialObj1 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform.GetChild(0).Find("RadialButton2")); 
                        }
                        // ButtonPrev, Next에서 실행된 경우에 Radial을 찾아서 child로 생성합니다.
                        else if(calledBtn.name == "ButtonPrev" || calledBtn.name == "ButtonNext")
                        {
                            RadialObj1 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform.parent.Find("RadialButton2"));
                        }
                        /*// 그 외(radialbutton에서 실행된 경우에, 본인의 child로 생성합니다.)
                        else
                        {
                            RadialObj1 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform);
                        }*/
                    }
                    // 중력속성, 크기, 위치 등 조정
                    RadialObj1.GetComponent<Rigidbody>().useGravity = false;  
                    RadialObj1.transform.localScale *= 1000f;
                    RadialObj1.transform.localPosition += new Vector3(0.0f, 0.0f, -20.0f); 
                    Debug.Log(RadialObj1.name);
                }
                catch
                {
                    Debug.Log("인벤토리에 해당 아이템이 추가된 적이 없습니다.");
                }
                
                
                
                
                // 3번 버튼
                CheckAndDestroy(3);
                // 아이템 갯수가 1개 이상이면 ItemDB속 ID가 일치하는 prefab을 찾아 Radial에 생성합니다.
                idx = database.ItemData.FindIndex(data => data.ID == 3001);
                try
                {
                    invenIdx = FileIOSystem.Instance.invendatabase.mydata.FindIndex(data => data.id == 3001);
                    Debug.Log(invenIdx);
                    if(FileIOSystem.Instance.invendatabase.mydata[invenIdx].count > 0)
                    {
                        // ButtonBackpack에서 함수가 실행된 경우에 Radial을 찾아서 child로 생성합니다. 생각해보니 calledBtn기준으로 parsing하는 게 아니라 ButtonManager기준으로 child tree 찾아가는게 낫겠네요(추후 수정할게요)
                        if(calledBtn.name == "ButtonBackpack")
                        {
                            RadialObj2 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform.GetChild(0).Find("RadialButton3")); 
                        }
                        // ButtonPrev, Next에서 실행된 경우에 Radial을 찾아서 child로 생성합니다.
                        else if(calledBtn.name == "ButtonPrev" || calledBtn.name == "ButtonNext")
                        {
                            RadialObj2 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform.parent.Find("RadialButton3"));
                        }
                        /*// 그 외(radialbutton에서 실행된 경우에, 본인의 child로 생성합니다.)
                        else
                        {
                            RadialObj2 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform);
                        }*/
                    }
                    // 중력속성, 크기, 위치 등 조정
                    RadialObj2.GetComponent<Rigidbody>().useGravity = false;  
                    RadialObj2.transform.localScale *= 1000f;
                    RadialObj2.transform.localPosition += new Vector3(0.0f, 0.0f, -20.0f); 
                    Debug.Log(RadialObj2.name);
                }
                catch
                {
                    Debug.Log("인벤토리에 해당 아이템이 추가된 적이 없습니다.");
                }

                // 4번 버튼
                CheckAndDestroy(4);
                // 아이템 갯수가 1개 이상이면 ItemDB속 ID가 일치하는 prefab을 찾아 Radial에 생성합니다.
                idx = database.ItemData.FindIndex(data => data.ID == 3002);
                try
                {
                    invenIdx = FileIOSystem.Instance.invendatabase.mydata.FindIndex(data => data.id == 3002);
                    Debug.Log(invenIdx);
                    if(FileIOSystem.Instance.invendatabase.mydata[invenIdx].count > 0)
                    {
                        // ButtonBackpack에서 함수가 실행된 경우에 Radial을 찾아서 child로 생성합니다. 생각해보니 calledBtn기준으로 parsing하는 게 아니라 ButtonManager기준으로 child tree 찾아가는게 낫겠네요(추후 수정할게요)
                        if(calledBtn.name == "ButtonBackpack")
                        {
                            RadialObj3 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform.GetChild(0).Find("RadialButton4")); 
                        }
                        // ButtonPrev, Next에서 실행된 경우에 Radial을 찾아서 child로 생성합니다.
                        else if(calledBtn.name == "ButtonPrev" || calledBtn.name == "ButtonNext")
                        {
                            RadialObj3 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform.parent.Find("RadialButton4"));
                        }
                        /*// 그 외(radialbutton에서 실행된 경우에, 본인의 child로 생성합니다.)
                        else
                        {
                            RadialObj3 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform);
                        }*/
                    }
                    // 중력속성, 크기, 위치 등 조정
                    RadialObj3.GetComponent<Rigidbody>().useGravity = false;  
                    RadialObj3.transform.localScale *= 1000f;
                    RadialObj3.transform.localPosition += new Vector3(0.0f, 0.0f, -20.0f); 
                    Debug.Log(RadialObj3.name);
                }
                catch
                {
                    Debug.Log("인벤토리에 해당 아이템이 추가된 적이 없습니다.");
                }

                // 5번 버튼
                CheckAndDestroy(5);
                // 아이템 갯수가 1개 이상이면 ItemDB속 ID가 일치하는 prefab을 찾아 Radial에 생성합니다.
                idx = database.ItemData.FindIndex(data => data.ID == 3003);
                try
                {
                    invenIdx = FileIOSystem.Instance.invendatabase.mydata.FindIndex(data => data.id == 3003);
                    Debug.Log(invenIdx);
                    if(FileIOSystem.Instance.invendatabase.mydata[invenIdx].count > 0)
                    {
                        // ButtonBackpack에서 함수가 실행된 경우에 Radial을 찾아서 child로 생성합니다. 생각해보니 calledBtn기준으로 parsing하는 게 아니라 ButtonManager기준으로 child tree 찾아가는게 낫겠네요(추후 수정할게요)
                        if(calledBtn.name == "ButtonBackpack")
                        {
                            RadialObj4 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform.GetChild(0).Find("RadialButton5")); 
                        }
                        // ButtonPrev, Next에서 실행된 경우에 Radial을 찾아서 child로 생성합니다.
                        else if(calledBtn.name == "ButtonPrev" || calledBtn.name == "ButtonNext")
                        {
                            RadialObj4 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform.parent.Find("RadialButton5"));
                        }
                        /*// 그 외(radialbutton에서 실행된 경우에, 본인의 child로 생성합니다.)
                        else
                        {
                            RadialObj4 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform);
                        }*/
                    }
                    // 중력속성, 크기, 위치 등 조정
                    RadialObj4.GetComponent<Rigidbody>().useGravity = false;  
                    RadialObj4.transform.localScale *= 1000f;
                    RadialObj4.transform.localPosition += new Vector3(0.0f, 0.0f, -20.0f); 
                    Debug.Log(RadialObj4.name);
                }
                catch
                {
                    Debug.Log("인벤토리에 해당 아이템이 추가된 적이 없습니다.");
                }

                // 6번 버튼
                CheckAndDestroy(6);
                // 아이템 갯수가 1개 이상이면 ItemDB속 ID가 일치하는 prefab을 찾아 Radial에 생성합니다.
                idx = database.ItemData.FindIndex(data => data.ID == 3004);
                try
                {
                    invenIdx = FileIOSystem.Instance.invendatabase.mydata.FindIndex(data => data.id == 3004);
                    Debug.Log(invenIdx);
                    if(FileIOSystem.Instance.invendatabase.mydata[invenIdx].count > 0)
                    {
                        // ButtonBackpack에서 함수가 실행된 경우에 Radial을 찾아서 child로 생성합니다. 생각해보니 calledBtn기준으로 parsing하는 게 아니라 ButtonManager기준으로 child tree 찾아가는게 낫겠네요(추후 수정할게요)
                        if(calledBtn.name == "ButtonBackpack")
                        {
                            RadialObj5 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform.GetChild(0).Find("RadialButton6")); 
                        }
                        // ButtonPrev, Next에서 실행된 경우에 Radial을 찾아서 child로 생성합니다.
                        else if(calledBtn.name == "ButtonPrev" || calledBtn.name == "ButtonNext")
                        {
                            RadialObj5 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform.parent.Find("RadialButton6"));
                        }
                        /*// 그 외(radialbutton에서 실행된 경우에, 본인의 child로 생성합니다.)
                        else
                        {
                            RadialObj5 = Instantiate(database.ItemData[idx].Prefab, calledBtn.transform);
                        }*/
                    }
                    // 중력속성, 크기, 위치 등 조정
                    RadialObj5.GetComponent<Rigidbody>().useGravity = false;  
                    RadialObj5.transform.localScale *= 1000f;
                    RadialObj5.transform.localPosition += new Vector3(0.0f, 0.0f, -20.0f); 
                    Debug.Log(RadialObj5.name);
                }
                catch
                {
                    Debug.Log("인벤토리에 해당 아이템이 추가된 적이 없습니다.");
                }
            }
            break;
            default:
            {
                Debug.Log("현재 페이지가 정상적으로 불러와지지 않았거나, ChangeRadial 함수가 정상 작동하지 않았습니다.");
            }
            break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if(other.gameObject.name == "index003.Col")
        {
           Debug.Log("button pushed by Second finger's tip");
           switch(buttonItSelf.name)
           {
                case "ButtonInteract":
                {
                    GameManager.Instance.LoadScene("InteractMode");
                }
                break;
                case "ButtonBackpack":
                {
                    buttonItSelf.transform.GetChild(0).gameObject.SetActive(true);
                    for(int i = 1; i < 5; i++)
                        buttonItSelf.transform.parent.GetChild(i).gameObject.SetActive(false);
                    ChangeRadial(CurrentPage(), buttonItSelf);
                }
                break;
                case "ButtonX":
                {
                    Application.Quit(0);
                }
                break;
                case "ButtonEdit":
                {
                    GameManager.Instance.ChangeMode(PlayMode.HousingMode);
                    GameManager.Instance.LoadScene("HousingMode");
                }
                break;
                case "ButtonBasket":
                {
                    GameManager.Instance.ChangeMode(PlayMode.StoreMode);
                    GameManager.Instance.LoadScene("StoreScene");
                }
                break;
                case "ButtonHome":
                {
                    if(buttonItSelf.transform.GetChild(0).gameObject.activeSelf == true)
                    {
                        buttonItSelf.transform.GetChild(0).gameObject.SetActive(false);
                        buttonItSelf.transform.GetChild(1).gameObject.SetActive(true);
                        MapInfo.Instance.SetVisiblemode();
                        MapInfo.Instance.SetReScale(6.4f);
                        Debug.Log("맵이 현실과 유사한 사이즈로 생성되었습니다.");
                    }
                    else
                    {
                        buttonItSelf.transform.GetChild(0).gameObject.SetActive(true);
                        buttonItSelf.transform.GetChild(1).gameObject.SetActive(false);
                        MapInfo.Instance.SetInvisiblemode();
                        Debug.Log("맵이 꺼졌습니다. 꺼져.");
                    }
                }
                break;
                case "RadialPrev":
                {
                    // 2페이지서 첫페이지로 넘어가면 RadialPrev 비활성화, curPage범위: (0,2)
                    if(ButtonMng.GetComponent<ButtonProperties>().curPage == 1)
                    {
                        buttonItSelf.SetActive(false);
                        ButtonMng.GetComponent<ButtonProperties>().curPage --;
                    }
                    // 마지막 페이지서 이전 페이지로 넘어가면 RadialNext 활성화
                    else if(ButtonMng.GetComponent<ButtonProperties>().curPage == 2)
                    {
                        buttonItSelf.transform.parent.Find("RadialNext").gameObject.SetActive(true);
                        ButtonMng.GetComponent<ButtonProperties>().curPage --;
                    }
                    else
                    {
                        ButtonMng.GetComponent<ButtonProperties>().curPage --;
                    }
                    Debug.Log("현재 Radial 페이지: "+ButtonMng.GetComponent<ButtonProperties>().curPage + 1);
                    ChangeRadial(CurrentPage(), buttonItSelf);
                }
                break;
                case "RadialNext":
                {
                    // 2페이지서 마지막 페이지로 넘어가면 RadialNext 비활성화
                    if(ButtonMng.GetComponent<ButtonProperties>().curPage == 1)
                    {
                        buttonItSelf.SetActive(false);
                        ButtonMng.GetComponent<ButtonProperties>().curPage ++;
                    }
                    // 첫페이지서 2페이지로 넘어가면 RadialPrev 활성화
                    else if(ButtonMng.GetComponent<ButtonProperties>().curPage == 0)
                    {
                        buttonItSelf.transform.parent.Find("RadialPrev").gameObject.SetActive(true);
                        ButtonMng.GetComponent<ButtonProperties>().curPage ++;
                    }
                    else
                    {
                        ButtonMng.GetComponent<ButtonProperties>().curPage ++;
                    }
                    Debug.Log("현재 Radial 페이지: "+ButtonMng.GetComponent<ButtonProperties>().curPage);
                    ChangeRadial(CurrentPage(), buttonItSelf);
                }
                break;
                case "RadialButton1":
                {
                    buttonItSelf.transform.parent.gameObject.SetActive(false);
                    for(int i = 1; i < 5; i++)
                        buttonItSelf.transform.parent.parent.parent.GetChild(i).gameObject.SetActive(true);
                }
                break;
                case "RadialButton2":
                {
                    buttonItSelf.transform.parent.gameObject.SetActive(false);
                    for(int i = 1; i < 5; i++)
                        buttonItSelf.transform.parent.parent.parent.GetChild(i).gameObject.SetActive(true);
                    switch(CurrentPage())
                    {
                        case 0:
                        {
                            if(FileIOSystem.Instance.invendatabase.mydata[2000].count > 0)
                            {
                                GameObject unPackedItem = Instantiate(database.ItemData[database.ItemData.FindIndex(data => data.ID == 2000)].Prefab);
                                unPackedItem.transform.localPosition = GameManager.Instance.leftHand.transform.position + new Vector3(0.0f,-0.5f, 0.0f);
                                FileIOSystem.Instance.invendatabase.mydata[2000].count --;
                            }
                        }
                        break;
                        case 1:
                        {
                            if(FileIOSystem.Instance.invendatabase.mydata[2005].count > 0)
                            {
                                GameObject unPackedItem = Instantiate(database.ItemData[database.ItemData.FindIndex(data => data.ID == 2005)].Prefab);
                                unPackedItem.transform.localPosition = GameManager.Instance.leftHand.transform.position + new Vector3(0.0f,-0.5f, 0.0f);
                                FileIOSystem.Instance.invendatabase.mydata[2005].count --;
                            }
                        }
                        break;
                        case 2:
                        {
                            if(FileIOSystem.Instance.invendatabase.mydata[3000].count > 0)
                            {
                                GameObject unPackedItem = Instantiate(database.ItemData[database.ItemData.FindIndex(data => data.ID == 3000)].Prefab);
                                unPackedItem.transform.localPosition = GameManager.Instance.leftHand.transform.position + new Vector3(0.0f,-0.5f, 0.0f);
                                FileIOSystem.Instance.invendatabase.mydata[3000].count --;
                            }
                        }
                        break;
                        default:{}break;
                    }
                    //ChangeRadial(CurrentPage(), buttonItSelf);
                }
                break;
                case "RadialButton3":
                {
                    buttonItSelf.transform.parent.gameObject.SetActive(false);
                    for(int i = 1; i < 5; i++)
                        buttonItSelf.transform.parent.parent.parent.GetChild(i).gameObject.SetActive(true);
                    switch(CurrentPage())
                    {
                        case 0:
                        {
                            if(FileIOSystem.Instance.invendatabase.mydata[2001].count > 0)
                            {
                                GameObject unPackedItem = Instantiate(database.ItemData[database.ItemData.FindIndex(data => data.ID == 2001)].Prefab);
                                unPackedItem.transform.localPosition = GameManager.Instance.leftHand.transform.position + new Vector3(0.0f,-0.5f, 0.0f);
                                FileIOSystem.Instance.invendatabase.mydata[2001].count --;
                            }
                        }
                        break;
                        case 1:
                        {
                            if(FileIOSystem.Instance.invendatabase.mydata[2006].count > 0)
                            {
                                GameObject unPackedItem = Instantiate(database.ItemData[database.ItemData.FindIndex(data => data.ID == 2006)].Prefab);
                                unPackedItem.transform.localPosition = GameManager.Instance.leftHand.transform.position + new Vector3(0.0f,-0.5f, 0.0f);
                                FileIOSystem.Instance.invendatabase.mydata[2006].count --;
                            }
                        }
                        break;
                        case 2:
                        {
                            if(FileIOSystem.Instance.invendatabase.mydata[3001].count > 0)
                            {
                                GameObject unPackedItem = Instantiate(database.ItemData[database.ItemData.FindIndex(data => data.ID == 3001)].Prefab);
                                unPackedItem.transform.localPosition = GameManager.Instance.leftHand.transform.position + new Vector3(0.0f,-0.5f, 0.0f);
                                FileIOSystem.Instance.invendatabase.mydata[3001].count --;
                            }
                        }
                        break;
                        default:{}break;
                    }
                    //ChangeRadial(CurrentPage(), buttonItSelf);
                }
                break;
                case "RadialButton4":
                {
                    buttonItSelf.transform.parent.gameObject.SetActive(false);
                    for(int i = 1; i < 5; i++)
                        buttonItSelf.transform.parent.parent.parent.GetChild(i).gameObject.SetActive(true);
                    switch(CurrentPage())
                    {
                        case 0:
                        {
                            if(FileIOSystem.Instance.invendatabase.mydata[2002].count > 0)
                            {
                                GameObject unPackedItem = Instantiate(database.ItemData[database.ItemData.FindIndex(data => data.ID == 2002)].Prefab);
                                unPackedItem.transform.localPosition = GameManager.Instance.leftHand.transform.position + new Vector3(0.0f,-0.5f, 0.0f);
                                FileIOSystem.Instance.invendatabase.mydata[2002].count --;
                            }
                        }
                        break;
                        case 1:
                        {
                            if(FileIOSystem.Instance.invendatabase.mydata[2007].count > 0)
                            {
                                GameObject unPackedItem = Instantiate(database.ItemData[database.ItemData.FindIndex(data => data.ID == 2007)].Prefab);
                                unPackedItem.transform.localPosition = GameManager.Instance.leftHand.transform.position + new Vector3(0.0f,-0.5f, 0.0f);
                                FileIOSystem.Instance.invendatabase.mydata[2007].count --;
                            }
                        }
                        break;
                        case 2:
                        {
                            if(FileIOSystem.Instance.invendatabase.mydata[3002].count > 0)
                            {
                                GameObject unPackedItem = Instantiate(database.ItemData[database.ItemData.FindIndex(data => data.ID == 3002)].Prefab);
                                unPackedItem.transform.localPosition = GameManager.Instance.leftHand.transform.position + new Vector3(0.0f,-0.5f, 0.0f);
                                FileIOSystem.Instance.invendatabase.mydata[3002].count --;
                            }
                        }
                        break;
                        default:
                        break;
                    }
                    //ChangeRadial(CurrentPage(), buttonItSelf);
                }
                break;
                case "RadialButton5":
                {
                    buttonItSelf.transform.parent.gameObject.SetActive(false);
                    for(int i = 1; i < 5; i++)
                        buttonItSelf.transform.parent.parent.parent.GetChild(i).gameObject.SetActive(true);
                    switch(CurrentPage())
                    {
                        case 0:
                        {
                            if(FileIOSystem.Instance.invendatabase.mydata[2003].count > 0)
                            {
                                GameObject unPackedItem = Instantiate(database.ItemData[database.ItemData.FindIndex(data => data.ID == 2003)].Prefab);
                                unPackedItem.transform.localPosition = GameManager.Instance.leftHand.transform.position + new Vector3(0.0f,-0.5f, 0.0f);
                                FileIOSystem.Instance.invendatabase.mydata[2003].count --;
                            }
                        }
                        break;
                        case 1:
                        {
                            if(FileIOSystem.Instance.invendatabase.mydata[2008].count > 0)
                            {
                                GameObject unPackedItem = Instantiate(database.ItemData[database.ItemData.FindIndex(data => data.ID == 2008)].Prefab);
                                unPackedItem.transform.localPosition = GameManager.Instance.leftHand.transform.position + new Vector3(0.0f,-0.5f, 0.0f);
                                FileIOSystem.Instance.invendatabase.mydata[2008].count --;
                            }
                        }
                        break;
                        case 2:
                        {
                            if(FileIOSystem.Instance.invendatabase.mydata[3003].count > 0)
                            {
                                GameObject unPackedItem = Instantiate(database.ItemData[database.ItemData.FindIndex(data => data.ID == 3003)].Prefab);
                                unPackedItem.transform.localPosition = GameManager.Instance.leftHand.transform.position + new Vector3(0.0f,-0.5f, 0.0f);
                                FileIOSystem.Instance.invendatabase.mydata[3003].count --;
                            }
                        }
                        break;
                        default:
                        break;
                    }
                    //ChangeRadial(CurrentPage(), buttonItSelf);
                }
                break;
                case "RadialButton6":
                {
                    buttonItSelf.transform.parent.gameObject.SetActive(false);
                    for(int i = 1; i < 5; i++)
                        buttonItSelf.transform.parent.parent.parent.GetChild(i).gameObject.SetActive(true);
                    switch(CurrentPage())
                    {
                        case 0:
                        {
                            if(FileIOSystem.Instance.invendatabase.mydata[2004].count > 0)
                            {
                                GameObject unPackedItem = Instantiate(database.ItemData[database.ItemData.FindIndex(data => data.ID == 2004)].Prefab);
                                unPackedItem.transform.localPosition = GameManager.Instance.leftHand.transform.position + new Vector3(0.0f,-0.5f, 0.0f);
                                FileIOSystem.Instance.invendatabase.mydata[2004].count --;
                            }
                        }
                        break;
                        case 1:
                        break;
                        case 2:
                        {
                            if(FileIOSystem.Instance.invendatabase.mydata[3004].count > 0)
                            {
                                GameObject unPackedItem = Instantiate(database.ItemData[database.ItemData.FindIndex(data => data.ID == 3004)].Prefab);
                                unPackedItem.transform.localPosition = GameManager.Instance.leftHand.transform.position + new Vector3(0.0f,-0.5f, 0.0f);
                                FileIOSystem.Instance.invendatabase.mydata[3004].count --;
                            }
                        }
                        break;
                        default:
                        break;
                    }
                    //ChangeRadial(CurrentPage(), buttonItSelf);
                }
                break;
                default:
                {
                    Debug.Log(buttonItSelf.name);
                }
                break;

           }
        }
    }

    /*private void OnTriggerStay(Collider other)
    {
        
    }*/
}
