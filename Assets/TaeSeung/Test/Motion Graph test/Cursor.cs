using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Cursor : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject mousepointer;
    public GameObject DebugPrefab;
    public Transform ParentPosition;
    public TMP_Text text;
    public TMP_Text comparetext;
    public TMP_Text resulttext;

    private Camera MainCamera;
    private int count = 0;

    private List<Vector3> vectors;
    private List<Vector3> Idealvectors;

    bool oncecheck = true;

    //float min = 999;
    //float max = -999;


    void Start()
    {
        MainCamera = Camera.main;
        vectors = new();
        Idealvectors = new();

    }

    // Update is called once per frame
    void Update()
    {
        mousepointerposition();
        if (oncecheck)
        {
            StartCoroutine(RenderDebugPoint());
            oncecheck = false;
        }
    }

    void mousepointerposition()
    {
        Vector3 mousepos = Input.mousePosition;
        Vector3 windowposition = MainCamera.ScreenPointToRay(mousepos).origin;
        windowposition *= 20;
        mousepointer.transform.position = windowposition;
    }

    IEnumerator RenderDebugPoint()
    {
        yield return new WaitForSeconds(1.25f);
        while (true)
        {
            //실험값

            GameObject point;
            Vector3 newpoint = mousepointer.transform.position/20;
            if (vectors.Count > 0)
            {
                if (Vector3.Distance(vectors[vectors.Count - 1], newpoint) > 0.01)
                {
                    vectors.Add(newpoint);
                    point = Instantiate(DebugPrefab, ParentPosition);
                    point.transform.position = newpoint;
                }

                else
                {
                    yield return new WaitForSeconds(0.01f);
                    continue;
                }
            }
            else
            {
                point = Instantiate(DebugPrefab, ParentPosition);
                point.transform.position = mousepointer.transform.position;
                vectors.Add(newpoint);
            }


            //측정값
                Vector3 idealpoint = new Vector3(newpoint.x, Mathf.Sin(newpoint.x), newpoint.z);
                GameObject point2 = Instantiate(DebugPrefab, ParentPosition);
                point2.transform.position = idealpoint;
                
                point2.GetComponent<SpriteRenderer>().color = Color.blue;

                Idealvectors.Add(idealpoint);

                count++;

                text.text += " " + newpoint + '\n';
                comparetext.text += " " + idealpoint + "\n";

                if (count == 50)
                {
                    vectors.Sort(Vectorcompare);
                    Idealvectors.Sort(Vectorcompare);
                    float corrrelation = PiersenCorrelation(vectors, Idealvectors, count);

                    resulttext.text = "Corr : " + corrrelation;

                    if(Mathf.Abs(corrrelation) > 0.85)
                    {
                        resulttext.text += "\n쓰다듬는 모션 맞음";
                    }
                    else
                    {
                    resulttext.text += "\n어색함";
                    }   
                    count = 0;
                    yield break;
                }
                yield return new WaitForSeconds(0.01f);
        }
    }


    private int Vectorcompare(Vector3 a, Vector3 b){
        return a.x < b.x ? -1 : 1;
    }


    private float PiersenCorrelation(List<Vector3> real, List<Vector3> ideal, int count)
    {
        float Meanreal = Mean(Sumy(real), count);
        float Meanideal=  Mean(Sumy(ideal), count);

        float Covariance = CoVariancey(real, ideal, Meanreal, Meanideal);
        float variancereal = Variancey(real, Meanreal);
        float varianceideal = Variancey(ideal, Meanideal);


        float piersenCorrelation = (Covariance / (count - 1)) /(Mathf.Sqrt(variancereal / (count - 1)) * Mathf.Sqrt(varianceideal / (count - 1)));

        return piersenCorrelation;

    }


    private float Sumy(List<Vector3> Sumlist)
    {
        float sum = 0f;
        for (int i = 0; i < Sumlist.Count; i++) sum += Sumlist[i].y;
        return sum;
    }

    private float CoVariancey(List<Vector3> Sumlist1, List<Vector3> Sumlist2, float Mean1, float Mean2)
    {
        float sum = 0f;
        for (int i = 0; i < Sumlist1.Count; i++) {
            sum += (Sumlist1[i].y - Mean1) * (Sumlist2[i].y - Mean2);
        }
        return sum;
    }

    private float Variancey(List<Vector3> Sumlist, float Mean)
    {
        float sum = 0f;
        for (int i = 0; i < Sumlist.Count; i++)
        {
            sum += Mathf.Pow((Sumlist[i].y - Mean), 2); 
        }
        return sum;
    }


    private float Mean(float sum, int count)
    {
        float mean = sum / count;
        return mean;
    }







    public void ClickEvent()
    {
        for (int i = 0; i < ParentPosition.childCount; i++) {
            Destroy(ParentPosition.GetChild(i).gameObject);
       }
        vectors.Clear();
        Idealvectors.Clear();
        resulttext.text = "";
        text.text = "";
        comparetext.text = "";
        StartCoroutine(RenderDebugPoint());
    }


}


