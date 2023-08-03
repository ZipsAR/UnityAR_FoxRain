using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        
        if(other.gameObject.name == "QC Hand Right")
        {
           Debug.Log("button pushed by Right hand");
           GameManager.Instance.LoadScene("InteractMode");
        }
    }

}
