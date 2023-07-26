using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public Text testtext;
    public void TestButton()
    {
        testtext.text = "버튼이 눌렸습니다.";
    }
    public void ExitButton()
    {
        Application.Quit();
    }
}
