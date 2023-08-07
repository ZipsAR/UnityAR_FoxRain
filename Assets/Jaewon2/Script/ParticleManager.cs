using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ParticleManager : MonoBehaviour
{
    public ParticleSystem particle; 

    private void Start()
    {
        particle.Play();
    }

    public void PlayParticle()
    {
        this.particle.Play();
    }
}
