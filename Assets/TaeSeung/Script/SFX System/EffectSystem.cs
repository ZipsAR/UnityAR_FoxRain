using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSystem : MonoBehaviour
{
    public Transform Parentposition;
    public ParticleSystem placedeffect;
    public ParticleSystem spawneffect;
    private GameObject placedeffectobj;
    private GameObject spawneffectobj;

    public static EffectSystem Instance;


    private void Start()
    {
        if (EffectSystem.Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }



    // Update is called once per frame
    void Update()
    {
        if (placedeffectobj)
        {
            if (placedeffectobj.GetComponent<ParticleSystem>().isStopped)
            { 
                Destroy(placedeffectobj);
                placedeffectobj = null;
            }
        }
        if (spawneffectobj)
        {
            if (spawneffectobj.GetComponent<ParticleSystem>().isStopped)
            {
                Destroy(spawneffectobj);
                spawneffectobj= null;
            }
        }
    }


    public void playplaceeffect(Vector3 position)
    {
        if (!placedeffectobj)
        {
            placedeffectobj = Instantiate(placedeffect.transform.gameObject);
            placedeffectobj.transform.position = position;
            placedeffectobj.GetComponent<ParticleSystem>().Play();
        }
    }

    public void playspawneffect(Vector3 position)
    {
        if (!spawneffectobj)
        {
            spawneffectobj = Instantiate(spawneffect.transform.gameObject);
            spawneffectobj.transform.position = position;
            spawneffectobj.GetComponent<ParticleSystem>().Play();
        }
    }


}
