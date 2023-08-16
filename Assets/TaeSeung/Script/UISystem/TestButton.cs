using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestButton : MonoBehaviour
{
    // Start is called before the first frame update
    public void btn()
    {
        GameManager.Instance.LoadScene("HousingMode");
    }

    public void A()
    {
        GameManager.Instance.LoadScene("InteractMode");
    }

    public void B()
    {
        GameManager.Instance.LoadScene("StoreScene");
    }
}
