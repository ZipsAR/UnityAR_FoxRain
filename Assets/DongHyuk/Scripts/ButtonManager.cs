using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public GameObject buttonItSelf;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
                    }
                    else
                    {
                        buttonItSelf.transform.GetChild(0).gameObject.SetActive(true);
                        buttonItSelf.transform.GetChild(1).gameObject.SetActive(false);
                    }
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
                }
                break;
                case "RadialButton3":
                {
                    buttonItSelf.transform.parent.gameObject.SetActive(false);
                    for(int i = 1; i < 5; i++)
                        buttonItSelf.transform.parent.parent.parent.GetChild(i).gameObject.SetActive(true);
                }
                break;
                case "RadialButton4":
                {
                    buttonItSelf.transform.parent.gameObject.SetActive(false);
                    for(int i = 1; i < 5; i++)
                        buttonItSelf.transform.parent.parent.parent.GetChild(i).gameObject.SetActive(true);
                }
                break;
                case "RadialButton5":
                {
                    buttonItSelf.transform.parent.gameObject.SetActive(false);
                    for(int i = 1; i < 5; i++)
                        buttonItSelf.transform.parent.parent.parent.GetChild(i).gameObject.SetActive(true);
                }
                break;
                case "RadialButton6":
                {
                    buttonItSelf.transform.parent.gameObject.SetActive(false);
                    for(int i = 1; i < 5; i++)
                        buttonItSelf.transform.parent.parent.parent.GetChild(i).gameObject.SetActive(true);
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
        
        if(other.gameObject.name == "index003.Col")
        {
           switch(buttonItSelf.name)
           {
                case "ButtonInteract":
                {
                    buttonItSelf.GetComponent<Image>().material.color = new Color(200,200,200);
                }
                break;
                case "ButtonBackpack":
                {
                    buttonItSelf.GetComponent<Image>().material.color = new Color(200,200,200);
                }
                break;
                case "ButtonX":
                {
                    buttonItSelf.GetComponent<Image>().material.color = new Color(200,200,200);
                }
                break;
                case "ButtonEdit":
                {
                    buttonItSelf.GetComponent<Image>().material.color = new Color(200,200,200);
                }
                break;
                case "ButtonBasket":
                {
                    buttonItSelf.GetComponent<Image>().material.color = new Color(200,200,200);
                }
                break;
                case "ButtonHome":
                {
                    buttonItSelf.GetComponent<Image>().material.color = new Color(200,200,200);
                }
                break;
                case "RadialButton1":
                {
                    buttonItSelf.GetComponent<Image>().material.color = new Color(200,200,200);
                }
                break;
                case "RadialButton2":
                {
                    buttonItSelf.GetComponent<Image>().material.color = new Color(200,200,200);
                }
                break;
                case "RadialButton3":
                {
                    buttonItSelf.GetComponent<Image>().material.color = new Color(200,200,200);
                }
                break;
                case "RadialButton4":
                {
                    buttonItSelf.GetComponent<Image>().material.color = new Color(200,200,200);
                }
                break;
                case "RadialButton5":
                {
                    buttonItSelf.GetComponent<Image>().material.color = new Color(200,200,200);
                }
                break;
                case "RadialButton6":
                {
                    buttonItSelf.GetComponent<Image>().material.color = new Color(200,200,200);
                }
                break;
                default:
                break;

           }
        }
    }*/
}
