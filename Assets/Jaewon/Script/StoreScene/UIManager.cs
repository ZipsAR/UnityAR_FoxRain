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
        testtext.text = "��ư�� ���Ƚ��ϴ�.";
    }
    public void ExitButton()
    {
        Application.Quit();
    }
}
