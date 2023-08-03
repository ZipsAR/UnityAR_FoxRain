using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ParticleManager : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem particle; 

    private void Start()
    {
        particle.Stop();
    }

    public void OnButtonClick()
    {
        particle.transform.position = new Vector3(10, 0, 0);
        particle.Play();
    }
}
