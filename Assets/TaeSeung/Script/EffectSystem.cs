using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSystem : Singleton<EffectSystem>
{
    // Start is called before the first frame update

    public Transform Parentposition;

    public ParticleSystem placedeffect;
    private GameObject placedeffectobj;


    void Start()
    {
        
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
    }


    public void playplaceeffect(Vector3 position)
    {
        if (!placedeffectobj)
        {
            print("play!");
            placedeffectobj = Instantiate(placedeffect.transform.gameObject, Parentposition);
            placedeffectobj.transform.localPosition = position;
            placedeffectobj.GetComponent<ParticleSystem>().Play();
        }

    }


}
